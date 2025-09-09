using Newtonsoft.Json;
using Skua.Core.Models.Items;

namespace Skua.Core.Models.Quests;

public class SimpleReward : ItemBase
{
    /// <summary>
    /// The rate at which this reward drops.
    /// </summary>
    [JsonProperty("iRate")]
    public double Rate { get; set; }

    /// <summary>
    /// The type of the item as an integer ID.
    /// </summary>
    [JsonProperty("iType")]
    public int Type { get; set; }
}