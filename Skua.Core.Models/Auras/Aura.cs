using Newtonsoft.Json;
using Skua.Core.Utils.CustomJsonConverters;

namespace Skua.Core.Models.Auras;

public class Aura
{
    /// <summary>
    /// The aura's stack value/count.
    /// </summary>
    [JsonProperty("value")]
    public object? Value { get; set; }

    /// <summary>
    /// The icon file name for the aura.
    /// </summary>
    [JsonProperty("icon")]
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// The name of the aura.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The type of the aura.
    /// </summary>
    [JsonProperty("t")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The duration of the aura in seconds.
    /// </summary>
    [JsonProperty("duration")]
    public int Duration { get; set; }

    /// <summary>
    /// Whether this is a new aura.
    /// </summary>
    [JsonProperty("isNew")]
    public bool IsNew { get; set; }

    /// <summary>
    /// The timestamp when the aura was applied - Unix timestamp in milliseconds.
    /// </summary>
    [JsonProperty("timeStamp")]
    public long _timeStamp { get; set; }

    /// <summary>
    /// Internal flag to track if this aura should reset its timestamp.
    /// </summary>
    [JsonIgnore]
    private static readonly Dictionary<string, (int stacks, long timestamp)> _auraCache = new();

    /// <summary>
    /// If the aura is a passive or not.
    /// </summary>
    [JsonProperty("passive")]
    public bool? Passive { get; set; }

    /// <summary>
    /// The potion type of an aura if it's a potion.
    /// </summary>
    [JsonProperty("potionType")]
    public string? PotionType { get; set; }

    /// <summary>
    /// DateTime timestamp (computed from Unix timestamp).
    /// </summary>
    [JsonIgnore]
    public DateTime? TimeStamp => _timeStamp > 0 ? DateTimeOffset.FromUnixTimeMilliseconds(_timeStamp).DateTime : null;

    /// <summary>
    /// Gets the remaining time of the aura.
    /// </summary>
    public TimeSpan RemainingTime => ExpiresAt.HasValue ? ExpiresAt.Value - DateTimeOffset.Now : TimeSpan.Zero;

    /// <summary>
    /// The debuff type of aura. eg. stun, stone, disable or etc.
    /// </summary>
    [JsonProperty("cat")]
    public string? Category { get; set; }

    [JsonProperty("fx")]
    public string? Fx { get; set; }

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

    public int SecondsRemaining()
        => (this == null || ExpiresAt == null) ? 0 : (int)(((DateTime)ExpiresAt) - DateTime.Now).TotalSeconds;

    /// <summary>
    /// The expiration time of the aura.
    /// </summary>
    [JsonIgnore]
    public DateTime? ExpiresAt => TimeStamp?.AddSeconds(Duration);
}
