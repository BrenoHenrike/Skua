using Skua.Core.Models.Monsters;

namespace Skua.Core.Interfaces;

public interface IScriptKill
{
    /// <summary>
    /// Attacks the player with specified <paramref name="name"/> and waits until they are killed (if SafeTimings are enabled). This should only be used in PVP.
    /// </summary>
    /// <param name="name">Name of the player to kill.</param>
    void Player(string name);

    /// <summary>
    /// Attacks the specified instance of <paramref name="monster"/> and waits until they are killed (if SafeTimings are enabled).
    /// </summary>
    /// <param name="monster">Name of the monster to kill.</param>
    void Monster(Monster monster);

    /// <summary>
    /// Attacks the specified instance of <paramref name="monster"/> and waits until they are killed (if SafeTimings are enabled).
    /// </summary>
    /// <param name="monster">Name of the monster to kill.</param>
    void Monster(Monster monster, CancellationToken? token);

    /// <summary>
    /// Attacks the monster with specified name and waits until it is killed (if SafeTimings are enabled).
    /// </summary>
    /// <param name="name">The name of the monster to kill.</param>
    void Monster(string name);

    /// <summary>
    /// Attacks the monster with specified name and waits until it is killed (if SafeTimings are enabled).
    /// </summary>
    /// <param name="name">The name of the monster to kill.</param>
    void Monster(string name, CancellationToken? token);

    /// <summary>
    /// Attacks the monster with specified id and waits until it is killed (if SafeTimings are enabled).
    /// </summary>
    /// <param name="id">The id of the monster to kill.</param>
    void Monster(int id);

    /// <summary>
    /// Attacks the monster with specified id and waits until it is killed (if SafeTimings are enabled).
    /// </summary>
    /// <param name="id">The id of the monster to kill.</param>
    /// <param name="token">Cancellation Token.</param>
    void Monster(int id, CancellationToken? token);

    /// <summary>
    /// Kills the specified monster (in the current player cell) until the desired item is collected in the desired quantity.
    /// </summary>
    /// <param name="name">The name of the monster to kill.</param>
    /// <param name="item">The item to collect.</param>
    /// <param name="quantity">The quantity of the item that must be collected before stopping the killing of the monster.</param>
    /// <param name="tempItem">Whether or not the item being collected is a temporary (quest) item.</param>
    void ForItem(string name, string item, int quantity, bool tempItem = false);

    /// <summary>
    /// Kills the all the specified monsters (in the current player cell) until the desired item is collected in the desired quantity.
    /// </summary>
    /// <param name="names">The names of the monsters to kill.</param>
    /// <param name="item">The item to collect.</param>
    /// <param name="quantity">The quantity of the item that must be collected before stopping the killing of the monster.</param>
    /// <param name="tempItem">Whether or not the item being collected is a temporary (quest) item.</param>
    void ForItem(IEnumerable<string> names, string item, int quantity, bool tempItem = false);

    /// <summary>
    /// Kills the specified monster (in the current player cell) until the desired items are collected in the desired quantities.
    /// </summary>
    /// <param name="name">The name of the monster to kill.</param>
    /// <param name="items">The items to collect.</param>
    /// <param name="quantities">The quantities of the items that must be collected before stopping the killing of the monster.</param>
    /// <param name="tempItems">Whether or not the items being collected are temporary (quest) items.</param>
    void ForItems(string name, IEnumerable<string> items, IEnumerable<int> quantities, bool tempItems = false);

    /// <summary>
    /// Kills the specified monster (in the current player cell) until the desired items are collected in the desired quantities.
    /// </summary>
    /// <param name="name">The name of the monster to kill.</param>
    /// <param name="items">The items to collect.</param>
    /// <param name="quantities">The quantities of the items that must be collected before stopping the killing of the monster.</param>
    /// <param name="tempItems">Whether or not the items being collected are temporary (quest) items.</param>
    void ForItems(string name, IEnumerable<string> items, IEnumerable<int> quantities, IEnumerable<bool> tempItems);

    /// <summary>
    /// Kills the all the specified monsters (in the current player cell) until the desired items are collected in the desired quantities.
    /// </summary>
    /// <param name="names">The names of the monsters to kill.</param>
    /// <param name="items">The items to collect.</param>
    /// <param name="quantities">The quantities of the items that must be collected before stopping the killing of the monster.</param>
    /// <param name="tempItems">Whether or not the items being collected are temporary (quest) items.</param>
    void ForItems(IEnumerable<string> names, IEnumerable<string> items, IEnumerable<int> quantities, bool tempItems = false);

    /// <summary>
    /// Kills the all the specified monsters (in the current player cell) until the desired items are collected in the desired quantities.
    /// </summary>
    /// <param name="names">The names of the monsters to kill.</param>
    /// <param name="items">The items to collect.</param>
    /// <param name="quantities">The quantities of the items that must be collected before stopping the killing of the monster.</param>
    /// <param name="tempItems">Whether or not the items being collected are temporary (quest) items.</param>
    void ForItems(IEnumerable<string> names, IEnumerable<string> items, IEnumerable<int> quantities, IEnumerable<bool> tempItems);
}