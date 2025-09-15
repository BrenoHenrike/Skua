# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Project Overview

Skua is a scriptable C# bot/client for the MMORPG AdventureQuest Worlds (AQW). It's the successor to RBot, built with .NET 6 and featuring a modular architecture with WPF applications for different use cases.

## Common Development Commands

### Building the Project
```powershell
# Build with automated script (recommended)
.\Build-Skua.ps1

# Build specific configuration
.\Build-Skua.ps1 -Configuration Debug
.\Build-Skua.ps1 -Configuration Release

# Build specific platforms
.\Build-Skua.ps1 -Platforms @("x64")
.\Build-Skua.ps1 -Platforms @("x86")
.\Build-Skua.ps1 -Platforms @("x64", "x86")

# Skip installer creation (faster for development)
.\Build-Skua.ps1 -BuildInstaller:$false

# Skip cleaning (incremental builds)
.\Build-Skua.ps1 -Clean:$false
```

### Manual Build Commands
```powershell
# Restore NuGet packages
dotnet restore

# Build for x64
dotnet build --configuration Release -p:Platform=x64

# Build for x86
dotnet build --configuration Release -p:Platform=x86

# Build specific project
dotnet build Skua.App.WPF\Skua.App.WPF.csproj --configuration Release
```

### Development Workflow
```powershell
# Clean build artifacts
if (Test-Path "build") { Remove-Item "build" -Recurse -Force }

# Development build (Debug, single platform, no installer)
.\Build-Skua.ps1 -Configuration Debug -Platforms @("x64") -BuildInstaller:$false -Clean:$false

# Run the main application
.\build\Debug\x64\Skua.App.WPF\Skua.exe

# Run the manager
.\build\Debug\x64\Skua.Manager.exe
```

## Architecture Overview

### Core Architecture Pattern
Skua follows a layered architecture with dependency injection:

- **Skua.Core.Interfaces**: Contract definitions for all services and components
- **Skua.Core.Models**: Data models and DTOs
- **Skua.Core.Utils**: Shared utilities and helpers  
- **Skua.Core**: Core business logic and script engine
- **Skua.WPF**: Shared WPF components and services
- **Application Projects**: Different WPF applications for various use cases

### Key Components

#### Script Engine (`ScriptInterface.cs`)
The heart of Skua - provides the main API that scripts interact with:
- Manages script lifecycle and execution
- Handles Flash integration for game communication
- Provides services for combat, inventory, quests, etc.
- Implements auto-relogin and error recovery

#### Flash Integration (`IFlashUtil`)
- Communicates with the AQW Flash game client
- Handles packet interception and injection  
- Manages game state synchronization
- Provides low-level game object access

#### Aura System
Comprehensive aura detection and monitoring:
- Unified `Aura` model for both player and monster auras
- Precise timing calculations using Unix timestamps
- Flexible helper methods without hard-coded aura names
- Real-time aura change notifications via `IAuraMonitorService`

### Application Variants
- **Skua.App.WPF**: Main full-featured application
- **Skua.Manager**: Lightweight script manager
- **Skua.App.WPF.Lite**: Minimal feature set
- **Skua.App.WPF.Follower**: Follower bot functionality
- **Skua.App.WPF.Sync**: Multi-client synchronization
- **Skua.SyncConsole**: Console version for sync

## Script Development

### Script Interface Access Pattern
Scripts inherit from a base class and access bot functionality through `Bot`:
```csharp
public class MyScript : IScriptInterface
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    
    public void ScriptMain(IScriptInterface bot)
    {
        // Player auras
        var selfAuras = bot.Self.Auras;
        if (selfAuras.HasActiveAura("Vendetta"))
        {
            // Logic here
        }
        
        // Target auras  
        var targetAuras = bot.Target.Auras;
        
        // Game actions
        bot.Combat.Attack("Monster Name");
        bot.Inventory.EquipItem("Item Name");
    }
}
```

### Aura System Usage
```csharp
// Check for specific aura
if (bot.Self.Auras.HasActiveAura("Vendetta"))
{
    int stacks = bot.Self.Auras.GetAuraValue("Vendetta");
}

// Check target defenses
if (bot.Target.Auras.HasAnyActiveAura("Counter Attack", "Damage Immunity"))
{
    // Stop attacking
}

// Boss phase detection
int phase = UltraBossAuraHelper.GetBossPhase(bot.Target.Auras, "Boss Phase");

// Precise timing
var aura = bot.Self.Auras.GetAura("Blessing");
if (aura != null && aura.RemainingTime > 1.5)
{
    // Safe to perform action
}
```

## Key Development Patterns

### Dependency Injection
All services use constructor injection with interfaces. Register services in the DI container during application startup.

### Event-Driven Architecture  
Use `IMessenger` for loosely coupled communication:
```csharp
// Send message
bot.Messenger.Send<ItemDroppedMessage, int>(new(item), (int)MessageChannels.GameEvents);

// Subscribe to messages
WeakReferenceMessenger.Default.Register<MyMessage>(this, HandleMessage);
```

### Flash Object Binding
Use `ObjectBinding` attribute for automatic Flash property synchronization:
```csharp
[ObjectBinding("player.intLevel")]
private int _level;

public int Level => _level;
```

### Memory Management
- All services implement `IDisposable` where needed
- Properly unsubscribe from events in `Dispose()`
- Use weak references for long-lived event subscriptions
- Clean up threads and cancellation tokens

## Build System Details

### Build Outputs
- `build/Release/x64/Skua.App.WPF/`: Main application (64-bit)
- `build/Release/x86/Skua.App.WPF/`: Main application (32-bit)  
- `build/Release/x64/Skua.Manager.exe`: Manager application
- `build/Installers/`: MSI installer files (if built)

### Platform Configuration
- Supports x64 and x86 architectures
- Main applications target `net6.0-windows`
- Uses platform-specific native assemblies in `Assemblies/` folder
- CoreHook dlls for both 32-bit and 64-bit injection

### WiX Installer
- Optional MSI installer creation using WiX CLI v6+
- Install WiX CLI: `dotnet tool install --global wix`
- Installer project: `Skua.Installer/Skua.Installer.wixproj`

## Important Implementation Details

### Thread Safety
Scripts run on dedicated "Script Thread". Use `CheckScriptTermination()` to respect cancellation tokens.

### Error Handling
- Scripts should handle `OperationCanceledException` for graceful shutdown
- Use `bot.ShouldExit` to check if script should terminate
- Log errors via `bot.Log(message)`

### Game State Management
- Always check `bot.Player.Playing` before game operations
- Use `bot.Wait.ForTrue(() => condition, timeout)` for polling
- Handle disconnections via auto-relogin system

### Performance Considerations
- Use `TimeLimiter` for rate-limiting expensive operations
- Cache aura lists when checking multiple auras frequently
- Prefer batch methods like `HasAnyActiveAura()` over individual calls

## Flash Game Integration

### Packet System
- Game communicates via XT packets
- Use `bot.Send.Packet(packetString)` for low-level commands
- Higher-level APIs wrap common packet operations

### Game Object Access
- Access Flash objects via `bot.Flash.GetGameObject(path)`
- Use `bot.Flash.Call(functionName, args)` for Flash method calls
- Check object existence with `bot.Flash.IsNull(path)`

## Multi-Client Support

### Synchronization
- Sync functionality allows coordinated multi-client operations
- Commands can be sent to all clients or specific client IDs
- Register custom sync commands via `IScriptSync` interface

### Account Management
- Local account storage (no database)
- Encrypted credential storage in appdata
- Multiple server/character configurations per account