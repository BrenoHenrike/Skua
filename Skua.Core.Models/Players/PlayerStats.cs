using Newtonsoft.Json;

namespace Skua.Core.Models.Players;

public class PlayerStats
{
    [JsonProperty("$tha")]
    public float Haste { get; set; }

    [JsonProperty("$STR")]
    public int Strength { get; set; }

    [JsonProperty("$WIS")]
    public int Wisdom { get; set; }

    [JsonProperty("$DEX")]
    public int Dexterity { get; set; }

    [JsonProperty("$END")]
    public int Endurance { get; set; }

    [JsonProperty("$INT")]
    public int Intellect { get; set; }

    [JsonProperty("$LCK")]
    public int Luck { get; set; }

    [JsonProperty("$ap")]
    public int AttackPower { get; set; }

    [JsonProperty("$sp")]
    public int SpellPower { get; set; }

    [JsonProperty("$tcr")]
    public float CriticalChance { get; set; }

    [JsonProperty("$scm")]
    public float CriticalMultiplier { get; set; }

    [JsonProperty("$tdo")]
    public float EvasionChance { get; set; }
}