using Skua.Core.Models;
using Skua.Core.Models.Monsters;

namespace Skua.Core.Interfaces;

public interface IScriptHunt
{
    /// <summary>
    /// Looks for the monster with specified name in the current map and kills it. This method disregards ScriptOptions#HuntPriority.
    /// </summary>
    /// <param name="name">The name of the enemy to hunt.</param>
    void Monster(string name);

    /// <summary>
    /// Looks for the monster with specified id in the current map and kills it. This method disregards ScriptOptions#HuntPriority.
    /// </summary>
    /// <param name="name">The name of the enemy to hunt.</param>
    void Monster(string name, CancellationToken? token);

    /// <summary>
    /// Looks for the monster with specified name in the current map and kills it. This method disregards ScriptOptions#HuntPriority.
    /// </summary>
    /// <param name="id">The id of the enemy to hunt.</param>
    void Monster(int id);

    /// <summary>
    /// Looks for the monster with specified id in the current map and kills it. This method disregards ScriptOptions#HuntPriority.
    /// </summary>
    /// <param name="id">The id of the enemy to hunt.</param>
    void Monster(int id, CancellationToken? token);

    /// <summary>
    /// Looks for the instance of monster in the current map and kills it. This method disregards ScriptOptions#HuntPriority.
    /// </summary>
    /// <param name="monster">The instance of the monster to hunt.</param>
    void Monster(Monster monster);

    /// <summary>
    /// Looks for the instance of monster in the current map and kills it. This method disregards ScriptOptions#HuntPriority.
    /// </summary>
    /// <param name="monster">The instance of the monster to hunt.</param>
    void Monster(Monster monster, CancellationToken? token);

    /// <summary>
    /// Hunts the specified monster for a specific item.
    /// </summary>
    /// <param name="name">The name of the monster to kill.</param>
    /// <param name="item">The item to collect.</param>
    /// <param name="quantity">The quantity of the item that must be collected before stopping the killing of the monster.</param>
    /// <param name="tempItem">Whether or not the item being collected is a temporary (quest) item.</param>
    void ForItem(string name, string item, int quantity, bool tempItem = false);

    /// <summary>
    /// Hunts the specified monsters for a specific item.
    /// </summary>
    /// <param name="names">The names of monsters to kill.</param>
    /// <param name="item">The item to collect.</param>
    /// <param name="quantity">The quantity of the item that must be collected before stopping the killing of the monster.</param>
    /// <param name="tempItem">Whether or not the item being collected is a temporary (quest) item.</param>
    void ForItem(IEnumerable<string> names, string item, int quantity, bool tempItem = false);

    /// <summary>
    /// Hunts the specified monster until the desired items are collected in the desired quantities.
    /// </summary>
    /// <param name="name">The name of the monster to kill.</param>
    /// <param name="items">The items to collect.</param>
    /// <param name="quantities">The quantities of the items that must be collected before stopping the killing of the monster.</param>
    /// <param name="tempItems">Whether or not the items being collected are temporary (quest) items.</param>
    void ForItems(string name, IEnumerable<string> items, IEnumerable<int> quantities, bool tempItems = false);

    /// <summary>
    /// Hunts the specified monster until the desired items are collected in the desired quantities.
    /// </summary>
    /// <param name="name">The name of the monster to kill.</param>
    /// <param name="items">The items to collect.</param>
    /// <param name="quantities">The quantities of the items that must be collected before stopping the killing of the monster.</param>
    /// <param name="tempItems">Whether or not the items being collected are temporary (quest) items.</param>
    void ForItems(string name, IEnumerable<string> items, IEnumerable<int> quantities, IEnumerable<bool> tempItems);

    /// <summary>
    /// Hunts the specified monster until the desired items are collected in the desired quantities.
    /// </summary>
    /// <param name="names">The names of the monsters to kill.</param>
    /// <param name="items">The items to collect.</param>
    /// <param name="quantities">The quantities of the items that must be collected before stopping the killing of the monster.</param>
    /// <param name="tempItems">Whether or not each item being collected is a temporary (quest) item.</param>
    void ForItems(IEnumerable<string> names, IEnumerable<string> items, IEnumerable<int> quantities, bool tempItems = false);

