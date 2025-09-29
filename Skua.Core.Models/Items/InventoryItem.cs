using Newtonsoft.Json;
using Skua.Core.Models.Converters;

namespace Skua.Core.Models.Items;

public class InventoryItem : ItemBase
{
    /// <summary>
    /// The character (instance) ID of this item.
    /// </summary>
    [JsonProperty("CharItemID")]
    public int CharItemID { get; set; }

    /// <summary>
    /// Indicates if the item is equipped.
    /// </summary>
    [JsonProperty("bEquip")]
    [JsonConverter(typeof(StringBoolConverter))]
    public bool Equipped { get; set; }

    /// <summary>
    /// The level of the item.
    /// </summary>
    [JsonProperty("iLvl")]
    public int Level { get; set; }

    /// <summary>
    /// The enhancement level of the item.
    /// </summary>
    [JsonProperty("EnhLvl")]
    public virtual int EnhancementLevel { get; set; }

    /// <summary>
    /// The enhancement pattern ID of the item, this identifies the current  enhancement type of the item.
    /// <br> 1: Adventurer </br>
    /// <br> 2: Fighter </br>
    /// <br> 3: Thief </br>
    /// <br> 4: Armsman </br>
    /// <br> 5: Hybrid </br>
    /// <br> 6: Wizard </br>
    /// <br> 7: Healer </br>
    /// <br> 8: Spellbreaker </br>
    /// <br> 9: Lucky </br>
    /// </summary>
    [JsonProperty("EnhPatternID")]
    public int EnhancementPatternID { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is InventoryItem item && item.ID == ID && item.CharItemID == CharItemID;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ID, CharItemID);
    }
}