# Skua Build Guide

This document provides instructions for building the Skua project from source, including automated build scripts for x64, x86, and WiX installer creation.

## Table of Contents
- [Prerequisites](#prerequisites)
- [Quick Start](#quick-start)
- [Build Scripts](#build-scripts)
- [Manual Building](#manual-building)
- [CI/CD](#cicd)
- [Troubleshooting](#troubleshooting)

## Prerequisites

### Required Software

1. **.NET 9.0 SDK or later**
   - Download from: https://dotnet.microsoft.com/download
   - Verify installation: `dotnet --version`

2. **Visual Studio 2019/2022** (for MSBuild and WiX support)
   - Community Edition or higher
   - Workloads required:
     - .NET desktop development
     - Desktop development with C++
   - Or install Build Tools for Visual Studio separately

3. **WiX Toolset v6.0.2** (for installer)
   - Download from: https://wixtoolset.org/releases/
   - **Note**: WiX v4 is not compatible with this project
   - The installer build is optional

4. **PowerShell 5.1 or later**
   - Pre-installed on Windows 10+
   - For PowerShell Core: https://github.com/PowerShell/PowerShell

### Optional Software

- **Git** for version control
- **GitHub CLI** for releases

## Quick Start

### Easiest Method: PowerShell Script

1. Clone the repository:
   ```bash
   git clone https://github.com/BrenoHenrike/Skua.git
   cd Skua
   ```

2. Right-click `Build-Skua.ps1` and select "Run with PowerShell"
   - This builds Release configuration for both x64 and x86
   - Includes WiX installer creation
   - Output is placed in `build` folder
   - **The window stays open after completion** showing build results

### Alternative: Run from Terminal

```powershell
# Build Release (default)
.\Build-Skua.ps1

# Build Debug configuration
.\Build-Skua.ps1 -Configuration Debug

# Skip cleaning
.\Build-Skua.ps1 -Clean:$false

# Skip installer
.\Build-Skua.ps1 -BuildInstaller:$false

# Combine options
.\Build-Skua.ps1 -Configuration Debug -BuildInstaller:$false
```

## Build Scripts

### PowerShell Script (`Build-Skua.ps1`)

The main build automation script with full control over the build process.

#### Basic Usage

```powershell
# Build everything (x64, x86, installer)
.\Build-Skua.ps1

# Build specific configuration
.\Build-Skua.ps1 -Configuration Debug

# Build specific platforms
.\Build-Skua.ps1 -Platforms @("x64")
.\Build-Skua.ps1 -Platforms @("x86")

# Skip installer
.\Build-Skua.ps1 -BuildInstaller:$false

# Skip cleaning
.\Build-Skua.ps1 -Clean:$false

# Custom output path
.\Build-Skua.ps1 -OutputPath "C:\MyBuilds"
```

#### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Configuration | String | Release | Build configuration (Debug/Release) |
| Platforms | String[] | @("x64", "x86") | Target platforms to build |
| BuildInstaller | Boolean | $true | Whether to build WiX installer |
| Clean | Boolean | $true | Clean before building |
| OutputPath | String | .\build | Output directory for artifacts |

#### Output Structure

```
build/
├── Release/
│   ├── x64/
│   │   ├── Skua.App.WPF/
│   │   └── Skua.Manager/
│   └── x86/
│       ├── Skua.App.WPF/
│       └── Skua.Manager/
└── Installers/
    ├── Skua_Release_x64_Skua.Installer.msi
    └── Skua_Release_x86_Skua.Installer.msi
```

### Running the Build Script

The PowerShell script can be run in several ways:

1. **Right-click method**: Right-click `Build-Skua.ps1` → "Run with PowerShell"
2. **Command line**: Open PowerShell and run `.\Build-Skua.ps1`
3. **Create a shortcut**: Make a shortcut with target:
   ```
   powershell.exe -ExecutionPolicy Bypass -File "Build-Skua.ps1"
   ```

## Manual Building

### Using Visual Studio

1. Open `Skua.sln` in Visual Studio
2. Select configuration (Debug/Release) and platform (x64/x86)
3. Build → Build Solution (Ctrl+Shift+B)

### Using .NET CLI

```bash
# Restore packages
dotnet restore

# Build x64 Release
dotnet build --configuration Release -p:Platform=x64

# Build x86 Release
dotnet build --configuration Release -p:Platform=x86

# Build specific project
dotnet build Skua.App.WPF\Skua.App.WPF.csproj --configuration Release
```

### Building the Installer

Requires WiX Toolset and MSBuild:

```bash
# Using MSBuild directly
msbuild Skua.Installer\Skua.Installer.wixproj /p:Configuration=Release /p:Platform=x64

# Or find MSBuild path first
"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" ^
  Skua.Installer\Skua.Installer.wixproj ^
  /p:Configuration=Release ^
  /p:Platform=x64
```

## CI/CD

### GitHub Actions

The project includes a GitHub Actions workflow (`.github/workflows/build.yml`) that automatically:

1. Builds on every push to main/master/develop branches
2. Builds on pull requests
3. Creates releases when tags starting with 'v' are pushed
4. Supports manual workflow dispatch

#### Triggering a Release

```bash
# Tag a release
git tag v1.0.0
git push origin v1.0.0

# This triggers:
# 1. Build for all platforms
# 2. Create installers
# 3. Generate release artifacts
# 4. Create GitHub release with downloads
```

#### GitHub Secrets (Optional)

For signed installers, add these repository secrets:
- `SIGNING_CERTIFICATE`: Base64-encoded PFX certificate
- `SIGNING_PASSWORD`: Certificate password

### Local CI Testing

Test the build process locally before pushing:

```powershell
# Full build test
.\Build-Skua.ps1 -Configuration Release -Clean:$true

# Debug build test
.\Build-Skua.ps1 -Configuration Debug -Platforms @("x64")
```

## Troubleshooting

### Common Issues

#### WiX Toolset Not Found
- **Error**: "The WiX Toolset v3.11 (or newer) build tools must be installed"
- **Solution**: Install WiX v6.0.2 from https://wixtoolset.org/releases/
- **Note**: WiX v4 is not compatible

#### MSBuild Not Found
- **Error**: "MSBuild not found"
- **Solution**: Install Visual Studio or Build Tools for Visual Studio
- Alternative: Use Developer Command Prompt

#### NuGet Restore Failures
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore with verbose output
dotnet restore --verbosity detailed
```

#### Platform Build Issues
```bash
# Ensure platform is specified correctly
dotnet build -p:Platform=x64  # Not "--platform x64"

# For x86, may need explicit target
dotnet build -p:Platform=x86 -p:PlatformTarget=x86
```

#### Permission Errors
- Run PowerShell as Administrator if needed
- Check execution policy: `Get-ExecutionPolicy`
- Temporarily bypass: `powershell -ExecutionPolicy Bypass -File Build-Skua.ps1`

### Build Performance

Speed up builds with these tips:

1. **Incremental builds**: Use `-Clean:$false` when testing
2. **Single platform**: Build only what you need
3. **Skip installer**: Use `-BuildInstaller:$false` during development
4. **Parallel builds**: MSBuild uses parallel builds by default

### Validation

Verify your build:

```powershell
# Check output files exist
Get-ChildItem -Path build -Recurse -Include *.exe, *.dll, *.msi

# Test the application
.\build\Release\x64\Skua.App.WPF\Skua.exe

# Verify installer
msiexec /i "build\Installers\Skua_Release_x64_Skua.Installer.msi" /quiet
```

## Advanced Configuration

### Custom Build Configurations

Edit project files to add custom configurations:

```xml
<!-- In .csproj files -->
<PropertyGroup Condition="'$(Configuration)'=='Custom'">
  <DefineConstants>CUSTOM_BUILD</DefineConstants>
  <Optimize>true</Optimize>
</PropertyGroup>
```

### Build Version Management

Set version in project files:

```xml
<PropertyGroup>
  <Version>1.0.0</Version>
  <AssemblyVersion>1.0.0.0</AssemblyVersion>
  <FileVersion>1.0.0.0</FileVersion>
</PropertyGroup>
```

Or via command line:

```bash
dotnet build -p:Version=1.2.3
```

## Contributing

When contributing build system changes:

1. Test all platforms (x64, x86)
2. Test both Debug and Release configurations
3. Verify installer builds correctly
4. Update this documentation if needed
5. Test GitHub Actions workflow locally if possible

## Support

For build issues:
1. Check [Prerequisites](#prerequisites) are installed
2. Review [Troubleshooting](#troubleshooting) section
3. Check existing GitHub issues
4. Create a new issue with:
   - Build error messages
   - System information (Windows version, .NET version)
   - Steps to reproduce
