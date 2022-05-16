using Skua.Core.Models.Items;

namespace Skua.Core.Interfaces;

public interface IScriptBoost
{
    event Action? BoostsStarted;
    event Action? BoostsStopped;
    /// <summary>
    /// Whether the boost thread is enabled.
    /// </summary>
    bool Enabled { get; }
    /// <summary>
    /// The Class Boost ID to be used.
    /// </summary>
    int ClassBoostID { get; set; }
    /// <summary>
    /// The XP Boost ID to be used.
    /// </summary>
    int ExperienceBoostID { get; set; }
    /// <summary>
    /// The Gold Boost ID to be used.
    /// </summary>
    int GoldBoostID { get; set; }
    /// <summary>
    /// The REP Boost ID to be used.
    /// </summary>
    int ReputationBoostID { get; set; }
    /// <summary>
    /// Whether to use the Class Boost with defined <see cref="ClassBoostID">ID</see>
    /// </summary>
    bool UseClassBoost { get; set; }
    /// <summary>
    /// Whether to use the Experience Boost with defined <see cref="ExperienceBoostID">ID</see>
    /// </summary>
    bool UseExperienceBoost { get; set; }
    /// <summary>
    /// Whether to use the Gold Boost with defined <see cref="GoldBoostID">ID</see>
    /// </summary>
    bool UseGoldBoost { get; set; }
    /// <summary>
    /// Whether to use the Gold Boost with defined <see cref="ReputationBoostID">ID</see>
    /// </summary>
    bool UseReputationBoost { get; set; }
    /// <summary>
    /// Whether it is using any of the boost types.
    /// </summary>
    bool UsingBoosts => UseClassBoost || UseExperienceBoost || UseGoldBoost || UseReputationBoost;
    /// <summary>
    /// Return the ID of the first boost found in the player's inventory.
    /// </summary>
    /// <param name="boostType">Type of the boost to search for.</param>
    /// <returns>The ID of the boost.</returns>
    int GetBoostID(BoostType boostType, bool searchBank = true);
    /// <summary>
    /// Checks if the specified <paramref name="boost"/> is active.
    /// </summary>
    /// <param name="boost">Type of boost to check.</param>
    /// <returns><see langword="true"/> if the specified boost is active.</returns>
    bool IsBoostActive(BoostType boost);
    /// <summary>
    /// Get and set all four boost types to the first boost found in the inventory.
    /// </summary>
    void SetAllBoostsIDs()
    {
        SetClassBoostID();
        SetGoldBoostID();
        SetExperienceBoostID();
        SetReputationBoostID();
    }
    /// <summary>
    /// Get and set <see cref="ClassBoostID"/> to the first boost found in the inventory.
    /// </summary>
    void SetClassBoostID()
    {
        ClassBoostID = GetBoostID(BoostType.Class);
    }
    /// <summary>
    /// Get and set <see cref="ExperienceBoostID"/> to the first boost found in the player's inventory.
    /// </summary>
    void SetExperienceBoostID()
    {
        ExperienceBoostID = GetBoostID(BoostType.Experience);
    }
    /// <summary>
    /// Get and set <see cref="GoldBoostID"/> to the first boost found in the player's inventory.
    /// </summary>
    void SetGoldBoostID()
    {
        GoldBoostID = GetBoostID(BoostType.Gold);
    }
    /// <summary>
    /// Get and set <see cref="ReputationBoostID"/> to the first boost found in the player's inventory.
    /// </summary>
    void SetReputationBoostID()
    {
        ReputationBoostID = GetBoostID(BoostType.Reputation);
    }
    /// <summary>
    /// Start the boost thread.
    /// </summary>
    void Start();
    /// <summary>
    /// Stops the boost thread.
    /// </summary>
    void Stop();
    /// <summary>
    /// Uses the boost with specified <paramref name="id"/>.
    /// </summary>
    /// <param name="id">ID of the boost.</param>
    void UseBoost(int id);
}
