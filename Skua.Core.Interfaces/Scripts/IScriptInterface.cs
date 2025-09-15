using Skua.Core.Interfaces.Auras;
using Skua.Core.Interfaces.Services;
using Skua.Core.Models;

namespace Skua.Core.Interfaces;

public interface IScriptInterfaceManager
{
    void Initialize();

    Task StopTimerAsync();
}

public interface IScriptInterface
{
    /// <summary>
    /// Static instance of the current <see cref="IScriptInterface"/>.
    /// </summary>
    static IScriptInterface Instance { get; protected set; }

    /// <summary>
    /// Class for Flash operations.
    /// </summary>
    IFlashUtil Flash { get; }

    /// <summary>
    /// Class to manage the status of the current script.
    /// </summary>
    IScriptStatus Manager { get; }

    /// <summary>
    /// Class to manage the state of the Auto Attack/Hunt functions.
    /// </summary>
    IScriptAuto Auto { get; }

    /// <summary>
    /// Class to control consumable Boosts.
    /// </summary>
    IScriptBoost Boosts { get; }

    /// <summary>
    /// Stats of the current application runtime.
    /// </summary>
    IScriptBotStats Stats { get; }

    IScriptSelfAuras Self { get; }
    IScriptTargetAuras Target { get; }

    /// <summary>
    /// Service to monitor aura changes and receive events when auras are activated, deactivated, or stack values change.
    /// </summary>
    IAuraMonitorService AuraMonitor { get; }

    /// <summary>
    /// Helper for managing ultra boss mechanics including counter attacks and aura monitoring.
    /// </summary>
    IUltraBossHelper UltraBossHelper { get; }

    /// <summary>
    /// Class to control combat mechanics.
    /// </summary>
    IScriptCombat Combat { get; }

    /// <summary>
    /// Class with methods for killing monsters/players.
    /// </summary>
    IScriptKill Kill { get; }

    /// <summary>
    /// Class with methods for hunting (teleport while killing) monsters.
    /// </summary>
    IScriptHunt Hunt { get; }

    /// <summary>
    /// Class to manage drops.
    /// </summary>
    IScriptDrop Drops { get; }

    /// <summary>
    /// Class with events that trigger with in-game events.
    /// </summary>
    IScriptEvent Events { get; }

    /// <summary>
    /// Class to manage the reputation list.
    /// </summary>
    IScriptFaction Reputation { get; }

    /// <summary>
    /// Class to manage the House inventory.
    /// </summary>
    IScriptHouseInv House { get; }

    /// <summary>
    /// Class to manage the Player Inventory.
    /// </summary>
    IScriptInventory Inventory { get; }

    /// <summary>
    /// Class to manage the player Temporary Inventory.
    /// </summary>
    IScriptTempInv TempInv { get; }

    /// <summary>
    /// Class to manage the player Bank.
    /// </summary>
    IScriptBank Bank { get; }

    /// <summary>
    /// Class to help inventory management.
    /// </summary>
    IScriptInventoryHelper InvHelper { get; }

    /// <summary>
    /// Class with options that reflect the in-game Advanced Options.
    /// </summary>
    IScriptLite Lite { get; }

    /// <summary>
    /// Class with options to customize the runtime of the bot.
    /// </summary>
    IScriptOption Options { get; }

    /// <summary>
    /// Class with properties of the current map and methods to travel to/in them.
    /// </summary>
    IScriptMap Map { get; }

    /// <summary>
    /// Class to manage the Monsters in the current map.
    /// </summary>
    IScriptMonster Monsters { get; }

    /// <summary>
    /// Class with properties of the current player.
    /// </summary>
    IScriptPlayer Player { get; }

    /// <summary>
    /// Class to manage Quests.
    /// </summary>
    IScriptQuest Quests { get; }

    /// <summary>
    /// Class with methods to send messages/packets.
    /// </summary>
    IScriptSend Send { get; }

    /// <summary>
    /// Class with properties of the current shop and methods to load, buy and sell items.
    /// </summary>
    IScriptShop Shops { get; }

    /// <summary>
    /// Class to control how the bot will use skills.
    /// </summary>
    IScriptSkill Skills { get; }

    /// <summary>
    /// Class with methods to wait certain actions of the game.
    /// </summary>
    IScriptWait Wait { get; }

    /// <summary>
    /// Class with properties of servers and methods to connect to them.
    /// </summary>
    IScriptServers Servers { get; }

    /// <summary>
    /// Class to control handlers which run in specific timings.
    /// </summary>
    IScriptHandlers Handlers { get; }

    /// <summary>
    /// Class to connect to a proxy and get packets sent between client and server.
    /// </summary>
    ICaptureProxy GameProxy { get; }

    /// <summary>
    /// Options within the compiled script.
    /// </summary>
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