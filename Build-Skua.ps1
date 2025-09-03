<#
.SYNOPSIS
    Automated build script for Skua project - builds x64, x86, and WiX installer
.DESCRIPTION
    This script automates the build process for the Skua project including:
    - Clean previous builds
    - Build x64 and x86 configurations
    - Build WiX installer package
    - Copy outputs to distribution folder
.NOTES
    To run this script without changing execution policy:
    PowerShell.exe -ExecutionPolicy Bypass -File Build-Skua.ps1
    
    Or right-click and select "Run with PowerShell"
.PARAMETER Configuration
    Build configuration (Debug or Release). Default is Release.
.PARAMETER Platforms
    Array of platforms to build. Default is @("x64", "x86")
.PARAMETER BuildInstaller
    Whether to build the WiX installer. Default is $true.
.PARAMETER Clean
    Whether to clean before building. Default is $true.
.EXAMPLE
    .\Build-Skua.ps1
    Builds all platforms in Release mode with installer
.EXAMPLE
    .\Build-Skua.ps1 -Configuration Debug -Platforms @("x64") -BuildInstaller $false
    Builds only x64 in Debug mode without installer
#>

param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    
    [ValidateSet("x64", "x86")]
    [string[]]$Platforms = @("x64", "x86"),
    
    [bool]$BuildInstaller = $true,
    
    [bool]$Clean = $true,
    
    [string]$OutputPath = ".\build"
)

# Script configuration
$ProgressPreference = "SilentlyContinue"

# Colors for output
function Write-Header {
    param([string]$Message)
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host $Message -ForegroundColor Cyan
    Write-Host "========================================`n" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host "✓ $Message" -ForegroundColor Green
}

function Write-BuildError {
    param([string]$Message)
    Write-Host "✗ $Message" -ForegroundColor Red
}

function Write-Info {
    param([string]$Message)
    Write-Host "ℹ $Message" -ForegroundColor Yellow
}

# Check prerequisites
function Test-Prerequisites {
    Write-Header "Checking Prerequisites"
    
    $hasErrors = $false
    
    # Check for .NET SDK
    try {
        $dotnetVersion = dotnet --version
        Write-Success ".NET SDK found: $dotnetVersion"
    }
    catch {
        Write-BuildError ".NET SDK not found. Please install from https://dotnet.microsoft.com/download"
        $hasErrors = $true
    }
    
    # Check for MSBuild (for WiX)
    if ($BuildInstaller) {
        $msbuildPath = Get-MSBuildPath
        if ($msbuildPath) {
            Write-Success "MSBuild found: $msbuildPath"
        }
        else {
            Write-BuildError "MSBuild not found. Please install Visual Studio or Build Tools"
            $hasErrors = $true
        }
        
        # Check for WiX v6 CLI
        try {
            $wixVersion = wix --version 2>$null
            if ($LASTEXITCODE -eq 0) {
                Write-Success "WiX CLI found: v$wixVersion"
            }
            else {
                throw "WiX CLI not found"
            }
        }
        catch {
            Write-BuildError "WiX CLI v6+ not found. Please install using: dotnet tool install --global wix"
            Write-Info "Documentation: https://wixtoolset.org/docs/tools/"
            $hasErrors = $true
        }
    }
    
    if ($hasErrors) {
        throw "Prerequisites check failed. Please install missing components."
    }
    
    Write-Success "All prerequisites met"
}

function Get-MSBuildPath {
    # Try VS 2022 first
    $vsWhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
    if (Test-Path $vsWhere) {
        $installPath = & $vsWhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
        if ($installPath) {
            $msbuildPath = Join-Path $installPath "MSBuild\Current\Bin\MSBuild.exe"
            if (Test-Path $msbuildPath) {
                return $msbuildPath
            }
        }
    }
    
    # Try VS 2019
    $msbuildPath = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\*\MSBuild\Current\Bin\MSBuild.exe"
    $found = Get-ChildItem -Path $msbuildPath -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($found) {
        return $found.FullName
    }
    
    # Try standalone Build Tools
    $msbuildPath = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
    if (Test-Path $msbuildPath) {
        return $msbuildPath
    }
    
    return $null
}

