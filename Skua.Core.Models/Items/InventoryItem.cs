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
    /// The enhancement pattern ID of the item, this identifies the current  enhancement type of the item. </br>
    /// 1: Adventurer </br> 2: Fighter </br> 3: Thief </br> 4: Armsman </br> 5: Hybrid </br> 6: Wizard </br> 7: Healer </br> 8: Spellbreaker </br> 9: Lucky
    /// </summary>
    [JsonProperty("InvEnhPatternID")]
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