using Newtonsoft.Json;
using Skua.Core.Models.Auras;

namespace Skua.Core.Models.Monsters;

/// <summary>
/// Class that represents the object data of a Monster
/// </summary>
public class Monster
{
    /// <summary>
    /// The name of the monster.
    /// </summary>
    [JsonProperty("strMonName")]
    public string Name { get; set; }

    /// <summary>
    /// The global ID of the monster.
    /// </summary>
    [JsonProperty("MonID")]
    public int ID { get; set; }

    /// <summary>
    /// The race of the monster.
    /// </summary>
    [JsonProperty("sRace")]
    public string Race { get; set; }

    /// <summary>
    /// The cell the monster is in.
    /// </summary>
    [JsonProperty("strFrame")]
    public string Cell { get; set; }

    /// <summary>
    /// The map ID of the monster.
    /// </summary>
    [JsonProperty("MonMapID")]
    public int MapID { get; set; }

    /// <summary>
    /// The monster's current HP.
    /// </summary>
    [JsonProperty("intHP")]
    public int HP { get; set; }

    /// <summary>
    /// The monster's max HP.
    /// </summary>
    [JsonProperty("intHPMax")]
    public int MaxHP { get; set; }

    /// <summary>
    /// The state of the monster.
    /// </summary>
    [JsonProperty("intState")]
    public int State { get; set; }

    /// <summary>
    /// The SWF file name of the monster.
    /// </summary>
    [JsonProperty("strMonFileName")]
    public string FileName { get; set; }

    /// <summary>
    /// List of auras currently active on this monster.
    /// </summary>
    [JsonProperty("auras")]
    public List<Aura> Auras { get; set; }

    /// <summary>
    /// Indicates if this monster is alive.
    /// </summary>
    public bool Alive
    {
        get
        {
            return HP > 0 && State > 0;
        }
    }

    /// <summary>
    /// Checks if the monster has a specific aura active.
    /// </summary>
    /// <param name="auraName">Name of the aura to check for.</param>
    /// <returns>True if the monster has the specified aura active.</returns>
    public bool HasAura(string auraName)
    {
        return Auras.Any(a => a.Name.Equals(auraName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets a specific aura by name.
    /// </summary>
    /// <param name="auraName">Name of the aura to get.</param>
    /// <returns>The aura if found, or null if not found.</returns>
    public Aura? GetAura(string auraName)
    {
        return Auras.FirstOrDefault(a => a.Name.Equals(auraName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets the value/stacks of a specific aura.
    /// </summary>
    /// <param name="auraName">Name of the aura to get the value for.</param>
    /// <returns>The aura value/stacks, or 0 if not found.</returns>
    public int GetAuraValue(string auraName)
    {
        var aura = Auras.FirstOrDefault(a => a.Name.Equals(auraName, StringComparison.OrdinalIgnoreCase));
        return aura?.Value ?? 0;
    }

    /// <summary>
    /// Gets all active aura names as a comma-separated string.
    /// </summary>
    public string AuraNames => string.Join(", ", Auras.Select(a => a.Name));

    public override string ToString()
    {
        return $"{Name} [{ID}] [{MapID}, {Cell}]";
    }
}