function Clean-Solution {
    Write-Header "Cleaning Previous Builds"
    
    # Clean output directories
    $dirsToClean = @("bin", "obj", "build", "dist", "publish")
    foreach ($dir in $dirsToClean) {
        if (Test-Path $dir) {
            Write-Info "Removing $dir..."
            Remove-Item -Path $dir -Recurse -Force -ErrorAction SilentlyContinue
        }
    }
    
    # Clean each project
    Get-ChildItem -Path . -Directory | ForEach-Object {
        $projectDir = $_.FullName
        foreach ($dir in @("bin", "obj")) {
            $targetDir = Join-Path $projectDir $dir
            if (Test-Path $targetDir) {
                Write-Info "Cleaning $($_.Name)\$dir..."
                Remove-Item -Path $targetDir -Recurse -Force -ErrorAction SilentlyContinue
            }
        }
    }
    
    Write-Success "Clean completed"
}

function Build-Platform {
    param(
        [string]$Platform,
        [string]$Config
    )
    
    Write-Header "Building $Platform - $Config"
    
    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    
    try {
        # Restore NuGet packages
        Write-Info "Restoring NuGet packages..."
        # Check if WiX CLI is installed
        $wixInstalled = $false
        try {
            $null = wix --version 2>$null
            if ($LASTEXITCODE -eq 0) {
                $wixInstalled = $true
            }
        }
        catch {
            $wixInstalled = $false
        }
        
        if (-not $wixInstalled -and (Test-Path "Skua.Installer\Skua.Installer.wixproj")) {
            Write-Info "WiX CLI not installed - restoring C# projects only..."
            # Restore each C# project individually to skip WiX project
            $projects = Get-ChildItem -Path . -Filter "*.csproj" -Recurse
            foreach ($project in $projects) {
                $result = dotnet restore $project.FullName --verbosity minimal 2>&1
                if ($LASTEXITCODE -ne 0) {
                    Write-BuildError "Failed to restore $($project.Name)"
                    Write-Host $result -ForegroundColor Red
                    throw "Restore failed"
                }
            }
        }
        else {
            # Restore entire solution if WiX is installed
            Write-Info "Restoring full solution..."
            $result = dotnet restore Skua.sln --verbosity minimal 2>&1
            if ($LASTEXITCODE -ne 0) {
                # If solution restore fails, try restoring projects only
                Write-Info "Solution restore failed, trying project-by-project restore..."
                $projects = Get-ChildItem -Path . -Filter "*.csproj" -Recurse
                foreach ($project in $projects) {
                    $result = dotnet restore $project.FullName --verbosity minimal 2>&1
                    if ($LASTEXITCODE -ne 0) {
                        Write-BuildError "Failed to restore $($project.Name)"
                        Write-Host $result -ForegroundColor Red
                        throw "Restore failed"
                    }
                }
            }
        }
        
        # Build the solution for the specified platform
        Write-Info "Building solution..."
        
        # If WiX CLI is not installed, build main projects directly
        if (-not $wixInstalled -and (Test-Path "Skua.Installer\Skua.Installer.wixproj")) {
            Write-Info "Building C# projects directly (WiX CLI not installed)..."
            # Build main projects directly
            $mainProjects = @(
                "Skua.App.WPF\Skua.App.WPF.csproj",
                "Skua.Manager\Skua.Manager.csproj"
            )
            
            foreach ($proj in $mainProjects) {
                if (Test-Path $proj) {
                    Write-Info "Building $proj..."
                    $projBuildArgs = @(
                        "build",
                        $proj,
                        "--configuration", $Config,
                        "-p:Platform=$Platform",
                        "--no-restore",
                        "--verbosity", "minimal",
                        "-p:WarningLevel=0"
                    )
                    
                    if ($Platform -eq "x86") {
                        $projBuildArgs += "-p:PlatformTarget=x86"
                    }
                    
                    $result = & dotnet $projBuildArgs 2>&1
                    if ($LASTEXITCODE -ne 0) {
                        Write-BuildError "Failed to build $proj"
                        Write-Host $result -ForegroundColor Red
                        throw "Build failed"
                    }
                }
            }
        }
        else {
            # Build entire solution if WiX CLI is installed
            $buildArgs = @(
                "build",
                "Skua.sln",
                "--configuration", $Config,
                "-p:Platform=$Platform",
                "--no-restore",
                "--verbosity", "minimal",
                "-p:WarningLevel=0"  # Suppress warnings for cleaner output
            )
            
            # Special handling for x86
            if ($Platform -eq "x86") {
                $buildArgs += "-p:PlatformTarget=x86"
            }
            
            $result = & dotnet $buildArgs 2>&1
            
            if ($LASTEXITCODE -ne 0) {
                Write-BuildError "Build failed for $Platform"
                Write-Host $result -ForegroundColor Red
                throw "Build failed"
            }
        }
        
        
        $stopwatch.Stop()
        Write-Success "Build completed for $Platform in $($stopwatch.Elapsed.TotalSeconds.ToString('F2')) seconds"
        
        # Copy outputs to distribution folder
        Copy-BuildOutputs -Platform $Platform -Config $Config
    }
    catch {
        $stopwatch.Stop()
        Write-BuildError "Build failed after $($stopwatch.Elapsed.TotalSeconds.ToString('F2')) seconds"
        throw $_
    }
}

