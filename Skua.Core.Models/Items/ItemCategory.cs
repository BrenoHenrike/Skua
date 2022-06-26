using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Skua.Core.Models.Items;

[DataContract]
[JsonConverter(typeof(StringEnumConverter))]
public enum ItemCategory
{

    Unknown,
    Sword,
    Axe,
    Dagger,
    Gun,
    HandGun,
    Rifle,
    Bow,
    Mace,
    Gauntlet,
    Polearm,
    Staff,
    Wand,
    Whip,
    Class,
    Armor,
    Helm,
    Cape,
    Pet,
    Amulet,
    Necklace,
    Note,
    Resource,
    Item,
    Misc,
    [EnumMember(Value = "Quest Item")]
    QuestItem,
    [EnumMember(Value = "Server Use")]
    ServerUse,
    House,
    [EnumMember(Value = "Wall Item")]
    WallItem,
    [EnumMember(Value = "Floor Item")]
    FloorItem,
    Enhancement
}
