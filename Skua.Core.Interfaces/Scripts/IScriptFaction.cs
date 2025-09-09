using Skua.Core.Models.Factions;

namespace Skua.Core.Interfaces;

public interface IScriptFaction
{
    /// <summary>
    /// Get a list of all factions the player has atleast 1 point in Reputation.
    /// </summary>
    List<Faction> FactionList { get; }

    /// <summary>
    /// Get the rank of the faction with specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">Name of the faction to get.</param>
    /// <returns>The <see langword="int"/> value representing the current rank in the specified faction.</returns>
    int GetRank(string name)
    {
        return FactionList.FirstOrDefault(f => f.Name == name)?.Rank ?? 0;
    }

    /// <summary>
    /// Get the rank of the faction with specified <paramref name="id"/>.
    /// </summary>
    /// <param name="id">Name of the faction to get.</param>
    /// <returns>The <see langword="int"/> value representing the current rank in the specified faction.</returns>
    int GetRank(int id)
    {
        return FactionList.FirstOrDefault(f => f.ID == id)?.Rank ?? 0;
    }

    /// <summary>
    /// Checks if the player has the desired <paramref name="rank"/> for the specified faction <paramref name="name"/>.
    /// </summary>
    /// <param name="name">Name of the faction to check.</param>
    /// <param name="rank">Desired rank for that faction.</param>
    /// <returns><see langword="true"/> if the rank is equal or greater than the desired <paramref name="rank"/>.</returns>
    bool HasRank(string name, int rank = 1)
    {
        return GetRank(name) >= rank;
    }

    /// <summary>
    /// Checks if the player has the desired <paramref name="rank"/> for the specified faction <paramref name="id"/>.
    /// </summary>
    /// <param name="id">ID of the faction to check.</param>
    /// <param name="rank">Desired rank for that faction.</param>
    /// <returns><see langword="true"/> if the rank is equal or greater than the desired <paramref name="rank"/>.</returns>
    bool HasRank(int id, int rank = 1)
    {
        return GetRank(id) >= rank;
    }
}