    /// <summary>
    /// Hunts the specified monster until the desired items are collected in the desired quantities.
    /// </summary>
    /// <param name="names">The names of the monsters to kill.</param>
    /// <param name="items">The items to collect.</param>
    /// <param name="quantities">The quantities of the items that must be collected before stopping the killing of the monster.</param>
    /// <param name="tempItems">Whether or not each item being collected is a temporary (quest) item.</param>
    void ForItems(IEnumerable<string> names, IEnumerable<string> items, IEnumerable<int> quantities, IEnumerable<bool> tempItems);

    /// <summary>
    /// Hunts monsters with a priority. If there is no priority, this has the same behaviour as just Hunt.
    /// If a priority is specified, monsters in the map are sorted by the given priority. Once sorted, the
    /// monster in the current cell which best matches the priority is killed. Otherwise, a cell jump is
    /// awaited and done based on ScriptOptions#HuntDelay.
    /// </summary>
    /// <param name="name">The name of the monster to hunt</param>
    /// <param name="priority">The priority which the hunts will be ordered</param>
    void WithPriority(string name, HuntPriorities priority);

    /// <summary>
    /// Hunts monsters with a priority. If there is no priority, this has the same behaviour as just Hunt.
    /// If a priority is specified, monsters in the map are sorted by the given priority. Once sorted, the
    /// monster in the current cell which best matches the priority is killed. Otherwise, a cell jump is
    /// awaited and done based on ScriptOptions#HuntDelay.
    /// </summary>
    /// <param name="name">The name of the monster to hunt</param>
    /// <param name="priority">The priority which the hunts will be ordered</param>
    void WithPriority(string name, HuntPriorities priority, CancellationToken? token);

    /// <summary>
    /// Hunts monsters with a priority. If there is no priority, this has the same behaviour as just Hunt.
    /// If a priority is specified, monsters in the map are sorted by the given priority. Once sorted, the
    /// monster in the current cell which best matches the priority is killed. Otherwise, a cell jump is
    /// awaited and done based on ScriptOptions#HuntDelay.
    /// </summary>
    /// <param name="monster">The instance of the monster to hunt</param>
    /// <param name="priority">The priority which the hunts will be ordered</param>
    void WithPriority(Monster monster, HuntPriorities priority);

    /// <summary>
    /// Hunts monsters with a priority. If there is no priority, this has the same behaviour as just Hunt.
    /// If a priority is specified, monsters in the map are sorted by the given priority. Once sorted, the
    /// monster in the current cell which best matches the priority is killed. Otherwise, a cell jump is
    /// awaited and done based on ScriptOptions#HuntDelay.
    /// </summary>
    /// <param name="monster">The instance of the monster to hunt</param>
    /// <param name="priority">The priority which the hunts will be ordered</param>
    void WithPriority(Monster monster, HuntPriorities priority, CancellationToken? token);

    /// <summary>
    /// Hunts monsters with a priority. If there is no priority, this has the same behaviour as just Hunt.
    /// If a priority is specified, monsters in the map are sorted by the given priority. Once sorted, the
    /// monster in the current cell which best matches the priority is killed. Otherwise, a cell jump is
    /// awaited and done based on ScriptOptions#HuntDelay.
    /// </summary>
    /// <param name="id">The id of the monster to hunt</param>
    /// <param name="priority">The priority which the hunts will be ordered</param>
    void WithPriority(int id, HuntPriorities priority);

    /// <summary>
    /// Hunts monsters with a priority. If there is no priority, this has the same behaviour as just Hunt.
    /// If a priority is specified, monsters in the map are sorted by the given priority. Once sorted, the
    /// monster in the current cell which best matches the priority is killed. Otherwise, a cell jump is
    /// awaited and done based on ScriptOptions#HuntDelay.
    /// </summary>
    /// <param name="id">The id of the monster to hunt</param>
    /// <param name="priority">The priority which the hunts will be ordered</param>
    void WithPriority(int id, HuntPriorities priority, CancellationToken? token);
}