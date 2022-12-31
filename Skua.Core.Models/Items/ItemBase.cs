using Newtonsoft.Json;
using Skua.Core.Models.Converters;

namespace Skua.Core.Models.Items;

public class ItemBase
{
    /// <summary>
    /// The ID of the item.
    /// </summary>
    [JsonProperty("ItemID")]
    public virtual int ID { get; set; }
    /// <summary>
    /// The name of the item.
    /// </summary>
    [JsonProperty("sName")]
    [JsonConverter(typeof(TrimConverter))]
    public virtual string Name { get; set; }
    /// <summary>
    /// The description of the item.
    /// </summary>
    [JsonProperty("sDesc")]
    public virtual string Description { get; set; }
    /// <summary>
    /// The quantity of the item in this stack.
    /// </summary>
    [JsonProperty("iQty")]
    public virtual int Quantity { get; set; }
    /// <summary>
    /// The maximum stack size this item can exist in.
    /// </summary>
    [JsonProperty("iStk")]
    public virtual int MaxStack { get; set; }
    /// <summary>
    /// Indicates if the item is a member/upgrade only item.
    /// </summary>
    [JsonProperty("bUpg")]
    [JsonConverter(typeof(StringBoolConverter))]
    public virtual bool Upgrade { get; set; }
    /// <summary>
    /// Indicates if the item is an AC item.
    /// </summary>
    [JsonProperty("bCoins")]
    [JsonConverter(typeof(StringBoolConverter))]
    public virtual bool Coins { get; set; }
    /// <summary>
    /// The category of the item.
    /// </summary>
    [JsonProperty("sType")]
    public virtual string CategoryString { get; set; }
    private ItemCategory? _category = null;
    public virtual ItemCategory Category
    {
        get
        {
            if (_category is not null)
                return (ItemCategory)_category;

            return (ItemCategory)(_category = Enum.TryParse(CategoryString, true, out ItemCategory result) ? result : ItemCategory.Unknown);
        }
    }
    /// <summary>
    /// Indicates if the item is a temporary item.
    /// </summary>
    [JsonProperty("bTemp")]
    [JsonConverter(typeof(StringBoolConverter))]
    public virtual bool Temp { get; set; }
    /// <summary>
    /// The group of the item. co = Armor; ba = Cape; he = Helm; pe = Pet; Weapon = Weapon.
    /// </summary>
    [JsonProperty("sES")]
    public virtual string ItemGroup { get; set; }

    public override string ToString()
    {
        var tag = string.Empty;
        var itemGroup = string.Empty;

        tag += Coins ? "AC " : string.Empty;
        tag += Upgrade ? "Member" : string.Empty;

        switch (ItemGroup)
        {
            case "co":
                itemGroup = "(Armor)";
                break;
            case "ba":
                itemGroup = "(Cape)";
                break;
            case "he":
                itemGroup = "(Helm)";
                break;
            case "pe":
                itemGroup = "(Pet)";
                break;
            case "Weapon":
                itemGroup = "(Weapon)";
                break;
            default:
                itemGroup = "(Item)";
                break;
        }

        return $"[{ID}]\t{itemGroup} {Name} x{Quantity} {tag}";
    }

    public override bool Equals(object? obj)
    {
        return obj is ItemBase item && ID == item.ID;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ID);
    }
}
