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

    [JsonCallBinding("getMonsters", Default = "new()")]
    private List<Monster> _mapMonsters = new();

    [JsonCallBinding("availableMonsters", Default = "new()")]
    public List<Monster> _currentAvailableMonsters = new();

    public List<Monster> CurrentMonsters => MapMonsters?.FindAll(m => m.Cell == Player.Cell) ?? new();

    public Dictionary<string, List<Monster>> GetCellMonsters()
    {
        if (Map.Cells is null)
            return new();
        Dictionary<string, List<Monster>> monsters = new();
        foreach (string cell in Map.Cells)
            monsters[cell] = ((IScriptMonster)this).GetMonstersByCell(cell);
        return monsters;
    }

    public Dictionary<string, int> GetAuraSummary()
    {
        var auraSummary = new Dictionary<string, int>();

        foreach (var monster in MapMonsters.Where(m => m.Auras?.Any() == true))
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