namespace Skua.Core.Interfaces;
public interface IScriptInterface
{
    /// <summary>
    /// Whether the world clip has been loaded yet.
    /// </summary>
    /// <remarks>This can be used as an additional way of checking if the player is logged in and ready to perform actions.</remarks>
    bool IsWorldLoaded => !Flash.IsNull("world");
    /// <summary>
    /// Whether the current script should terminate.
    /// </summary>
    bool ShouldExit { get; }
    IFlashUtil Flash { get; }
    IScriptBoost Boosts { get; }
    IScriptBotStats Stats { get; }
    IScriptCombat Combat { get; }
    IScriptDrop Drops { get; }
    IScriptEvent Events { get; }
    IScriptFaction Reputation { get; }
    IScriptHouseInv House { get; }
    IScriptInventory Inventory { get; }
    IScriptTempInv TempInv { get; }
    IScriptBank Bank { get; }
    IScriptLite Lite { get; }
    IScriptOption Options { get; }
    IScriptMap Map { get; }
    IScriptMonster Monsters { get; }
    IScriptPlayer Player { get; }
    IScriptQuest Quests { get; }
    IScriptSend Send { get; }
    IScriptShop Shops { get; }
    IScriptSkill Skills { get; }
    IScriptWait Wait { get; }
    IScriptServers Servers { get; }
    /// <summary>
    /// Initializes the <see cref="IScriptInterface"/> timer thread.
    /// </summary>
    void Initialize();
    /// <summary>
    /// Sleeps the bot for the specified time period.
    /// </summary>
    /// <param name="ms">Time in milliseconds for the bot to sleep.</param>
    void Sleep(int ms);
    /// <summary>
    /// Writes a message to the script logs.
    /// </summary>
    /// <param name="message">Message to log.</param>
    void Log(string message);
    /// <summary>
    /// Schedules the specified <paramref name="action"/> to run after the desired <paramref name="delay"/> in ms.
    /// </summary>
    /// <param name="delay">Time to wait before invoking the action.</param>
    /// <param name="action">Action to run. This can be passed as a lambda expression.</param>
    Task Schedule(int delay, Action<IScriptInterface> action);
    /// <summary>
    /// Schedules the specified <paramref name="action"/> to run after the desired <paramref name="delay"/> in ms.
    /// </summary>
    /// <param name="delay">Time to wait before invoking the action.</param>
    /// <param name="function">Action to run. This can be passed as a lambda expression.</param>
    Task Schedule(int delay, Func<IScriptInterface, Task> function);
}
