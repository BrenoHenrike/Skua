using Skua.Core.Models.Items;
using Skua.Core.Models.Players;
using Skua.Core.Utils;

namespace Skua.Core.Interfaces;

public interface IScriptMap
{
    /// <summary>
    /// The name of the last map joined in this session.
    /// </summary>
    string LastMap { get; set; }
    /// <summary>
    /// The file path to the last loaded map SWF.
    /// </summary>
    string FilePath { get; set; }
    /// <summary>
    /// The name of the map SWF file.
    /// </summary>
    string FileName { get; }
    /// <summary>
    /// Gets the name of the currently loaded map.
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Gets the name and number of the current map.
    /// </summary>
    string FullName { get; }
    /// <summary>
    /// Determines whether a map is currently loaded completely.
    /// </summary>
    bool Loaded { get; }
    /// <summary>
    /// Gets the current room's area ID.
    /// </summary>
    int RoomID { get; }
    /// <summary>
    /// Gets the number of players in the currently loaded map.
    /// </summary>
    int PlayerCount { get; }
    /// <summary>
    /// Gets a list of all of the cells in the current map.
    /// </summary>
    List<string> Cells { get; }
    /// <summary>
    /// Gets a list of player names in the currently loaded map.
    /// </summary>
    List<string>? PlayerNames { get; }
    /// <summary>
    /// Gets a list of all players in the current map.
    /// </summary>
    List<PlayerInfo>? Players { get; }
    /// <summary>
    /// Gets a list of all players in the current cell.
    /// </summary>
    List<PlayerInfo>? CellPlayers { get; }
    /// <summary>
    /// Reloads the current map.
    /// </summary>
    void Reload();
    /// <summary>
    /// Joins the specified <paramref name="map"/>.
    /// </summary>
    /// <param name="map">Name of the map.</param>
    /// <param name="cell">Cell to be placed at.</param>
    /// <param name="pad">Pad to be placed at.</param>
    /// <param name="ignoreCheck">Whether to ignore if the player is in the specified map.</param>
    void Join(string map, string cell = "Enter", string pad = "Spawn", bool ignoreCheck = false);
    /// <summary>
    /// Joins the specified <paramref name="map"/>, ignoring whether or not you are in that map.
    /// </summary>
    /// <param name="map">Name of the map.</param>
    void JoinIgnore(string map)
    {
        Join(map, "Enter", "Spawn", true);
    }
    /// <summary>
    /// Sends a join packet to the server.
    /// </summary>
    /// <param name="map">Name of the map to join.</param>
    /// <param name="cell">Cell to be placed at.</param>
    /// <param name="pad">Pad to be placed at.</param>
    void JoinPacket(string map, string cell = "Enter", string pad = "Spawn");
    /// <summary>
    /// Jumps the player to the specified <paramref name="cell"/> and <paramref name="pad"/>.
    /// </summary>
    /// <param name="cell">Cell to jump to.</param>
    /// <param name="pad">Pad to jump to.</param>
    /// <param name="clientOnly">If <see langword="true"/>, the client will not send a moveToCell packet to the server.</param>
    void Jump(string cell, string pad, bool clientOnly = false);
    /// <summary>
    /// Sends a getMapItem packet for the specified item <paramref name="id"/>.
    /// </summary>
    /// <param name="id">ID of the item</param>
    void GetMapItem(int id);
    /// <summary>
    /// Sends a getMapItem packet for the specified item <paramref name="id"/> in the desired <paramref name="quantity"/>.
    /// </summary>
    /// <param name="id">ID of the item</param>
    /// <param name="quantity">Desired quantity.</param>
    void GetMapItem(int id, int quantity)
    {
        for (int i = 0; i < quantity; i++)
            GetMapItem(id);
    }
    /// <summary>
    /// Checks if the specified player exists in the current room.
    /// </summary>
    /// <param name="name">Name of the player.</param>
    /// <returns><see langword="true"/> if the player with the specified <paramref name="name"/> is in the current room.</returns>
    bool PlayerExists(string name)
    {
        return PlayerNames.Contains(x => x == name);
    }
    /// <summary>
    /// Attempts to get the player by the given <paramref name="username"/> and sets the out parameter to its value.
    /// </summary>
    /// <param name="username">Name of the player to get.</param>
    /// <param name="player">The player object to set.</param>
    /// <returns><see langword="true"/> if the player with the given <paramref name="username"/> exists in the current map.</returns>
    bool TryGetPlayer(string username, out PlayerInfo? player)
    {
        return (player = GetPlayer(username)) is not null;
    }
    /// <summary>
    /// Gets info about the player with the given <paramref name="username"/>.
    /// </summary>
    /// <param name="username">Username of the player.</param>
    /// <returns>An <see cref="PlayerInfo"/> object of the given player.</returns>
    PlayerInfo? GetPlayer(string username);
    /// <summary>
    /// Search for map items in the current map.
    /// </summary>
    /// <returns>A list of the current map items.</returns>
    /// <remarks>Returns <see langword="null"/> if <see cref="MapFilePath"/> or the file of the map isn't found.</remarks>
    List<MapItem>? FindMapItems();
}
