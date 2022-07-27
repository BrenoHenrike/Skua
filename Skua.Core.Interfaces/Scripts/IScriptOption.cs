using System.Collections.Immutable;
using System.ComponentModel;
using Skua.Core.Models;

namespace Skua.Core.Interfaces;

public interface IScriptOption : INotifyPropertyChanged
{
    ImmutableDictionary<string, Func<object>> OptionDictionary { get; }
    /// <summary>
    /// Delay between relogin tries using <see cref="IScriptServers.EnsureRelogin"/>.
    /// </summary>
    int ReloginTryDelay { get; set; }
    /// <summary>
    /// Timeout 
    /// </summary>
    int LoginTimeout { get; set; }
    /// <summary>
    /// When enabled will pickup any AC item that drops, even when the drop should be rejected.
    /// </summary>
    bool AcceptACDrops { get; set; }
    /// <summary>
    /// When enabled will pickup any item that drops.
    /// </summary>
    bool AcceptAllDrops { get; set; }
    /// <summary>
    /// When enabled will reject all items that drops (if <see cref="AcceptACDrops" /> is <see langword="true"/> will first accept AC items and then reject).
    /// </summary>
    bool RejectAllDrops { get; set; }
    /// <summary>
    /// Determines whether all monsters in the MAP should be aggroed (provoked). They will all attack you at the same time.
    /// </summary>
    /// <remarks>Having this option enabled keeps you in combat at all times, sometimes making it impossible to turn in quests.</remarks>
    bool AggroAllMonsters { get; set; }
    /// <summary>
    /// Determines whether all monsters in the room should be aggroed (provoked). They will all attack you at the same time.
    /// </summary>
    /// <remarks>Having this option enabled could keep you in combat at all times, sometimes making it impossible to turn in quests.</remarks>
    bool AggroMonsters { get; set; }
    /// <summary>
    /// Setting this to true will make it use the skills even without target. Use with caution.
    /// </summary>
    bool AttackWithoutTarget { get; set; }
    /// <summary>
    /// Enables the auto-relogin feature. If enabled, when the player is logged out of the game, they will automatically be logged back in with the configured username and password, to the configured server.
    /// </summary>
    bool AutoRelogin { get; set; }
    /// <summary>
    /// Re-logs into any server that wasn't the last one. This ensures the re-log is successful.
    /// </summary>
    bool AutoReloginAny { get; set; }
    /// <summary>
    /// Whether it should try certain amount of times (<see cref="ReloginTries"/>) to relogin.
    /// </summary>
    bool RetryRelogin { get; set; }
    /// <summary>
    /// Sets a persistent, custom guild name (client side).
    /// </summary>
    string CustomGuild { get; set; }
    /// <summary>
    /// Sets a persistent, custom player name (client side).
    /// </summary>
    string CustomName { get; set; }
    /// <summary>
    /// Disables all collisions in the game.
    /// </summary>
    bool DisableCollisions { get; set; }
    /// <summary>
    /// Disables the death AD.
    /// </summary>
    bool DisableDeathAds { get; set; }
    /// <summary>
    /// Disables all player combat animations (improves framerate).
    /// </summary>
    bool DisableFX { get; set; }
    /// <summary>
    /// The bot will ensure the player is not in combat before attempting to turn in a quest.
    /// </summary>
    bool ExitCombatBeforeQuest { get; set; }
    /// <summary>
    /// Maximum tries for Ensure (like <see cref="IScriptInventory.EnsureToBank(int)"/>) methods.
    /// </summary>
    int MaximumTries { get; set; }
    /// <summary>
    /// Delay in milliseconds that the bot will take to perform some actions.
    /// </summary>
    int ActionDelay { get; set; }
    /// <summary>
    /// Sets the color of your guild name with HEX (0xFFFFFF)
    /// </summary>
    int GuildColor { get; set; }
    /// <summary>
    /// When enabled, all player avatars are hidden.
    /// </summary>
    bool HidePlayers { get; set; }
    /// <summary>
    /// How many kills hunt should wait for before picking up drops.
    /// </summary>
    int HuntBuffer { get; set; }
    /// <summary>
    /// The minimum time between jumping between rooms when hunting for enemies (in milliseconds). The default is 1000ms.
    /// </summary>
    int HuntDelay { get; set; }
    /// <summary>
    /// Enabling this option allows you to attack targets from any range (without moving).
    /// </summary>
    bool InfiniteRange { get; set; }
    /// <summary>
    /// Disables drawing the world to (somewhat) reduce lag and CPU usage.
    /// </summary>
    /// <remarks>It is much more effective to minimize the game to reduce CPU usage than to enable this option. For the lowest CPU usage, try both.</remarks>
    bool LagKiller { get; set; }
    /// <summary>
    /// The time in ms that the game is allowed to load before logging the user out (triggering a relogin if enabled).
    /// </summary>
    int LoadTimeout { get; set; }
    /// <summary>
    /// The server to relogin to.
    /// </summary>
    string? ReloginServer { get; set; }
    /// <summary>
    /// When enabled, this will cause all targeted monsters to teleport to you.
    /// </summary>
    bool Magnetise { get; set; }
    /// <summary>
    /// Sets the color of your name with HEX (0xFFFFFF)
    /// </summary>
    int NameColor { get; set; }
    /// <summary>
    /// Determines whether to join only private rooms or not.
    /// </summary>
    bool PrivateRooms { get; set; }
    /// <summary>
    /// A rest packet will be sent every second, causing the player to heal when not in combat.
    /// </summary>
    bool RestPackets { get; set; }
    /// <summary>
    /// When enabled, there will be a 1 minute 15 second delay before the player is re-logged in.
    /// </summary>
    bool SafeRelogin { get; set; }
    /// <summary>
    /// When safe timings are enabled, the bot will wait for any action called to be completed with a timeout of (generally) 5 seconds (i.e. picking a drop) before continuing execution. It is strongly recommended that this is turned on.
    /// </summary>
    /// <remarks>This option does not ensure actions are carried out successfully, as it is quite possible that the 5 second timeout is reached before an action is completed.</remarks>
    bool SafeTimings { get; set; }
    /// <summary>
    /// Changes game maximum FPS (frames per second)
    /// </summary>
    int SetFPS { get; set; }
    /// <summary>
    /// Whether to show the in-game FPS counter
    /// </summary>
    bool ShowFPS { get; set; }
    /// <summary>
    /// Determines whether cutsenes should be skipped.
    /// </summary>
    bool SkipCutscenes { get; set; }
    /// <summary>
    /// An option to constantly modify the player's walk speed (the ScriptManager's timer thread will update the ingame value).
    /// </summary>
    int WalkSpeed { get; set; }
    /// <summary>
    /// Delay in milliseconds that the bot will take to perform some actions.
    /// </summary>
    int PrivateNumber { get; set; }
    /// <summary>
    /// How many tries the bot will make to join a map.
    /// </summary>
    int JoinMapTries { get; set; }
    /// <summary>
    /// How many tries the bot will make when trying to accept/complete a quest.
    /// </summary>
    int QuestAcceptAndCompleteTries { get; set; }
    /// <summary>
    /// How many tries the bot will make when using <see cref="IScriptServers.EnsureRelogin(string)"/>
    /// </summary>
    int ReloginTries { get; set; }
    /// <summary>
    /// The priority mode for hunting.
    /// </summary>
    HuntPriorities HuntPriority { get; set; }
    /// <summary>
    /// Resets the options to the user defined defaults. If no user settings are found, uses the application default values.
    /// </summary>
    void Reset();
    /// <summary>
    /// Resets the options to the application default values.
    /// </summary>
    void ResetToDefault();
    /// <summary>
    /// Saves the current options to the user settings.
    /// </summary>
    void Save();
}