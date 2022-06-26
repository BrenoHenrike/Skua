using Skua.Core.Interfaces;
using Skua.Core.Models.Monsters;
using Skua.Core.Flash;

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
    private List<Monster> _mapMonsters;

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

    [MethodCallBinding("availableMonsters", ParseFromJson = true, Default = "new()")]
    private List<Monster> _currentAvailableMonsters() => new();
}
