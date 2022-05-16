using Skua.Core.Interfaces;
using Skua.Core.Models.Monsters;
using Skua.Core.PostSharp;

namespace Skua.Core.Scripts;

public class ScriptMonster : ScriptableObject, IScriptMonster
{
    [ObjectBinding("world.monsters", Select = "objData")]
    public List<Monster> MapMonsters { get; } = new();
    public List<Monster> CurrentMonsters => MapMonsters.FindAll(m => m.Cell == Bot.Player.Cell);

    public Dictionary<string, List<Monster>> GetCellMonsters()
    {
        Dictionary<string, List<Monster>> monsters = new();
        foreach (string cell in Bot.Map.Cells)
            monsters[cell] = ((IScriptMonster)this).GetMonstersByCell(cell);
        return monsters;
    }

    [MethodCallBinding("availableMonsters", ForceJSON = true)]
    public List<Monster> CurrentAvailableMonsters() => new();
}
