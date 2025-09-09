using System.ComponentModel;

namespace Skua.Core.Interfaces;

public interface IScriptBotStats : INotifyPropertyChanged
{
    /// <summary>
    /// The number of times the player has died.
    /// </summary>
    int Deaths { get; set; }

    /// <summary>
    /// The number of drops picked up.
    /// </summary>
    int Drops { get; set; }

    /// <summary>
    /// The number of monsters killed by the bot.
    /// </summary>
    int Kills { get; set; }

    /// <summary>
    /// The number of quests accepted (not unique).
    /// </summary>
    int QuestsAccepted { get; set; }

    /// <summary>
    /// The number of quests completed and turned in (not unique).
    /// </summary>
    int QuestsCompleted { get; set; }

    /// <summary>
    /// The number of times the player has been relogged in.
    /// </summary>
    int Relogins { get; set; }

    /// <summary>
    /// Resets all values.
    /// </summary>
    void Reset();
}