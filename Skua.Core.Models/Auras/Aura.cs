using Newtonsoft.Json;
using Skua.Core.Utils.CustomJsonConverters;

namespace Skua.Core.Models.Auras;
public class Aura
{
    /// <summary>
    /// The name of the aura.
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; set; }
    
    /// <summary>
    /// The aura's stack value.
    /// </summary>
    [JsonProperty("value")]
    public string? Value { get; set; }

    /// <summary>
    /// If the aura is a passive or not.
    /// </summary>
    [JsonProperty("passive")]
    public bool? Passive { get; set; }

    /// <summary>
    /// The timestamp of when the aura was applied.
    /// </summary>
    [JsonProperty("timeStamp")]
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime? TimeStamp { get; set; }

    /// <summary>
    /// The duratio of the aura.
    /// </summary>
    [JsonProperty("duration")]
    public int? Duration { get; set; }

    /// <summary>
    /// The expiration time of the aura.
    /// </summary>
    public DateTime? ExpiresAt => ((DateTime)TimeStamp!).AddSeconds((double)Duration!);

    /// <summary>
    /// The potion type of an aura if it's a potion.
    /// </summary>
    [JsonProperty("potionType")]
    public string? PotionType { get; set; }

    /// <summary>
    /// The debuff type of aura. eg. stun, stone, disable or etc.
    /// </summary>
    [JsonProperty("cat")]
    public string? Cat { get; set; }

    [JsonProperty("t")]
    public string? T { get; set; }

    [JsonProperty("s")]
    public string? S { get; set; }

    [JsonProperty("fx")]
    public string? Fx { get; set; }

    /// <summary>
    /// On aura activate is it new?
    /// </summary>
    [JsonProperty("isNew")]
    public bool? IsNew { get; set; }

    [JsonProperty("msgOn")]
    public string? MsgOn { get; set; }

    [JsonProperty("animOn")]
    public string? AnimationOn { get; set; }

    [JsonProperty("animOff")]
    public string? AnimationOff { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
