using Skua.Core.Models;

namespace Skua.Core.Interfaces;

public interface IScriptInterfaceManager
{
    void Initialize();
    Task StopTimerAsync();
}
public interface IScriptInterface
{
    static IScriptInterface Instance { get; protected set; }
    IFlashUtil Flash { get; }
    IScriptStatus Manager { get; }
    IScriptAuto Auto { get; }
    IScriptBoost Boosts { get; }
    IScriptBotStats Stats { get; }
    IScriptCombat Combat { get; }
    IScriptKill Kill { get; }
    IScriptHunt Hunt { get; }
    IScriptDrop Drops { get; }
    IScriptEvent Events { get; }
    IScriptFaction Reputation { get; }
    IScriptHouseInv House { get; }
    IScriptInventory Inventory { get; }
    IScriptTempInv TempInv { get; }
    IScriptBank Bank { get; }
    IScriptInventoryHelper InvHelper { get; }
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
    IScriptHandlers Handlers { get; }
    ICaptureProxy GameProxy { get; }
    IScriptOptionContainer? Config { get; }

    /// <summary>
    /// Whether the bot should stop.
    /// </summary>
    bool ShouldExit { get; }
    /// <summary>
    /// Current version of Skua.
    /// </summary>
    Version Version { get; }

    /// <summary>
    /// A random instance for the script.
    /// </summary>
    Random Random { get; set; }
    /// <summary>
    /// Sleeps the bot for the specified time period.
    /// </summary>
    /// <param name="ms">Time in milliseconds for the bot to sleep.</param>
    void Sleep(int ms);
    /// <summary>
    /// Stops the script
    /// </summary>
    /// <param name="runScriptStoppingEvent">Whether to fire the <see cref="IScriptEvent.ScriptStopping"/> event.</param>
    void Stop(bool runScriptStoppingEvent = true);
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
    /// <summary>
    /// Shows a message box.
    /// </summary>
    /// <param name="message">Message in the popup.</param>
    /// <param name="caption">Title of the popup.</param>
    /// <param name="yesAndNo">Whether it should have 'Yes' and 'No' buttons. If <see langword="false"/> will only have the 'Ok' button.</param>
    /// <returns><see langword="true"/> if the 'Yes' or 'Ok' button was clicked.</returns>
    bool? ShowMessageBox(string message, string caption, bool yesAndNo = false);
    /// <summary>
    /// Shows a message box with the specified buttons.
    /// </summary>
    /// <param name="message">Message in the popup.</param>
    /// <param name="caption">Title of the popup.</param>
    /// <param name="buttons">A list of buttons that will be shown.</param>
    /// <returns>A <see cref="DialogResult"/> object with the text and value of the button. The value is the index of the button in the passed array, meaning that -1 is no button found.</returns>
    DialogResult ShowMessageBox(string message, string caption, params string[] buttons);

}
