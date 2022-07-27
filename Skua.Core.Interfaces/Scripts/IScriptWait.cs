using Skua.Core.Models;

namespace Skua.Core.Interfaces;

public interface IScriptWait
{
    /// <summary>
    /// The duration, in milliseconds, for which the thread will sleep before re-checking whether the awaited condition is met.
    /// </summary>
    int WAIT_SLEEP { get; set; }
    /// <summary>
    /// Whether to override all Wait timeouts with each of the defined ActionTimeouts. By default they will not change the behaviour of the bot.
    /// Methods mentioned in each ActionTimeout are used when <see cref="IScriptOption.SafeTimings"/> is enabled.
    /// </summary>
    bool OverrideTimeout { get; set; }
    /// <summary>
    /// The number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.
    /// This will override any action timeout made by the Player:
    /// <list type="bullet">
    /// <item><see cref="ForPlayerPosition(int, int, int)"/></item>
    /// <item><see cref="ForCombatExit(int)"/></item>
    /// </list>
    /// </summary>
    int PlayerActionTimeout { get; set; }
    /// <summary>
    /// The number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.
    /// This will override any action timeout related to drops:
    /// <list type="bullet">
    /// <item><see cref="ForPickup(string, int)"/></item>
    /// <item><see cref="ForDrop(string, int)"/></item>
    /// </list>
    /// </summary>
    int DropActionTimeout { get; set; }
    /// <summary>
    /// The number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.
    /// This will override any action timeout related to game actions:
    /// <list type="bullet">
    /// <item><see cref="ForActionCooldown(GameActions, int)"/></item>
    /// <item><see cref="ForActionCooldown(string, int)"/></item>
    /// </list>
    /// </summary>
    int GameActionTimeout { get; set; }
    /// <summary>
    /// The number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.
    /// This will override any action timeout related to items:
    /// <list type="bullet">
    /// <item><see cref="ForItemBuy(int)"/></item>
    /// <item><see cref="ForItemSell(int)"/></item>
    /// <item><see cref="ForItemEquip(string, int)"/></item>
    /// <item><see cref="ForItemEquip(int, int)"/></item>
    /// <item><see cref="ForBankToInventory(string, int)"/></item>
    /// <item><see cref="ForInventoryToBank(string, int)"/></item>
    /// </list>
    /// </summary>
    int ItemActionTimeout { get; set; }
    /// <summary>
    /// The number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.
    /// This will override any action timeout related to maps:
    /// <list type="bullet">
    /// <item><see cref="ForMapLoad(string, int)"/></item>
    /// <item><see cref="ForCellChange(string)"/></item>
    /// </list>
    /// </summary>
    int MapActionTimeout { get; set; }
    /// <summary>
    /// The number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.<br/>
    /// This will override any action timeout related to monsters:
    /// <list type="bullet">
    /// <item><see cref="ForMonsterSpawn(string, int)"/>.</item>
    /// </list>
    /// </summary>
    int MonsterActionTimeout { get; set; }
    /// <summary>
    /// The number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.
    /// This will override any action timeout related to quests:
    /// <list type="bullet">
    /// <item><see cref="ForQuestAccept(int, int)"/></item>
    /// <item><see cref="ForQuestComplete(int, int)"/></item>
    /// </list>
    /// </summary>
    int QuestActionTimeout { get; set; }

