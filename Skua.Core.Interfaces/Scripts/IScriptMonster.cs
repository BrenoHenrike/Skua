using Skua.Core.Models.Monsters;

namespace Skua.Core.Interfaces;

public interface IScriptMonster
{
    /// <summary>
    /// A list of monsters in the current cell.
    /// </summary>
    List<Monster> CurrentMonsters { get; }
    /// <summary>
    /// A list of all monsters in the current map.
    /// </summary>
    List<Monster> MapMonsters { get; }
    /// <summary>
    /// A list of all monsters dataLeaf object in the current map.
    /// </summary>
    List<MonsterDataLeaf> MapMonstersDataLeaf { get; }
    /// <summary>
    /// A list of all monsters that the player can attack in the current cell.
    /// </summary>
    List<Monster> CurrentAvailableMonsters { get; }
    /// <summary>
    /// Checks whether the specified <paramref name="name"/> exists in the current cell.
    /// </summary>
    /// <param name="name">Name of the monster to check.</param>
    /// <returns><see langword="true"/> if the specified monster exists and is alive in the current cell.</returns>
    bool Exists(string name)
    {
        if (TryGetMonster(name, out Monster? monster) && monster != null)
            return MapMonstersDataLeaf.Any(m => name == "*" || (monster.Name.Trim() == name.Trim() && m.Alive));

        return false;
    }
    /// <summary>
    /// Checks whether the specified <paramref name="id"/> exists in the current cell.
    /// </summary>
    /// <param name="id">Name of the monster to check.</param>
    /// <returns><see langword="true"/> if the specified monster exists and is alive in the current cell.</returns>
    bool Exists(int id)
    {
        return MapMonstersDataLeaf.Any(m => m.ID == id && m.Alive);
    }
    /// <summary>
    /// Gets a dictionary which maps cell names of the current map to all monsters in that cell.
    /// </summary>
    Dictionary<string, List<Monster>> GetCellMonsters();
    /// <summary>
    /// Gets all of the cells with a living monster of the specified <paramref name="name"/>.
    /// </summary>
    List<string> GetLivingMonsterCells(string name)
    {
        return MapMonsters.Where(m => m.Alive && (name == "*" || m.Name.Trim() == name.Trim())).Select(m => m.Cell).Distinct().ToList();
    }
    /// <summary>
    /// Gets all of the cells with a living monster of the specified <paramref name="id"/>.
    /// </summary>
    List<string> GetLivingMonsterCells(int id)
    {
        return MapMonstersDataLeaf.Where(m => m.Alive && m.ID == id).Select(m => m.Cell).Distinct().ToList();
    }
    /// <summary>
    /// Gets all of the cells with a living monster of the spacified <paramref name="name"/>
    /// This uses the dataLeaf of the monster to prevent outdated data.
    /// </summary>
    List<string> GetLivingMonsterDataLeafCells(string name)
    {
        if (name == "*")
            return MapMonstersDataLeaf.Where(m => m.Alive).Select(m => m.Cell).Distinct().ToList();

        if (TryGetMonster(name, out Monster? monster) && monster != null)
            return MapMonstersDataLeaf.Where(m => m.Alive && m.ID == monster.ID).Select(m => m.Cell).Distinct().ToList();

        return new();
    }
    /// <summary>
    /// Gets all of the cells with a living monster of the spacified <paramref name="id"/>
    /// This uses the dataLeaf of the monster to prevent outdated data.
    /// </summary>
    List<string> GetLivingMonsterDataLeafCells(int id)
    {
        return MapMonstersDataLeaf.Where(m => m.Alive && m.ID == id).Select(m => m.Cell).Distinct().ToList();
    }
    /// <summary>
    /// Gets all of the cells with the desired monster in.
    /// </summary>
    /// <param name="name">Name of the monster to get.</param>
    List<string> GetMonsterCells(string name)
    {
        return MapMonsters.Where(m => m.Name.Trim() == name.Trim()).Select(m => m.Cell).Distinct().ToList();
    }
    /// <summary>
    /// Gets all of the cells with the desired monster in.
    /// </summary>
    /// <param name="id">ID of the monster to get.</param>
    List<string> GetMonsterCells(int id)
    {
        return MapMonsters.Where(m => m.ID == id).Select(m => m.Cell).Distinct().ToList();
    }
    /// <summary>
    /// Gets all of the monsters in the specified <paramref name="cell"/>.
    /// </summary>
    /// <param name="cell">Cell to get the monsters from.</param>
    List<Monster> GetMonstersByCell(string cell)
    {
        return MapMonsters.FindAll(x => x.Cell == cell);
    }
    /// <summary>
    /// Attempts to get the monster by the given <paramref name="name"/> and sets the out parameter to its value.
    /// </summary>
    /// <param name="name">Name of the monster to get.</param>
    /// <param name="monster">The monster object to set.</param>
    /// <returns><see langword="true"/> if the monster with the given <paramref name="name"/> exists in the current map.</returns>
    bool TryGetMonster(string name, out Monster? monster)
    {
        return (monster = MapMonsters.Find(m => name == "*" || m.Name.Trim() == name.Trim())) is not null;
    }
    /// <summary>
    /// Attempts to get the monster by the given <paramref name="id"/> and sets the out parameter to its value.
    /// </summary>
    /// <param name="id">Name of the monster to get.</param>
    /// <param name="monster">The monster object to set.</param>
    /// <returns><see langword="true"/> if the monster with the given <paramref name="id"/> exists in the current map.</returns>
    bool TryGetMonster(int id, out Monster? monster)
    {
        return (monster = MapMonsters.Find(m => m.ID == id)) is not null;
    }
}
