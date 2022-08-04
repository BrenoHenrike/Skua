using Skua.Core.Models;
using Skua.Core.Models.Monsters;

namespace Skua.Core.Interfaces;

public interface IScriptCombat
{
    bool StopAttacking { get; }
    /// <summary>
    /// Walks towards (approaches) the currently selected target.
    /// </summary>
    void ApproachTarget();
    /// <summary>
    /// Attacks the monster with the specified <paramref name="mapId">.
    /// </summary>
    /// <param name="mapId">The id of the monster to attack.</param>
    /// <remarks>This will not wait until the monster is killed, but simply select it and start attacking it.</remarks>
    void Attack(int id);
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
}
