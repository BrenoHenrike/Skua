# Skua Aura System Documentation

## Table of Contents
1. [Overview](#overview)
2. [Core Models](#core-models)
3. [Interfaces](#interfaces)
4. [Helpers](#helpers)
5. [Usage Examples](#usage-examples)
6. [Best Practices](#best-practices)
7. [Migration Guide](#migration-guide)

## Overview

The Skua Aura System provides comprehensive aura detection, monitoring, and analysis capabilities for both player (self) and monster (target) auras. The system is designed to be flexible, allowing scripts to work with any aura names without hard-coded assumptions.

### Key Features
- **Unified Aura Model**: Single `Aura` class supports both legacy and monster aura JSON formats
- **Precise Timing**: Unix timestamp-based calculations for accurate duration tracking
- **Flexible Helpers**: No hard-coded aura names - all methods accept parameters
- **Event System**: Real-time aura change notifications
- **Backward Compatibility**: Works with existing scripts while providing new capabilities

---

## Core Models

### `Aura` Class
Location: `Skua.Core.Models.Auras.Aura`

The unified aura model that handles both player and monster auras with precise timing calculations.

#### Properties
```csharp
public class Aura
{
    // Core Properties
    public string Name { get; set; }           // Aura name
    public int Value { get; set; }             // Stack count/value (default: 1)
    public int Duration { get; set; }          // Duration in seconds
    public long Timestamp { get; set; }        // Unix timestamp when applied
    public string Icon { get; set; }           // Icon filename
    public string Category { get; set; }       // Aura category/type
    public bool IsDebuff { get; set; }         // Whether it's a debuff
    
    // Optional Properties
    public bool? Passive { get; set; }         // Is passive aura
    public string? PotionType { get; set; }    // Potion type if applicable
    public bool? IsNew { get; set; }           // Is new when activated
    public string? MsgOn { get; set; }         // Message on activation
    public string? AnimationOn { get; set; }   // Animation on activation
    public string? AnimationOff { get; set; }  // Animation on deactivation
    
    // Computed Properties (Read-only)
    public DateTime? TimeStamp { get; }        // Legacy DateTime timestamp
    public DateTime? ExpiresAt { get; }        // When aura expires
    public double RemainingTime { get; }       // Precise remaining time in seconds
    public bool IsActive { get; }              // Whether aura is still active
    
    // Methods
    public int GetSecondsRemaining()           // Remaining time (rounded up)
}
```

#### JSON Format Support
The `Aura` class supports multiple JSON property formats:

**Legacy Format:**
```json
{
  "name": "Vendetta",
  "value": 25,
  "duration": 30,
  "timeStamp": "2023-01-01T12:00:00Z",
  "icon": "vendetta.swf",
  "category": "Enhancement",
  "s": "false"
}
```

**Current Format:**
```json
{
  "nam": "Counter Attack",
  "val": 1,
  "dur": 5,
  "ts": 1672574400000,
  "icon": "counter.swf",
  "cat": "Defensive",
  "s": true
}
```

---

## Interfaces

### `IScriptSelfAuras` Interface
Location: `Skua.Core.Interfaces.Auras.IScriptSelfAuras`

Interface for accessing player auras.

```csharp
public interface IScriptSelfAuras
{
    List<Aura> Auras { get; }                                    // All active auras
    Aura? GetAura(string auraName);                             // Get specific aura
    bool HasActiveAura(string auraName);                        // Check if aura is active
    bool TryGetAura(string auraName, out Aura? aura);          // Safe aura retrieval
    int GetAuraValue(string auraName);                          // Get aura stack value
    bool HasAuraWithMinStacks(string auraName, int minStacks);  // Check minimum stacks
    int GetAuraSecondsRemaining(string auraName);               // Get remaining time
    bool HasAnyActiveAura(params string[] auraNames);           // Check any of multiple
    bool HasAllActiveAuras(params string[] auraNames);          // Check all of multiple
    int GetTotalAuraStacks(string auraNamePattern);             // Sum matching auras
}
```

### `IScriptTargetAuras` Interface
Location: `Skua.Core.Interfaces.Auras.IScriptTargetAuras`

Interface for accessing target/monster auras.

```csharp
public interface IScriptTargetAuras
{
    List<Aura> Auras { get; }                                    // All active auras
    Aura? GetAura(string auraName);                             // Get specific aura
    bool HasActiveAura(string auraName);                        // Check if aura is active
    bool TryGetAura(string auraName, out Aura? aura);          // Safe aura retrieval
    int GetAuraValue(string auraName);                          // Get aura stack value
    bool HasAuraWithMinStacks(string auraName, int minStacks);  // Check minimum stacks
    int GetAuraSecondsRemaining(string auraName);               // Get remaining time
    bool HasAnyActiveAura(params string[] auraNames);           // Check any of multiple
    bool HasAllActiveAuras(params string[] auraNames);          // Check all of multiple
    int GetTotalAuraStacks(string auraNamePattern);             // Sum matching auras
}
```

### Script Access
In scripts, access these through the bot interface:
```csharp
IScriptSelfAuras selfAuras = Bot.Self.Auras;      // Player auras
IScriptTargetAuras targetAuras = Bot.Target.Auras; // Target auras
```

---

## Helpers

### `UltraBossAuraHelper` Class
Location: `Skua.Core.Scripts.Helpers.UltraBossAuraHelper`

Flexible helper class for aura analysis and boss mechanics. **No hard-coded aura names** - all methods accept parameters.

### this probably doesn't work as it should for boss phases because I don't know how they work in aqw

#### Boss Phase Detection

```csharp
// Get phase from specific aura
public static int GetBossPhase(IScriptTargetAuras targetAuras, string phaseAuraName)

// Get phase from multiple possible auras (returns first match)
public static int GetBossPhase(IScriptTargetAuras targetAuras, params string[] phaseAuraNames)

// Check if in final phase
public static bool IsInFinalPhase(IScriptTargetAuras targetAuras, params string[] finalPhaseAuraNames)
```

#### Charge Mechanics Analysis

```csharp
// Analyze charge mechanics (e.g., Queen Iona)
public static (bool hasPositive, bool hasNegative, bool hasReversed) AnalyzeChargeMechanics(
    IScriptSelfAuras auras, 
    string positiveChargeName, 
    string negativeChargeName, 
    string? reversedSuffix = null)
```

#### Threshold Checking

```csharp
// Check if aura value meets threshold condition
public static bool CheckAuraThreshold(
    IScriptSelfAuras auras, 
    int threshold, 
    string comparison = ">=")  // "<", ">", "<=", ">=", "==", "!="
```

#### Pattern Matching

```csharp
// Get auras matching pattern (supports wildcards, case-insensitive)
public static List<Aura> GetAurasMatchingPattern(
    List<Aura> auras, 
    string namePattern)
```

#### Monster Analysis

```csharp
// Get monsters with specific aura (case-insensitive)
public static List<Monster> GetMonstersWithAura(
    IEnumerable<Monster> monsters, 
    string auraName)

// Get monsters with any of specified auras (case-insensitive)
public static List<Monster> GetMonstersWithAnyAura(
    IEnumerable<Monster> monsters, 
    params string[] auraNames)

// Get monsters with all specified auras (case-insensitive)
public static List<Monster> GetMonstersWithAllAuras(
    IEnumerable<Monster> monsters, 
    params string[] auraNames)

// Get summary of all auras on monsters
public static Dictionary<string, int> GetAuraSummary(IEnumerable<Monster> monsters)
```

---

## Usage Examples

### Basic Aura Checking

```csharp
public void BasicAuraUsage(IScriptInterface bot)
{
    var selfAuras = bot.Self.Auras;
    var targetAuras = bot.Target.Auras;
    
    // Check if player has specific aura
    if (selfAuras.HasActiveAura("Vendetta"))
    {
        int stacks = selfAuras.GetAuraValue("Vendetta");
        bot.Log($"Vendetta has {stacks} stacks");
    }
    
    // Check target defensive auras
    if (targetAuras.HasAnyActiveAura("Counter Attack", "Damage Immunity", "Invulnerable"))
    {
        bot.Log("Target is defended - stop attacking!");
    }
    
    // Get precise timing
    var aura = selfAuras.GetAura("Blessing");
    if (aura != null)
    {
        bot.Log($"Blessing expires in {aura.RemainingTime:F1} seconds");
    }
}
```

### Boss Phase Detection

```csharp
public void BossPhaseExample(IScriptInterface bot)
{
    var targetAuras = bot.Target.Auras;
    
    // Check specific phase aura
    int phase = UltraBossAuraHelper.GetBossPhase(targetAuras, "Boss Phase");
    
    // Check multiple possible phase auras
    int altPhase = UltraBossAuraHelper.GetBossPhase(targetAuras, 
        "Boss Phase", "Ultra Phase", "Enrage Phase", "Custom Phase");
    
    // Check for final phase indicators
    bool isFinal = UltraBossAuraHelper.IsInFinalPhase(targetAuras, 
        "Final Phase", "Enrage", "Death Mode");
    
    if (phase > 0)
    {
        bot.Log($"Boss is in phase {phase}");
        
        switch (phase)
        {
            case 1:
                bot.Log("Using phase 1 strategy");
                break;
            case 2:
                bot.Log("Phase 2 - boss is more aggressive");
                break;
            case 3:
                bot.Log("Final phase - maximum danger!");
                break;
        }
    }
}
```

### Class-Specific Logic

```csharp
public void ClassSpecificExample(IScriptInterface bot)
{
    var selfAuras = bot.Self.Auras;
    
    // Legion Revenant - Vendetta management
    if (UltraBossAuraHelper.CheckAuraThreshold(selfAuras, "Vendetta", 40, "<"))
    {
        if (!selfAuras.HasActiveAura("Invulnerable"))
        {
            bot.Log("Using Vendetta taunt - current stacks: " + selfAuras.GetAuraValue("Vendetta"));
            // Use skill 2 (Vendetta)
        }
    }
    
    // Chrono classes - Rounds management
    if (UltraBossAuraHelper.CheckAuraThreshold(selfAuras, "Rounds Empty", 1, "==") || 
        bot.Player.Mana < 15)
    {
        bot.Log("Reloading rounds");
        // Use reload skill
    }
    
    // Arcana Invoker - Complex conditions
    var conditions = new Dictionary<string, Func<int, bool>>
    {
        ["XX - Judgement"] = value => value == 1,
        ["End of the world"] = value => value >= 13,
        ["XXI - The World"] = value => value == 0,
        ["0 - The Fool"] = value => value == 0
    };
    
    if (UltraBossAuraHelper.ShouldUseSkill(selfAuras, conditions))
    {
        bot.Log("Conditions met for Arcana skill 1");
        // Use skill
    }
}
```

### Charge Mechanics (Queen Iona Example)

```csharp
public void ChargeMechanicsExample(IScriptInterface bot)
{
    var selfAuras = bot.Self.Auras;
    
    // Analyze charge mechanics
    var charges = UltraBossAuraHelper.AnalyzeChargeMechanics(
        selfAuras, 
        "Positive Charge", 
        "Negative Charge", 
        "?");  // Reversed suffix
    
    if (charges.hasPositive)
    {
        bot.Log("Player has positive charge");
    }
    
    if (charges.hasNegative)
    {
        bot.Log("Player has negative charge");
    }
    
    if (charges.hasReversed)
    {
        bot.Log("Charges are reversed!");
    }
    
    // Use appropriate strategy based on charges
    if (charges.hasPositive && !charges.hasReversed)
    {
        // Move to negative area
    }
    else if (charges.hasNegative && !charges.hasReversed)
    {
        // Move to positive area
    }
}
```

### Monster Aura Analysis

```csharp
public void MonsterAuraExample(IScriptInterface bot)
{
    var monsters = bot.Monsters.MapMonstersWithCurrentData;
    
    // Find monsters with Counter Attack
    var defendedMonsters = UltraBossAuraHelper.GetMonstersWithAura(monsters, "Counter Attack");
    if (defendedMonsters.Any())
    {
        bot.Log($"Warning: {defendedMonsters.Count} monsters have Counter Attack");
        foreach (var monster in defendedMonsters)
        {
            bot.Log($"- {monster.Name} [{monster.Cell}]");
        }
    }
    
    // Find Ultra Bosses (monsters with "Ultra Boss" aura)
    var ultraBosses = UltraBossAuraHelper.GetMonstersWithAura(monsters, "Ultra Boss");
    
    // Find monsters with multiple defensive auras
    var heavilyDefended = UltraBossAuraHelper.GetMonstersWithAllAuras(monsters, 
        "Counter Attack", "Damage Immunity");
    
    // Get aura summary for the map
    var auraSummary = UltraBossAuraHelper.GetAuraSummary(monsters);
    bot.Log("Auras present on monsters:");
    foreach (var kvp in auraSummary.OrderByDescending(x => x.Value))
    {
        bot.Log($"- {kvp.Key}: {kvp.Value} monster(s)");
    }
}
```

### Pattern Matching

```csharp
public void PatternMatchingExample(IScriptInterface bot)
{
    var selfAuras = bot.Self.Auras;
    
    // Find all auras containing "Boost"
    var boostAuras = UltraBossAuraHelper.GetAurasMatchingPattern(
        selfAuras.Auras, "*Boost*");
    
    // Find auras starting with "Arcana"
    var arcanaAuras = UltraBossAuraHelper.GetAurasMatchingPattern(
        selfAuras.Auras, "Arcana*");
    
    // Case-sensitive matching
    var exactMatch = UltraBossAuraHelper.GetAurasMatchingPattern(
        selfAuras.Auras, "EXACT_NAME", false);
    
    bot.Log($"Found {boostAuras.Count} boost auras");
    foreach (var aura in boostAuras)
    {
        bot.Log($"- {aura.Name} ({aura.Value}) [{aura.RemainingTime:F1}s]");
    }
}
```

### Safe Aura Access

```csharp
public void SafeAuraAccess(IScriptInterface bot)
{
    var selfAuras = bot.Self.Auras;
    
    // Safe retrieval with TryGetAura
    if (selfAuras.TryGetAura("Blessing", out Aura? blessing))
    {
        bot.Log($"Blessing: {blessing.Value} stacks, {blessing.RemainingTime:F1}s remaining");
        
        // Access all properties safely
        bot.Log($"Category: {blessing.Category}");
        bot.Log($"Is Debuff: {blessing.IsDebuff}");
        bot.Log($"Icon: {blessing.Icon}");
        
        if (blessing.Passive.HasValue)
        {
            bot.Log($"Is Passive: {blessing.Passive.Value}");
        }
    }
    else
    {
        bot.Log("Blessing not found");
    }
    
    // Null-safe direct access
    var vendetta = selfAuras.GetAura("Vendetta");
    if (vendetta != null)
    {
        bot.Log($"Vendetta expires at: {vendetta.ExpiresAt}");
    }
}
```

---

## Best Practices

### 1. Always Check for Null
```csharp
var aura = bot.Self.Auras.GetAura("Vendetta");
if (aura != null)
{
    // Use aura safely
}

// Or use TryGetAura
if (bot.Self.Auras.TryGetAura("Vendetta", out var vendetta))
{
    // Use vendetta safely
}
```

### 2. Use Precise Timing for Critical Operations
```csharp
var aura = bot.Self.Auras.GetAura("Critical Timing");
if (aura != null && aura.RemainingTime > 1.5)
{
    // Safe to perform action with 1.5s buffer
}
```

### 3. Batch Aura Checks for Efficiency
```csharp
// Instead of multiple individual checks
bool hasDefense = bot.Target.Auras.HasAnyActiveAura(
    "Counter Attack", "Damage Immunity", "Invulnerable", "Shield");
```

### 4. Use Helper Methods for Complex Logic
```csharp
// Instead of hard-coding conditions
var conditions = new Dictionary<string, Func<int, bool>>
{
    ["Aura1"] = v => v >= 5,
    ["Aura2"] = v => v == 0,
    ["Aura3"] = v => v < 10
};
bool shouldAct = UltraBossAuraHelper.ShouldUseSkill(bot.Self.Auras, conditions);
```

### 5. Handle Case Sensitivity Appropriately
```csharp
// Most methods are case-insensitive by default, but be explicit when needed
var monsters = UltraBossAuraHelper.GetMonstersWithAura(allMonsters, "Ultra Boss");
```

---

## Migration Guide

### From Old Hard-coded System

**Old Way (Hard-coded):**
```csharp
// This no longer works - arrays were removed
int phase = UltraBossAuraHelper.GetBossPhase(targetAuras);  // ❌
```

**New Way (Flexible):**
```csharp
// Specify the aura names you want to check
int phase = UltraBossAuraHelper.GetBossPhase(targetAuras, "Boss Phase", "Ultra Phase");  // ✅
```

### From MonsterAura to Unified Aura

**Old Way:**
```csharp
// MonsterAura class no longer exists
MonsterAura aura = monster.GetMonsterAura("Counter Attack");  // ❌
```

**New Way:**
```csharp
// Use the unified Aura class
Aura aura = monster.GetAura("Counter Attack");  // ✅
// OR
if (monster.HasAura("Counter Attack"))  // ✅
{
    // Handle defended monster
}
```

### Legacy Method Support
Most legacy methods are still supported for backward compatibility:
```csharp
// Both work the same way
int remaining1 = aura.GetSecondsRemaining();  // New preferred method
```

---

## Performance Considerations

1. **Cache Aura Lists**: If checking multiple auras frequently, cache the aura list:
   ```csharp
   var auras = bot.Self.Auras.Auras;  // Get once, use multiple times
   ```

2. **Use Batch Methods**: Prefer `HasAnyActiveAura()` over multiple `HasActiveAura()` calls

3. **Pattern Matching**: Use sparingly for performance-critical code

4. **Event-Driven**: Use aura events instead of polling when possible

---

## Troubleshooting

### Common Issues

1. **Aura Not Found**: Check spelling and case sensitivity
2. **Timing Issues**: Use `RemainingTime` for precise calculations instead of `GetSecondsRemaining()`
3. **Monster Auras**: Ensure monsters have current data loaded
4. **Event Timing**: Aura events may fire before/after other game events

### Debugging Tips

```csharp
// Log all active auras
var auras = bot.Self.Auras.Auras;
bot.Log($"Active auras: {string.Join(", ", auras.Select(a => $"{a.Name}({a.Value})"))}");

// Log monster auras
foreach (var monster in bot.Monsters.CurrentAvailableMonsters)
{
    if (monster.Auras != null && monster.Auras.Any())
    {
        bot.Log($"{monster.Name}: {string.Join(", ", monster.Auras.Select(a => a.Name))}");
    }
}
```

---

This documentation covers all the available aura-related functionality that scripts can use. The system is designed to be flexible and extensible, allowing scripts to work with any aura names without requiring code changes to the core system.
