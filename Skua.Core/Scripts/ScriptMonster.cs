using Newtonsoft.Json;
using Skua.Core.Flash;
using Skua.Core.Interfaces;
using Skua.Core.Models.Monsters;

namespace Skua.Core.Scripts;

public partial class ScriptMonster : IScriptMonster
{
    private readonly Lazy<IFlashUtil> _lazyFlash;
    private readonly Lazy<IScriptMap> _lazyMap;
    private readonly Lazy<IScriptPlayer> _lazyPlayer;

    private IFlashUtil Flash => _lazyFlash.Value;
    private IScriptMap Map => _lazyMap.Value;
    private IScriptPlayer Player => _lazyPlayer.Value;

    public ScriptMonster(
        Lazy<IFlashUtil> flash,
        Lazy<IScriptMap> map,
        Lazy<IScriptPlayer> player)
    {
        _lazyFlash = flash;
        _lazyMap = map;
        _lazyPlayer = player;
    }

    [ObjectBinding("world.monsters", Select = "objData", Default = "new()")]
    private List<Monster> _mapMonsters = new();

    [ObjectBinding("world.monsters", Select = "dataLeaf", Default = "new()")]
    private List<MonsterDataLeaf> _mapMonstersDataLeaf = new();

    public List<Monster> CurrentAvailableMonsters
    {
        get
        {
            string? monsters = Flash.Call("availableMonsters");
            return string.IsNullOrEmpty(monsters) ? new() : JsonConvert.DeserializeObject<List<Monster>>(monsters) ?? new();
        }
    }

    public int MonsterHP(int id)
    {
        string? hp = Flash.Call("getMonsterHealthById", id);
        return string.IsNullOrEmpty(hp) ? 0 : int.Parse(hp);
    }

    public int MonsterHP(string name)
    {
        string? hp = Flash.Call("getMonsterHealth", name);
        return string.IsNullOrEmpty(hp) ? 0 : int.Parse(hp);
    }


    public List<Monster> CurrentMonsters => MapMonsters?.FindAll(m => m.Cell == Player.Cell) ?? new();

    public List<Monster> MapMonstersWithCurrentData
    {
        get
        {
            try
            {
                var monsters = MapMonsters.ToList();
                var dataLeafDict = MapMonstersDataLeaf.ToDictionary(dl => dl.MapID, dl => dl);

                foreach (var monster in monsters)
                {
                    if (dataLeafDict.TryGetValue(monster.MapID, out var dataLeaf))
                    {
                        monster.HP = dataLeaf.HP;
                        monster.MaxHP = dataLeaf.MaxHP;
                        monster.State = dataLeaf.State;
                        monster.Auras = dataLeaf.Auras;
                    }
                }

                return monsters;
            }
            catch
            {
                return MapMonsters.ToList();
            }
        }
    }

    public Dictionary<string, List<Monster>> GetCellMonsters()
    {
        if (Map.Cells is null)
            return new();
        Dictionary<string, List<Monster>> monsters = new();
        foreach (string cell in Map.Cells)
            monsters[cell] = ((IScriptMonster)this).GetMonstersByCell(cell);
        return monsters;
    }

    /// <summary>
    /// Gets a summary of auras present on all monsters in the current map.
    /// </summary>
    public Dictionary<string, int> GetAuraSummary()
    {
        var auraSummary = new Dictionary<string, int>();

        foreach (var monster in MapMonstersWithCurrentData.Where(m => m.Auras?.Any() == true))
        {
            foreach (var aura in monster.Auras!)
            {
                if (auraSummary.ContainsKey(aura.Name))
                    auraSummary[aura.Name]++;
                else
                    auraSummary[aura.Name] = 1;
            }
        }

        return auraSummary;
    }
}