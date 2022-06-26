using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Skua.Core.Models.Items;

namespace Skua.Core.Models.Quests;
public class QuestData : IEqualityComparer<QuestData>
{
    /// <summary>
    /// The ID of the quest.
    /// </summary>
    [JsonProperty("ID")]
    public int ID { get; set; }

    /// <summary>
    /// The slot of the quest.
    /// </summary>
    [JsonProperty("Slot")]
    public int Slot { get; set; }

    /// <summary>
    /// The value of the quest.
    /// </summary>
    [JsonProperty("Value")]
    public int Value { get; set; }

    /// <summary>
    /// The name of the quest.
    /// </summary>
    [JsonProperty("Name")]
    public string Name { get; set; }

    /// <summary>
    /// Whether this quest can only be completed once.
    /// </summary>
    [JsonProperty("Once")]
    public bool Once { get; set; }

    /// <summary>
    /// The field of the quest.
    /// </summary>
    [JsonProperty("Field")]
    public string Field { get; set; }

    /// <summary>
    /// The index of the quest.
    /// </summary>
    [JsonProperty("Index")]
    public int Index { get; set; }

    /// <summary>
    /// Whether this quest requires upgrade.
    /// </summary>
    [JsonProperty("Upgrade")]
    public bool Upgrade { get; set; }

    /// <summary>
    /// The level required to accept the quest.
    /// </summary>
    [JsonProperty("Level")]
    public int Level { get; set; }

    /// <summary>
    /// The id of the class required to accept the quest.
    /// </summary>
    [JsonProperty("RequiredClassID")]
    public int RequiredClassID { get; set; }

    /// <summary>
    /// The class points required to accept the quest.
    /// </summary>
    [JsonProperty("RequiredClassPoints")]
    public int RequiredClassPoints { get; set; }

    /// <summary>
    /// The faction required to accept the quest.
    /// </summary>
    [JsonProperty("RequiredFactionId")]
    public int RequiredFactionId { get; set; }

    /// <summary>
    /// The required faction rep to accept the quest.
    /// </summary>
    [JsonProperty("RequiredFactionRep")]
    public int RequiredFactionRep { get; set; }

    /// <summary>
    /// The amount of gold this quest gives as a reward.
    /// </summary>
    [JsonProperty("Gold")]
    public int Gold { get; set; }

    /// <summary>
    /// The amount of XP this quest gives as a reward.
    /// </summary>
    [JsonProperty("XP")]
    public int XP { get; set; }

    /// <summary>
    /// The items required in the player's inventory to accept the quest.
    /// </summary>
    [JsonProperty("AcceptRequirements")]
    public List<ItemBase> AcceptRequirements { get; set; } = new();

    /// <summary>
    /// The items used to turn in the quest.
    /// </summary>
    [JsonProperty("Requirements")]
    public List<ItemBase> Requirements { get; set; } = new();

    /// <summary>
    /// The items given as a reward for completing the quest.
    /// </summary>
    [JsonProperty("Rewards")]
    public List<ItemBase> Rewards { get; set; } = new();

    /// <summary>
    /// Item drop rates are mapped to their IDs in this list.
    /// </summary>
    [JsonProperty("SimpleRewards")]
    public List<SimpleReward> SimpleRewards { get; set; } = new();

    public bool Equals(QuestData? x, QuestData? y)
    {
        return (x is null && y is null) || ((x is not null || y is null) && (x is null || y is not null) && x.ID == y.ID);
    }

    public override bool Equals(object? obj)
    {
        return obj is not null && obj is QuestData quest && quest.ID == ID;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(ID, Name);
    }

    public int GetHashCode([DisallowNull] QuestData obj)
    {
        return HashCode.Combine(obj.ID, obj.Name);
    }

    public override string ToString()
    {
        return $"[{ID}] {Name}{(Upgrade ? " (Member)" : string.Empty)}";
    }
}
