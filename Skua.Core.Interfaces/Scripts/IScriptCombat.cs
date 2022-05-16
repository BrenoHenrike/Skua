using Skua.Core.Models;
using Skua.Core.Models.Monsters;

namespace Skua.Core.Interfaces;

public interface IScriptCombat
{
    /// <summary>
    /// Walks towards (approaches) the currently selected target.
    /// </summary>
    void ApproachTarget();
    /// <summary>
    /// Attacks the monster with the specified <paramref name="mapId">.
    /// </summary>
    /// <param name="mapId">Map ID of the monster to attack.</param>
    /// <remarks>This will not wait until the monster is killed, but simply select it and start attacking it.</remarks>
    void Attack(int mapId);
    /// <summary>
    /// Attacks the specified instance of a <paramref name="monster"/>.
    /// </summary>
    /// <param name="monster">Monster to attack.</param>
    /// <remarks>This will not wait until the monster is killed, but simply select it and start attacking it.</remarks>
    void Attack(Monster monster)
    {
        Attack(monster.MapID);
    }
    /// <summary>
    /// Attacks the monster with specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">Name of the monster to attack.</param>
    /// <remarks>This will not wait until the monster is killed, but simply select it and start attacking it.</remarks>
    void Attack(string name);
    /// <summary>
    /// Attacks the player with specified <paramref name="name"/>. If not in PVP mode, this will only target the player, and not attack them.
    /// </summary>
    /// <param name="name">Name of the player to attack.</param>
    /// <remarks>This will not wait until the player is killed, but simply select it.</remarks>
    void AttackPlayer(string name);
    /// <summary>
    /// Cancel the player's auto attack.
    /// </summary>
    void CancelAutoAttack();
    /// <summary>
    /// Deselects the currently selected target.
    /// </summary>
    void CancelTarget();
    /// <summary>
    /// Jumps to the current cell and waits for the player to exit combat.
    /// </summary>
    void Exit();
    /// <summary>
    /// Untargets the player if they are currently targeted.
    /// </summary>
    void UntargetSelf();
    void Hunt(string name);
    void HuntForItem(List<string> names, string item, int quantity, bool tempItem = false, bool rejectElse = true);
    void HuntForItem(string name, string item, int quantity, bool tempItem = false, bool rejectElse = true);
    void HuntForItem(string[] names, string item, int quantity, bool tempItem = false, bool rejectElse = true);
    void HuntForItems(List<string> names, string[] items, int[] quantities, bool tempItems = false, bool rejectElse = true);
    void HuntForItems(List<string> names, string[] items, int[] quantities, bool[] tempItems, bool rejectElse);
    void HuntForItems(string name, string[] items, int[] quantities, bool tempItems = false, bool rejectElse = true);
    void HuntForItems(string name, string[] items, int[] quantities, bool[] tempItems, bool rejectElse);
    void HuntForItems(string[] names, string[] items, int[] quantities, bool tempItems = false, bool rejectElse = true);
    void HuntForItems(string[] names, string[] items, int[] quantities, bool[] tempItems, bool rejectElse);
    void HuntWithPriority(string name, HuntPriorities priority);
    /// <summary>
    /// Attacks the specified instance of <paramref name="monster"/> and waits until they are killed (if SafeTiings are enabled).
    /// </summary>
    /// <param name="name">Name of the player to kill.</param>
    void Kill(Monster monster);
    void Kill(string name);
    void Kill(int mapId);
    void KillForItem(string name, string item, int quantity, bool tempItem = false, bool rejectElse = true);
    void KillForItems(string name, string[] items, int[] quantities, bool tempItems = false, bool rejectElse = true);
    /// <summary>
    /// Attacks the player with specified <paramref name="name"/> and waits until they are killed (if SafeTiings are enabled). This should only be used in PVP.
    /// </summary>
    /// <param name="name">Name of the player to kill.</param>
    void KillPlayer(string name);
    /// <summary>
    /// Rests the player (equivilent to clicking the rest button on the UI).
    /// </summary>
    /// <param name="full">If <see langword="true"/>, the bot will wait until the player's HP and MP are full.</param>
    void Rest(bool full = false, int timeout = -1);
}
