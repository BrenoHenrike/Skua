using Newtonsoft.Json;

namespace Skua.Core.Models.Factions;

public class Faction
{
    /// <summary>
    /// The ID of the faction.
    /// </summary>
    [JsonProperty("FactionID")]
    public int ID { get; set; }
    /// <summary>
    /// The in-game name of the faction.
    /// </summary>
    [JsonProperty("sName")]
    public string Name { get; set; }
    /// <summary>
    /// The rank that the player has achieved in this faction.
    /// </summary>
    [JsonProperty("iRank")]
    public int Rank { get; set; }
    /// <summary>
    /// The total amount of rep the player has for this faction.
    /// </summary>
    [JsonProperty("iRep")]
    public int TotalRep { get; set; }
    /// <summary>
    /// The amount of rep the player has for their current rank.
    /// </summary>
    [JsonProperty("iSpillRep")]
    public int Rep { get; set; }
    /// <summary>
    /// The total required rep for the player to rank up.
    /// </summary>
    [JsonProperty("iRepToRank")]
    public int RequiredRep { get; set; }
    /// <summary>
    /// The remaining amount of rep required for the player to rank up.
    /// </summary>
    public int RemainingRep => RequiredRep - Rep;
}
