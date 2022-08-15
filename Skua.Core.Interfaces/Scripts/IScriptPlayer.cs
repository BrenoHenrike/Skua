using System.Drawing;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Players;
using Skua.Core.Models.Skills;

namespace Skua.Core.Interfaces;

public interface IScriptPlayer
{
    string Guild { get; }
    /// <summary>
    /// Gets the player ID (useful for some packets).
    /// </summary>
    int ID { get; }
    /// <summary>
    /// Gets the player's current XP.
    /// </summary>
    int XP { get; }
    /// <summary>
    /// Gets the player's required XP to level up.
    /// </summary>
    int RequiredXP { get; }
    /// <summary>
    /// The player's access level.
    /// </summary>
    int AccessLevel { get; set; }
    /// <summary>
    /// The current cell that the player is in.
    /// </summary>
    string Cell { get; }
    /// <summary>
    /// The current pad that the player spawned from.
    /// </summary>
    string Pad { get; }
    /// <summary>
    /// The server to which the player is currently connected.
    /// </summary>
    string ServerIP { get; }
    /// <summary>
    /// Gets the player's state. 0 = Dead; 1 = Idle; 2 = In Combat.
    /// </summary>
    int State { get; }
    /// <summary>
    /// Checks whether the player is in combat.
    /// </summary>
    bool InCombat => State == 2;
    /// <summary>
    /// Checks whether the player is alive.
    /// </summary>
    bool Alive => State > 0;
    /// <summary>
    /// Checks whether the player is logged in.
    /// </summary>
    bool LoggedIn { get; }
    /// <summary>
    /// Checks whether the player is both logged in and alive.
    /// </summary>
    bool Playing { get; }
    /// <summary>
    /// Gets the player's username.
    /// </summary>
    string Username { get; }
    /// <summary>
    /// Gets the player's password.
    /// </summary>
    string Password { get; }
    /// <summary>
    /// Gets whether the player was kicked from the server.
    /// </summary>
    bool Kicked { get; }
    /// <summary>
    /// Checks if the player is a member (upgrade).
    /// </summary>
    bool IsMember { get; }
    /// <summary>
    /// Gets the player's current health.
    /// </summary>
    int Health { get; }
    /// <summary>
    /// Gets the player's maximum health.
    /// </summary>
    int MaxHealth { get; }
    /// <summary>
    /// Gets the player's current mana.
    /// </summary>
    int Mana { get; }
    /// <summary>
    /// Gets the player's maximum mana.
    /// </summary>
    int MaxMana { get; }
    /// <summary>
    /// Gets the player's level.
    /// </summary>
    int Level { get; }
    /// <summary>
    /// Gets the player's gold.
    /// </summary>
    int Gold { get; }
    /// <summary>
    /// A reference object of the player's current class.
    /// </summary>
    InventoryItem? CurrentClass { get; }
    /// <summary>
    /// Gets the player's current class rank.
    /// </summary>
    int CurrentClassRank { get; }
    /// <summary>
    /// The currently targeted monster. If no monster is targeted, null is returned.
    /// </summary>
    Monster? Target { get; }
    /// <summary>
    /// Checks if the player currently has a target selected.
    /// </summary>
    bool HasTarget { get; }
    /// <summary>
    /// Checks whether the player's avatar is loaded.
    /// </summary>
    bool Loaded { get; }
    /// <summary>
    /// Gets an array containing information about the player's current skills.
    /// </summary>
    SkillInfo[]? Skills { get; }
    /// <summary>
    /// The player stats.
    /// </summary>
    PlayerStats? Stats { get; }
    /// <summary>
    /// Checks whether the player is marked as AFK.
    /// </summary>
    bool AFK { get; }
    /// <summary>
    /// The player's current X coordinate.
    /// </summary>
    int X { get; }
    /// <summary>
    /// The player's current Y coordinate.
    /// </summary>
    int Y { get; }
    /// <summary>
    /// The current position of the player.
    /// </summary>
    Point Position => new(X, Y);
    /// <summary>
    /// This does nothing at the moment...
    /// </summary>
    int Scale { get; set; }
    /// <summary>
    /// Gets or sets the walking speed of the player. The default value is 8.
    /// </summary>
    int WalkSpeed { get; set; }

    /// <summary>
    /// Goes to the specified player (equivilent to using the /goto command).
    /// </summary>
    /// <param name="name">The name of the player.</param>
    void Goto(string name);
    /// <summary>
    /// Sets the player's respawn point to the current cell and pad.
    /// </summary>
    void SetSpawnPoint()
    {
        SetSpawnPoint(Cell, Pad);
    }
    /// <summary>
    /// Sets the player's respawn point to the given cell and pad.
    /// </summary>
    /// <param name="cell">Cell to be placed at.</param>
    /// <param name="pad">Pad to be placed at.</param>
    void SetSpawnPoint(string cell, string pad);
    /// <summary>
    /// Walks the player to the specified x and y coordinates.
    /// </summary>
    /// <param name="x">X coordinate value.</param>
    /// <param name="y">Y coordinate value.</param>
    /// <param name="speed">The speed at which to move the player's avatar.</param>
    void WalkTo(int x, int y, int speed = 8);
    /// <summary>
    /// Rests the player (equivilent to clicking the rest button on the UI).
    /// </summary>
    /// <param name="full">If <see langword="true"/>, the bot will wait until the player's HP and MP are full.</param>    
    void Rest(bool full = false);
}