    /// <summary>
    /// Waits for the specified <paramref name="function"/> to return the desired <paramref name="value"/>.
    /// </summary>
    /// <param name="function">Function to poll.</param>
    /// <param name="value">Value to wait for.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool For(Func<object> function, object value, int timeout = 10);
    /// <summary>
    /// Waits for the specified <paramref name="function"/> to return the desired <paramref name="value"/>.
    /// </summary>
    /// <param name="function">Function to poll.</param>
    /// <param name="value">Value to wait for.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    ValueTask<bool> ForAsync(Func<object> function, object value, int timeout = 10, CancellationToken token = default);
    /// <summary>
    /// Waits for the specified game action to be available.
    /// </summary>
    /// <param name="action">Game action to wait for.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool ForActionCooldown(GameActions action, int timeout = 40);
    /// <summary>
    /// Waits for the specified game action to be available.
    /// </summary>
    /// <param name="action">Game action to wait for.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool ForActionCooldown(string action, int timeout = 40);
    /// <summary>
    /// Waits for the bank to be loaded. If the bank is already loaded, this method does not wait at all.
    /// </summary>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool ForBankLoad(int timeout = 20);
    /// <summary>
    /// Waits for an item with specified <paramref name="name"/> to be moved from the bank to the inventory.
    /// </summary>
    /// <param name="name">Name of the item to wait for.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool ForBankToInventory(string name, int timeout = 14);
    /// <summary>
    /// Waits for an item with specified <paramref name="id"/> to be moved from the inventory to the inventory.
    /// </summary>
    /// <param name="id">ID of the item to wait for.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool ForBankToInventory(int id, int timeout = 14);
    /// <summary>
    /// Waits for an item with specified <paramref name="name"/> to be moved from the bank to the house inventory.
    /// </summary>
    /// <param name="name">Name of the item to wait for.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool ForBankToHouseInventory(string name, int timeout = 14);
    /// <summary>
    /// Waits for an item with specified <paramref name="id"/> to be moved from the inventory to the house inventory.
    /// </summary>
    /// <param name="id">ID of the item to wait for.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool ForBankToHouseInventory(int id, int timeout = 14);
    /// <summary>
    /// Waits for the current cell to change to the specified one.
    /// </summary>
    /// <param name="name">Name of the cell to wait for.</param>
    /// <remarks>Changing between cells should be instant, so this wait is usually not necessary at all.</remarks>
    bool ForCellChange(string name);
    /// <summary>
    /// Waits until the player is no longer in combat.
    /// </summary>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool ForCombatExit(int timeout = 10);
    /// <summary>
    /// Waits for a drop with specified <paramref name="name"/> to exist.
    /// </summary>
    /// <param name="name">Name of the item to wait for.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool ForDrop(string name, int timeout = 10);
    /// <summary>
    /// Waits for a drop with specified <paramref name="id"/> to exist.
    /// </summary>
    /// <param name="id">Name of the item to wait for.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool ForDrop(int id, int timeout = 10);
    /// <summary>
    /// Waits until the player is fully rested (has maximum HP and mana).
    /// </summary>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool ForFullyRested(int timeout = -1);
    /// <summary>
    /// Waits for an item with specified <paramref name="name"/> to be moved from the inventory to the bank.
    /// </summary>
    /// <param name="name">Name of the item to wait for.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool ForInventoryToBank(string name, int timeout = 14);
    /// <summary>
    /// Waits for an item with specified <paramref name="id"/> to be moved from the inventory to the bank.
    /// </summary>
    /// <param name="id">ID of the item to wait for.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool ForInventoryToBank(int id, int timeout = 14);
    /// <summary>
    /// Waits for an item with specified <paramref name="name"/> to be moved from the house inventory to the bank.
    /// </summary>
    /// <param name="name">Name of the item to wait for.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool ForHouseInventoryToBank(string name, int timeout = 14);
    /// <summary>
    /// Waits for an item with specified <paramref name="id"/> to be moved from the house inventory to the bank.
    /// </summary>
    /// <param name="id">ID of the item to wait for.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool ForHouseInventoryToBank(int id, int timeout = 14);
    /// <summary>
    /// Waits for an item to be bought.
    /// </summary>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool ForItemBuy(int timeout = 10);
    /// <summary>
    /// Waits for an item with specified <paramref name="name"/> to be equipped.
    /// </summary>
    /// <param name="name">Name of the item to wait for.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool ForItemEquip(string name, int timeout = 10);
    /// <summary>
    /// Waits for an item with specified <paramref name="id"/> to be equipped.
    /// </summary>
    /// <param name="id">ID of the item to wait for.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool ForItemEquip(int id, int timeout = 10);
    /// <summary>
    /// Waits for an item to be sold.
    /// </summary>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool ForItemSell(int timeout = 10);
    /// <summary>
    /// Waits until a map is fully loaded.
    /// </summary>
    /// <param name="name">Name of the map to wait for.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    /// <returns>Whether or not the timeout was reached.</returns>
    bool ForMapLoad(string name, int timeout = 20);
    /// <summary>
    /// Waits until the currently targeted monster has been killed.
    /// </summary>
    /// <remarks>This actually waits for the player to have no target selected, so may not accurately reflect when the current monster is killed.</remarks>
    bool ForMonsterDeath(int timeout = -1);
    /// <summary>
    /// Waits until a monster with specified <paramref name="name"/> is present in the current cell.
    /// </summary>
    /// <param name="name">Name of the monster to wait for.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool ForMonsterSpawn(string name, int timeout = 10);
    /// <summary>
    /// Waits until a monster with specified <paramref name="id"/> is present in the current cell.
    /// </summary>
    /// <param name="id">ID of the monster to wait for.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool ForMonsterSpawn(int id, int timeout = 10);
    /// <summary>
    /// Waits for a drop with specified <paramref name="name"/> to be picked up.
    /// </summary>
    /// <param name="name">Name of the item to wait for.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    /// <remarks>This actually waits for no drops with specified <paramref name="name"/> to be available, so can be used even when you do not expect the drop to exist.</remarks>
    bool ForPickup(string name, int timeout = 10);
    /// <summary>
    /// Waits for a drop with specified <paramref name="id"/> to be picked up.
    /// </summary>
    /// <param name="id">Name of the item to wait for.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    /// <remarks>This actually waits for no drops with specified <paramref name="id"/> to be available, so can be used even when you do not expect the drop to exist.</remarks>
    bool ForPickup(int id, int timeout = 10);
    /// <summary>
    /// Waits until the player has reached a specified position.
    /// </summary>
    /// <param name="x">X coordinate value the player should be at.</param>
    /// <param name="y">Y coordinate value the player should be at.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool ForPlayerPosition(int x, int y, int timeout = 10);
    /// <summary>
    /// Waits for quest with specified <paramref name="id"/> to be accepted.
    /// </summary>
    /// <param name="id">ID of the quest to be accepted.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool ForQuestAccept(int id, int timeout = 14);
    /// <summary>
    /// Waits for quest with specified <paramref name="id"/> to be completed.
    /// </summary>
    /// <param name="id">ID of the quest to be completed.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    /// <remarks>This actually waits until the quest is no longer in progress so does not guarentee that the quest has been completed; it could have never been accepted in the first place.</remarks>
    bool ForQuestComplete(int id, int timeout = 10);
    /// <summary>
    /// Waits for the specified skill to cooldown.
    /// </summary>
    /// <param name="index">Index of the skill.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    bool ForSkillCooldown(int index, int timeout = 50);
    /// <summary>
    /// Waits for the specified <paramref name="predicate"/> to return <see langword="true"/>.
    /// </summary>
    /// <param name="predicate">Function to poll.</param>
    /// <param name="loopFunction">Function to run in between polling the <paramref name="predicate"/> function.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    /// <param name="sleepOverride">Time to sleep between polling the <paramref name="predicate"/> function (-1 = <see cref="WAIT_SLEEP"/>).</param>
    bool ForTrue(Func<bool> predicate, Action? loopFunction, int timeout, int sleepOverride = -1);
    /// <summary>
    /// Waits for the specified <paramref name="predicate"/> to return <see langword="true"/>.
    /// </summary>
    /// <param name="predicate">Function to poll.</param>
    /// <param name="loopFunction">Function to run in between polling the <paramref name="predicate"/> function.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    /// <param name="sleepOverride">Time to sleep between polling the <paramref name="predicate"/> function (-1 = <see cref="WAIT_SLEEP"/>).</param>
    ValueTask<bool> ForTrueAsync(Func<bool> predicate, Action? loopFunction, int timeout, int sleepOverride = -1, CancellationToken token = default);
    /// <summary>
    /// Waits for the specified <paramref name="predicate"/> to return <see langword="true"/>.
    /// </summary>
    /// <param name="predicate">Function to poll.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    /// <param name="sleepOverride">Time to sleep between polling the <paramref name="predicate"/> function (-1 = <see cref="WAIT_SLEEP"/>).</param>
    bool ForTrue(Func<bool> predicate, int timeout, int sleepOverride = -1);
    /// <summary>
    /// Waits for the specified <paramref name="predicate"/> to return <see langword="true"/>.
    /// </summary>
    /// <param name="predicate">Function to poll.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    /// <param name="sleepOverride">Time to sleep between polling the <paramref name="predicate"/> function (-1 = <see cref="WAIT_SLEEP"/>).</param>
    ValueTask<bool> ForTrueAsync(Func<bool> predicate, int timeout, int sleepOverride = -1, CancellationToken token = default);
    /// <summary>
    /// Waits for the specified <paramref name="predicate"/> to return <see langword="true"/>.
    /// </summary>
    /// <param name="predicate">Function to poll.</param>
    /// <param name="timeout">Number of times the thread should be slept (for <see cref="WAIT_SLEEP"/> milliseconds) before the wait is cancelled.</param>
    /// <param name="sleepOverride">Time to sleep between polling the <paramref name="predicate"/> function (-1 = <see cref="WAIT_SLEEP"/>).</param>
    bool ForTrue(Func<bool> predicate, Action? loopFunction, int sleepOverride, CancellationToken token);
    /// <summary>
    /// Checks whether the given game action is cooled down or not.
    /// </summary>
    /// <param name="action">The game action to check.</param>
    /// <returns><see langword="true"/> if the given game action has cooled down.</returns>
    bool IsActionAvailable(GameActions action);
    /// <summary>
    /// Checks whether the given game action is cooled down or not.
    /// </summary>
    /// <param name="action">The game action to check.</param>
    /// <returns><see langword="true"/> if the given game action has cooled down.</returns>
    bool IsActionAvailable(string action);
}
