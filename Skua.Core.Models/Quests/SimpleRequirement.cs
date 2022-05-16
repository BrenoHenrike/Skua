using Newtonsoft.Json;

namespace Skua.Core.Models.Quests;

public class SimpleRequirement
{
    [JsonProperty("ItemID")]
    public int ID { get; set; }
    [JsonProperty("iQty")]
    public int Quantity { get; set; }
}
