using Newtonsoft.Json;

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
    /// Indicates if this monster is alive.
    /// </summary>
    public bool Alive => HP > 0;

    public override string ToString()
    {
        return $"{Name} [{ID}] [{MapID}, {Cell}]";
    }
}