function Copy-BuildOutputs {
    param(
        [string]$Platform,
        [string]$Config
    )
    
    Write-Info "Copying build outputs..."
    
    $outputDir = Join-Path $OutputPath "$Config\$Platform"
    if (-not (Test-Path $outputDir)) {
        New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
    }
    
    # Main application outputs
    $mainProjects = @(
        "Skua.App.WPF",
        "Skua.Manager"
    )
    
    foreach ($project in $mainProjects) {
        $sourceDir = Join-Path "." "$project\bin\$Platform\$Config\net6.0-windows"
        if (-not (Test-Path $sourceDir)) {
            $sourceDir = Join-Path "." "$project\bin\$Config\net6.0-windows"
        }
        
        if (Test-Path $sourceDir) {
            $destDir = Join-Path $outputDir $project
            Write-Info "Copying $project to $destDir..."
            
            if (Test-Path $destDir) {
                Remove-Item -Path $destDir -Recurse -Force
            }
            
            Copy-Item -Path $sourceDir -Destination $destDir -Recurse -Force
            Write-Success "Copied $project"
        }
        else {
            Write-Info "Skipping $project (not built for this platform)"
        }
    }
}

function Build-Installer {
    param(
        [string]$Platform
    )
    
    if (-not $BuildInstaller) {
        Write-Info "Skipping installer build (disabled)"
        return
    }
    
    Write-Header "Building WiX Installer for $Platform"
    
    $msbuildPath = Get-MSBuildPath
    if (-not $msbuildPath) {
        Write-BuildError "MSBuild not found, skipping installer"
        return
    }
    
    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    
    try {
        $installerProject = ".\Skua.Installer\Skua.Installer.wixproj"
        
        if (-not (Test-Path $installerProject)) {
            Write-BuildError "Installer project not found: $installerProject"
            return
        }
        
        Write-Info "Building installer..."
        
        $msbuildArgs = @(
            $installerProject,
            "/p:Configuration=$Configuration",
            "/p:Platform=$Platform",
            "/t:Rebuild",
            "/verbosity:minimal",
            "/nologo"
        )
        
        $result = & $msbuildPath $msbuildArgs 2>&1
        
        if ($LASTEXITCODE -ne 0) {
            Write-BuildError "Installer build failed"
            Write-Host $result -ForegroundColor Red
            throw "Installer build failed"
        }
        
        # Copy installer to distribution folder
        $installerOutput = ".\Skua.Installer\bin\$Platform\$Configuration\*.msi"
        $installers = Get-ChildItem -Path $installerOutput -ErrorAction SilentlyContinue
        
        if ($installers) {
            $installerDest = Join-Path $OutputPath "Installers"
            if (-not (Test-Path $installerDest)) {
                New-Item -ItemType Directory -Path $installerDest -Force | Out-Null
            }
            
            foreach ($installer in $installers) {
                $destName = "Skua_${Configuration}_${Platform}_$($installer.Name)"
                $destPath = Join-Path $installerDest $destName
                Copy-Item -Path $installer.FullName -Destination $destPath -Force
                Write-Success "Installer created: $destName"
            }
        }
        
        $stopwatch.Stop()
        Write-Success "Installer build completed in $($stopwatch.Elapsed.TotalSeconds.ToString('F2')) seconds"
    }
    catch {
        $stopwatch.Stop()
        Write-BuildError "Installer build failed after $($stopwatch.Elapsed.TotalSeconds.ToString('F2')) seconds"
        # Don't rethrow - installer is optional
        Write-Info "Continuing without installer..."
    }
}

