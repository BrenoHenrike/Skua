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
        return CurrentAvailableMonsters.Any(m => name == "*" || (m.Name.Trim() == name.Trim()));
    }

    /// <summary>
    /// Checks whether the specified <paramref name="id"/> exists in the current cell.
    /// </summary>
    /// <param name="id">Name of the monster to check.</param>
    /// <returns><see langword="true"/> if the specified monster exists and is alive in the current cell.</returns>
    bool Exists(int id)
    {
        return CurrentAvailableMonsters.Any(m => m.ID == id || m.MapID == id);
    }

    /// <summary>
    /// Gets a dictionary which maps cell names of the current map to all monsters in that cell.
    /// </summary>
    Dictionary<string, List<Monster>> GetCellMonsters();

    /// <summary>
    /// Gets a summary of auras present on all monsters in the current map.
    /// </summary>
    Dictionary<string, int> GetAuraSummary();

    /// Gets all of the cells with a living monster of the specified <paramref name="name"/>.
    /// </summary>
    List<string> GetLivingMonsterCells(string name)
    {
        try
        {
            return MapMonsters.Where(m => m.Alive && (name == "*" || m.Name.Trim() == name.Trim())).Select(m => m.Cell).Distinct().ToList();
        }
        catch
        {
            return new();
        }
    }

    /// <summary>
    /// Gets all of the cells with a living monster of the specified <paramref name="id"/>.
    /// </summary>
    List<string> GetLivingMonsterCells(int id)
    {
        try
        {
            return MapMonsters.Where(m => m.Alive && m.ID == id).Select(m => m.Cell).Distinct().ToList();
        }
        catch { }
        return new();
    }

    /// <summary>
    /// Gets all of the cells with a living monster of the spacified <paramref name="name"/>
    /// This uses the dataLeaf of the monster to prevent outdated data.
    /// </summary>
    List<string> GetLivingMonsterDataLeafCells(string name)
    {
        if (name == "*")
            return MapMonsters.Where(m => m.Alive).Select(m => m.Cell).Distinct().ToList();

        if (TryGetMonster(name, out Monster? monster) && monster != null)
        {
            try
            {
                return MapMonsters.Where(m => m.Alive && m.ID == monster.ID).Select(m => m.Cell).Distinct().ToList();
            }
            catch { }
        }

        return new();
    }

    /// <summary>
    /// Gets all of the cells with a living monster of the spacified <paramref name="id"/>
    /// This uses the dataLeaf of the monster to prevent outdated data.
    /// </summary>
    List<string> GetLivingMonsterDataLeafCells(int id)
    {
        try
        {
            return MapMonsters.Where(m => m.Alive && m.ID == id).Select(m => m.Cell).Distinct().ToList();
        }
        catch { }
        return new();
    }

    /// <summary>
    /// Gets all of the cells with the desired monster in.
    /// </summary>
    /// <param name="name">Name of the monster to get.</param>
    List<string> GetMonsterCells(string name)
    {
        try
        {
            return MapMonsters.Where(m => m.Name.Trim() == name.Trim()).Select(m => m.Cell).Distinct().ToList();
        }
        catch { }
        return new();
    }

    /// <summary>
    /// Gets all of the cells with the desired monster in.
    /// </summary>
    /// <param name="id">ID of the monster to get.</param>
    List<string> GetMonsterCells(int id)
    {
        try
        {
            return MapMonsters.Where(m => m.ID == id).Select(m => m.Cell).Distinct().ToList();
        }
        catch { }
        return new();
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