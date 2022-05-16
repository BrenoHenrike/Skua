using Newtonsoft.Json;

namespace Skua.Core.Models.Skills;

public class SkillInfo
{
    /// <summary>
    /// The ID of the skill.
    /// </summary>
    [JsonProperty("id")]
    public int ID { get; set; }
    /// <summary>
    /// The action ID of the skill.
    /// </summary>
    [JsonProperty("actID")]
    public int ActID { get; set; }
    /// <summary>
    /// The damage value of the skill.
    /// </summary>
    [JsonProperty("damage")]
    public double Damage { get; set; }
    /// <summary>
    /// The icon key of the skill.
    /// </summary>
    [JsonProperty("icon")]
    public string Icon { get; set; }
    /// <summary>
    /// Indicates if the skill is an auto attack.
    /// </summary>
    [JsonProperty("auto")]
    public bool Auto { get; set; }
    /// <summary>
    /// The name of the skill.
    /// </summary>
    [JsonProperty("nam")]
    public string Name { get; set; }
    /// <summary>
    /// The description of the skill.
    /// </summary>
    [JsonProperty("desc")]
    public string Description { get; set; }
    /// <summary>
    /// The range of the skill.
    /// </summary>
    [JsonProperty("range")]
    public int Range { get; set; }
    /// <summary>
    /// The MP consumed by using this skill.
    /// </summary>
    [JsonProperty("mp")]
    public int MP { get; set; }
    /// <summary>
    /// Indicates if the skill is OK (idk what this is).
    /// </summary>
    [JsonProperty("isOK")]
    public bool IsOk { get; set; }
    /// <summary>
    /// The animation list of this skill as a string.
    /// </summary>
    [JsonProperty("anim")]
    public string _Anim { get; set; }
    /// <summary>
    /// The animation list of this skill as an array.
    /// </summary>
    public string[] Animations => _Anim.Split(',');
    /// <summary>
    /// The type of the skill.
    /// </summary>
    [JsonProperty("typ")]
    public string Type { get; set; }
    /// <summary>
    /// The minimum number of targets this skill must be used on.
    /// </summary>
    [JsonProperty("tgtMin")]
    public int MinTargets { get; set; }
    /// <summary>
    /// The maximum number of targets this skill can be used on.
    /// </summary>
    [JsonProperty("tgtMax")]
    public int MaxTargets { get; set; }
    /// <summary>
    /// The cooldown time of this skill in ms.
    /// </summary>
    [JsonProperty("cd")]
    public int Cooldown { get; set; }
    /// <summary>
    /// Indicates if this skill is locked.
    /// </summary>
    [JsonProperty("lock")]
    public bool Locked { get; set; }
    /// <summary>
    /// The timestamp of when this skill was last used.
    /// </summary>
    [JsonProperty("ts")]
    public long LastUse { get; set; }

    public override string ToString()
    {
        return Name;
    }
}