function Show-Summary {
    param(
        [TimeSpan]$TotalTime,
        [bool]$Success
    )
    
    Write-Header "Build Summary"
    
    if ($Success) {
        Write-Success "Build completed successfully!"
    }
    else {
        Write-BuildError "Build completed with errors"
    }
    
    Write-Info "Configuration: $Configuration"
    Write-Info "Platforms: $($Platforms -join ', ')"
    Write-Info "Installer: $(if ($BuildInstaller) { 'Yes' } else { 'No' })"
    Write-Info "Total time: $($TotalTime.TotalSeconds.ToString('F2')) seconds"
    
    if ($Success -and (Test-Path $OutputPath)) {
        Write-Info "`nOutput location: $(Resolve-Path $OutputPath)"
        
        # List created files
        Write-Info "`nCreated artifacts:"
        Get-ChildItem -Path $OutputPath -Recurse -File | 
            Where-Object { $_.Extension -in @('.exe', '.msi') } |
            ForEach-Object {
                $relativePath = $_.FullName.Replace((Resolve-Path $OutputPath).Path, '').TrimStart('\')
                Write-Host "  • $relativePath" -ForegroundColor Gray
            }
    }
}

# Main execution
function Main {
    $totalStopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    $success = $false
    $exitCode = 0
    
    # Set error action preference here to ensure functions are loaded first
    $ErrorActionPreference = "Stop"
    
    try {
        # Test if functions are available
        if (-not (Get-Command Write-Header -ErrorAction SilentlyContinue)) {
            Write-Host "✗ Critical Error: Script functions not loaded properly. Please run the script directly." -ForegroundColor Red
            exit 1
        }
        
        Write-Header "Skua Build Automation"
        Write-Info "Starting build process..."
        
        # Check prerequisites
        Test-Prerequisites
        
        # Clean if requested
        if ($Clean) {
            Clean-Solution
        }
        
        # Build each platform
        foreach ($platform in $Platforms) {
            Build-Platform -Platform $platform -Config $Configuration
            
            # Build installer for this platform
            if ($BuildInstaller) {
                Build-Installer -Platform $platform
            }
        }
        
        $success = $true
    }
    catch {
        # Check if function exists before calling it
        if (Get-Command Write-BuildError -ErrorAction SilentlyContinue) {
            Write-BuildError "Build failed: $_"
        }
        else {
            Write-Host "✗ Build failed: $_" -ForegroundColor Red
        }
        $exitCode = 1
        $success = $false
    }
    finally {
        $totalStopwatch.Stop()
        Show-Summary -TotalTime $totalStopwatch.Elapsed -Success $success
        
        # Keep window open after execution
        Wait-ForKeyPress -ExitCode $exitCode
    }
}

function Wait-ForKeyPress {
    param(
        [int]$ExitCode = 0
    )
    
    Write-Host "`n========================================" -ForegroundColor DarkGray
    if ($ExitCode -eq 0) {
        Write-Host "Press any key to exit..." -ForegroundColor Green
    }
    else {
        Write-Host "Build failed. Press any key to exit..." -ForegroundColor Red
    }
    Write-Host "========================================" -ForegroundColor DarkGray
    
    # Check if running in an interactive session
    if ($Host.UI.RawUI.KeyAvailable -or [Environment]::UserInteractive) {
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    }
    
    exit $ExitCode
}

# Check if the script is being run with -Help parameter
if ($PSBoundParameters.ContainsKey('Help')) {
    Write-Host @"
`nSkua Build Script
=================

Usage: Build-Skua.ps1 [options]

Parameters:
  -Configuration <String>   Build configuration (Debug or Release). Default: Release
  -Platforms <String[]>     Platforms to build. Default: @("x64", "x86")
  -BuildInstaller <Boolean> Whether to build WiX installer. Default: `$true
  -Clean <Boolean>          Whether to clean before building. Default: `$true
  -OutputPath <String>      Output directory path. Default: .\build

Examples:
  .\Build-Skua.ps1
      Builds all platforms in Release mode with installer
  
  .\Build-Skua.ps1 -Configuration Debug
      Builds all platforms in Debug mode with installer
  
  .\Build-Skua.ps1 -Platforms @("x64") -BuildInstaller `$false
      Builds only x64 without installer
  
  .\Build-Skua.ps1 -Clean `$false
      Builds without cleaning previous builds

To run this script directly (double-click or from Explorer):
  1. Right-click the script and select "Run with PowerShell"
  2. Or create a shortcut with:
     Target: powershell.exe -ExecutionPolicy Bypass -File "Build-Skua.ps1"

"@
    exit 0
}

# Run the build
Main
