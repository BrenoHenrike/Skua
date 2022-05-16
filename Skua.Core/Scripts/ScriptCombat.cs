using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.PostSharp;

namespace Skua.Core.Scripts;
public class ScriptCombat : ScriptableObject, IScriptCombat
{
    public void Rest(bool full = false)
    {
        if (Bot.Options.SafeTimings)
            Bot.Wait.ForActionCooldown(GameActions.Rest);
        Bot.Flash.CallGameFunction("world.rest");
        if (full)
            Bot.Wait.ForTrue(() => Bot.Player.Health >= Bot.Player.MaxHealth && Bot.Player.Mana >= Bot.Player.MaxMana, 20);
    }

    [MethodCallBinding("world.approachTarget", GameFunction = true)]
    public void ApproachTarget() { }

    [MethodCallBinding("untargetSelf")]
    public void UntargetSelf() { }

    [MethodCallBinding("world.cancelTarget", RunMethodPost = true, GameFunction = true)]
    public void CancelTarget()
    {
        if (Bot.Options.SafeTimings)
            Bot.Wait.ForMonsterDeath();
    }

    [MethodCallBinding("world.cancelAutoAttack", GameFunction = true)]
    public void CancelAutoAttack() { }

    public void Exit()
    {
        if (Bot.Player.State == 1)
            return;
        CancelAutoAttack();
        CancelTarget();
        Bot.Map.Jump(Bot.Player.Cell, Bot.Player.Pad);
        Bot.Sleep(300);
        Bot.Map.Jump(Bot.Player.Cell, Bot.Player.Pad);
        Bot.Wait.ForCombatExit();
    }

    [MethodCallBinding("attackMonsterName")]
    public void Attack(string name) { }

    [MethodCallBinding("attackMonsterID")]
    public void Attack(int id) { }

    private int _lastHuntTick;
    /// <summary>
    /// Looks for the enemy in the current map and kills it. This method disregards ScriptOptions#HuntPriority.
    /// </summary>
    /// <param name="name">The name of the enemy to hunt.</param>
    public void Hunt(string name) => _Hunt(name, null);

    /// <summary>
    /// Hunts monsters with a priority. If there is no priority, this has the same behaviour as just Hunt.
    /// If a priority is specified, monsters in the map are sorted by the given priority. Once sorted, the
    /// monster in the current cell which best matches the priority is killed. Otherwise, a cell jump is
    /// awaited and done based on ScriptOptions#HuntDelay.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="priority"></param>
    public void HuntWithPriority(string name, HuntPriorities priority) => _HuntWithPriority(name, priority, null);

    private void _Hunt(string name, CancellationToken? token)
    {
        Bot.Lite.UntargetSelf = true;
        Bot.Lite.UntargetDead = true;
        string[] names = name.Split('|');
        _lastHuntTick = Environment.TickCount;
        while (!token?.IsCancellationRequested ?? true)
        {
            List<string> cells = names.SelectMany(n => Bot.Monsters.GetLivingMonsterCells(n)).Distinct().ToList();
            foreach (string cell in cells)
            {
                if (token?.IsCancellationRequested ?? false)
                    break;
                if (!cells.Contains(Bot.Player.Cell) && (!token?.IsCancellationRequested ?? true))
                {
                    if (Environment.TickCount - _lastHuntTick < Bot.Options.HuntDelay)
                        Bot.Sleep(Bot.Options.HuntDelay - Environment.TickCount + _lastHuntTick);
                    Bot.Map.Jump(cell, "Left");
                    _lastHuntTick = Environment.TickCount;
                }
                foreach (string mon in names)
                {
                    if (token?.IsCancellationRequested ?? false)
                        break;
                    if (Bot.Monsters.Exists(mon) && (!token?.IsCancellationRequested ?? true))
                    {
                        _Kill(mon, token);
                        return;
                    }
                }
                Bot.Sleep(200);
            }
        }
    }

    private void _HuntWithPriority(string name, HuntPriorities priority, CancellationToken? token)
    {
        //if (priority == HuntPriorities.None)
        //{
        //    _Hunt(name, token);
        //    return;
        //}

        //Bot.Lite.UntargetSelf = true;
        //Bot.Lite.UntargetDead = true;
        //_lastHuntTick = Environment.TickCount;
        //while (!token?.IsCancellationRequested ?? true)
        //{
        //    string[] names = name.Split('|').Select(x => x.ToLower()).ToArray();
        //    IOrderedEnumerable<Monster> ordered = Bot.Monsters.MapMonsters.OrderBy(x => 0);
        //    if (priority.HasFlag(HuntPriorities.HighHP))
        //        ordered = ordered.OrderByDescending(x => x.HP);
        //    else if (priority.HasFlag(HuntPriorities.LowHP))
        //        ordered = ordered.OrderBy(x => x.HP);
        //    if (priority.HasFlag(HuntPriorities.Close))
        //        ordered = ordered.OrderBy(x => x.Cell == Cell ? 0 : 1);
        //    List<Monster> targets = ordered.Where(m => names.Any(n => n == "*" || n.Trim().Equals(m.Name.Trim(), StringComparison.OrdinalIgnoreCase)) && m.Alive).ToList();
        //    foreach (Monster target in targets)
        //    {
        //        if (token?.IsCancellationRequested ?? false)
        //            break;
        //        bool sameCell = Bot.Monsters.Exists(target.Name);
        //        if (sameCell || CanJumpForHunt())
        //        {
        //            if (!sameCell && (!token?.IsCancellationRequested ?? true))
        //            {
        //                if (Bot.Player.Cell == target.Cell)
        //                    continue;
        //                Jump(target.Cell, "Left");
        //                _lastHuntTick = Environment.TickCount;
        //            }
        //            _Kill(target, token);
        //            return;
        //        }
        //    }
        //    Bot.Sleep(200);
        //}
    }

    /// <summary>
    /// Hunts the specified monster for a specific item.
    /// </summary>
    /// <param name="name">The name of the monster to kill.</param>
    /// <param name="item">The item to collect.</param>
    /// <param name="quantity">The quantity of the item that must be collected before stopping the killing of the monster.</param>
    /// <param name="tempItem">Whether or not the item being collected is a temporary (quest) item.</param>
    /// <param name="rejectElse">Whether or not to reject items which are not the 'item' paramater.</param>
    public void HuntForItem(string name, string item, int quantity, bool tempItem = false, bool rejectElse = true)
    {
        //HuntCTS = new();
        //this.item = (item, quantity, tempItem);
        //Bot.Events.ItemDropped += ItemHunted;
        //while (!Bot.ShouldExit
        //    && (tempItem || !Bot.Inventory.Contains(item, quantity))
        //    && (!tempItem || !Bot.Inventory.ContainsTempItem(item, quantity)))
        //{
        //    _HuntWithPriority(name, Bot.Options.HuntPriority, HuntCTS.Token);
        //    if (rejectElse)
        //        RejectExcept(item);
        //}
        //Bot.Events.ItemDropped -= ItemHunted;
        //HuntCTS.Dispose();
        //HuntCTS = null;
        //CancelTarget();
        //CancelAutoAttack();
        //Jump(Cell, Pad);
        //Bot.Wait.ForCombatExit();
    }

    private (string name, int quantity, bool isTemp) item = ("", 0, false);
    private CancellationTokenSource HuntCTS;

    private void ItemHunted(ScriptInterface bot, ItemBase item, bool addedToInv, int quantityNow)
    {
        //if (item.Name != this.item.name)
        //    return;

        //if (addedToInv && !item.Temp && quantityNow >= this.item.quantity)
        //{
        //    HuntCTS?.Cancel();
        //    return;
        //}
        //Pickup(item.Name);
        //int quant = this.item.isTemp ? bot.Inventory.GetTempQuantity(item.Name) : bot.Inventory.GetQuantity(item.Name);
        //if (quant >= this.item.quantity)
        //    HuntCTS?.Cancel();
    }

    /// <summary>
    /// Hunts the specified monsters for a specific item.
    /// </summary>
    /// <param name="names">The array of names of monsters to kill.</param>
    /// <param name="item">The item to collect.</param>
    /// <param name="quantity">The quantity of the item that must be collected before stopping the killing of the monster.</param>
    /// <param name="tempItem">Whether or not the item being collected is a temporary (quest) item.</param>
    /// <param name="rejectElse">Whether or not to reject items which are not the 'item' paramater.</param>
    public void HuntForItem(string[] names, string item, int quantity, bool tempItem = false, bool rejectElse = true)
    {
        HuntForItem(ConvertToNamesString(names), item, quantity, tempItem, rejectElse);
    }

    /// <summary>
    /// Hunts the specified monsters for a specific item.
    /// </summary>
    /// <param name="names">The list of names of monsters to kill.</param>
    /// <param name="item">The item to collect.</param>
    /// <param name="quantity">The quantity of the item that must be collected before stopping the killing of the monster.</param>
    /// <param name="tempItem">Whether or not the item being collected is a temporary (quest) item.</param>
    /// <param name="rejectElse">Whether or not to reject items which are not the 'item' paramater.</param>
    public void HuntForItem(List<string> names, string item, int quantity, bool tempItem = false, bool rejectElse = true)
    {
        HuntForItem(ConvertToNamesString(names), item, quantity, tempItem, rejectElse);
    }

    /// <summary>
    /// Hunts the specified monster until the desired items are collected in the desired quantities.
    /// </summary>
    /// <param name="name">The name of the monster to kill.</param>
    /// <param name="items">The items to collect.</param>
    /// <param name="quantities">The quantities of the items that must be collected before stopping the killing of the monster.</param>
    /// <param name="tempItems">Whether or not each item being collected is a temporary (quest) item.</param>
    /// <param name="rejectElse">Whether or not to reject items which are not contained in the 'items' array.</param>
    public void HuntForItems(string name, string[] items, int[] quantities, bool[] tempItems, bool rejectElse)
    {
        //if (items.Length != quantities.Length)
        //{
        //    Bot.Log("Item count does not match quantity count.");
        //    return;
        //}
        //bool[] temp = tempItems ?? new bool[tempItems.Length];
        //for (int i = 0; i < items.Length; i++)
        //{
        //    HuntForItem(name, items[i], quantities[i], temp[i], false);
        //    if (rejectElse)
        //        RejectExcept(items);
        //}
    }

    /// <summary>
    /// Hunts the specified monster until the desired items are collected in the desired quantities.
    /// </summary>
    /// <param name="name">The name of the monster to kill.</param>
    /// <param name="items">The items to collect.</param>
    /// <param name="quantities">The quantities of the items that must be collected before stopping the killing of the monster.</param>
    /// <param name="tempItems">Whether or not the items being collected are temporary (quest) items.</param>
    /// <param name="rejectElse">Whether or not to reject items which are not contained in the 'items' array.</param>
    public void HuntForItems(string name, string[] items, int[] quantities, bool tempItems = false, bool rejectElse = true)
    {
        HuntForItems(name, items, quantities, Enumerable.Range(0, items.Length).Select(i => tempItems).ToArray(), rejectElse);
    }

    /// <summary>
    /// Hunts the specified monster until the desired items are collected in the desired quantities.
    /// </summary>
    /// <param name="names">The names of the monsters to kill.</param>
    /// <param name="items">The item to collect.</param>
    /// <param name="quantities">The quantities of the items that must be collected before stopping the killing of the monster.</param>
    /// <param name="tempItems">Whether or not each item being collected is a temporary (quest) item.</param>
    /// <param name="rejectElse">Whether or not to reject items which are not contained in the 'items' array.</param>
    public void HuntForItems(string[] names, string[] items, int[] quantities, bool[] tempItems, bool rejectElse)
    {
        HuntForItems(ConvertToNamesString(names), items, quantities, tempItems, rejectElse);
    }

    /// <summary>
    /// Hunts the specified monster until the desired items are collected in the desired quantities.
    /// </summary>
    /// <param name="names">The names of the monsters to kill.</param>
    /// <param name="items">The items to collect.</param>
    /// <param name="quantities">The quantities of the items that must be collected before stopping the killing of the monster.</param>
    /// <param name="tempItems">Whether or not the items being collected are temporary (quest) items.</param>
    /// <param name="rejectElse">Whether or not to reject items which are not contained in the 'items' array.</param>
    public void HuntForItems(string[] names, string[] items, int[] quantities, bool tempItems = false, bool rejectElse = true)
    {
        HuntForItems(names, items, quantities, Enumerable.Range(0, items.Length).Select(i => tempItems).ToArray(), rejectElse);
    }

    /// <summary>
    /// Hunts the specified monster until the desired items are collected in the desired quantities.
    /// </summary>
    /// <param name="names">The names of the monsters to kill.</param>
    /// <param name="items">The items to collect.</param>
    /// <param name="quantities">The quantities of the items that must be collected before stopping the killing of the monster.</param>
    /// <param name="tempItems">Whether or not each item being collected is a temporary (quest) item.</param>
    /// <param name="rejectElse">Whether or not to reject items which are not contained in the 'items' array.</param>
    public void HuntForItems(List<string> names, string[] items, int[] quantities, bool[] tempItems, bool rejectElse)
    {
        HuntForItems(ConvertToNamesString(names), items, quantities, tempItems, rejectElse);
    }

    /// <summary>
    /// Hunts the specified monster until the desired items are collected in the desired quantities.
    /// </summary>
    /// <param name="names">The names of the monsters to kill.</param>
    /// <param name="items">The items to collect.</param>
    /// <param name="quantities">The quantities of the items that must be collected before stopping the killing of the monster.</param>
    /// <param name="tempItems">Whether or not the items being collected are temporary (quest) items.</param>
    /// <param name="rejectElse">Whether or not to reject items which are not contained in the 'items' array.</param>
    public void HuntForItems(List<string> names, string[] items, int[] quantities, bool tempItems = false, bool rejectElse = true)
    {
        HuntForItems(names, items, quantities, Enumerable.Range(0, items.Length).Select(i => tempItems).ToArray(), rejectElse);
    }

    private bool CanJumpForHunt()
    {
        return Environment.TickCount - _lastHuntTick >= Bot.Options.HuntDelay;
    }

    internal string saveCell = "Enter", savePad = "Spawn";

    /// <summary>
    /// Attacks the specified monster and waits until it is killed (if SafeTimings are enabled).
    /// </summary>
    /// <param name="name">The name of the monster to kill.</param>
    public void Kill(string name) => _Kill(name, null);

    internal void _Kill(string name, CancellationToken? token)
    {
        if (Bot.Options.SafeTimings)
            Bot.Wait.ForMonsterSpawn(name, 30);
        Attack(name);
        if (token is null)
        {
            Bot.Wait.ForMonsterDeath();
            return;
        }
        //Bot.Wait.ForMonsterDeath((CancellationToken)token);
    }

    /// <summary>
    /// Attacks the specified instance of a monster and waits until it is killed (if SafeTimings are enabled).
    /// </summary>
    /// <param name="monster">The monster to kill.</param>
    public void Kill(Monster monster) => _Kill(monster, null);

    internal void _Kill(Monster monster, CancellationToken? token)
    {
        //if (Bot.Options.SafeTimings)
        //    Bot.Wait.ForTrue(() => Bot.Monsters.CurrentMonsters.Contains(m => m.MapID == monster.MapID && m.Alive), 30);
        //Attack(monster.MapID);
        //if (token is null)
        //{
        //    Bot.Wait.ForMonsterDeath();
        //    return;
        //}
        //Bot.Wait._ForMonsterDeath((CancellationToken)token);
    }

    /// <summary>
    /// Kills the specified monster until the desired item is collected in the desired quantity.
    /// </summary>
    /// <param name="name">The name of the monster to kill.</param>
    /// <param name="item">The item to collect.</param>
    /// <param name="quantity">The quantity of the item that must be collected before stopping the killing of the monster.</param>
    /// <param name="tempItem">Whether or not the item being collected is a temporary (quest) item.</param>
    /// <param name="rejectElse">Whether or not to reject items which are not the 'item' paramater.</param>
    public void KillForItem(string name, string item, int quantity, bool tempItem = false, bool rejectElse = true)
    {
        //saveCell = Cell;
        //savePad = Pad;
        //while (!Bot.ShouldExit
        //    && (tempItem || !Bot.Inventory.Contains(item, quantity))
        //    && (!tempItem || !Bot.Inventory.ContainsTempItem(item, quantity)))
        //{
        //    if (Cell != saveCell)
        //        Jump(saveCell, savePad);
        //    Attack(name);
        //    Pickup(item);
        //    if (rejectElse)
        //        RejectExcept(item);
        //}
        //saveCell = savePad = "";
    }

    /// <summary>
    /// Kills the specified monster until the desired items are collected in the desired quantities.
    /// </summary>
    /// <param name="name">The name of the monster to kill.</param>
    /// <param name="items">The item to collect.</param>
    /// <param name="quantities">The quantities of the items that must be collected before stopping the killing of the monster.</param>
    /// <param name="tempItems">Whether or not the items being collected are temporary (quest) items.</param>
    /// <param name="rejectElse">Whether or not to reject items which are not contained in the 'items' array.</param>
    public void KillForItems(string name, string[] items, int[] quantities, bool tempItems = false, bool rejectElse = true)
    {
        //if (items.Length != quantities.Length)
        //{
        //    Bot.Log("Item count does not match quantity count.");
        //    return;
        //}
        //saveCell = Cell;
        //savePad = Pad;
        //while (!Bot.ShouldExit
        //    && Enumerable.Range(0, items.Length).All(i => (!tempItems && Bot.Inventory.Contains(items[i], quantities[i]))
        //    || (tempItems && Bot.Inventory.ContainsTempItem(items[i], quantities[i]))))
        //{
        //    if (Cell != saveCell)
        //        Jump(saveCell, savePad);
        //    Attack(name);
        //    Bot.Drops.Pickup(items);
        //    if (rejectElse)
        //        RejectExcept(items);
        //}
    }

    [MethodCallBinding("attackPlayer")]
    public void AttackPlayer(string name) { }

    public void KillPlayer(string name)
    {
        AttackPlayer(name);
        if (Bot.Options.SafeTimings)
            Bot.Wait.ForMonsterDeath();
    }

    private string ConvertToNamesString(IEnumerable<string> names)
    {
        return string.Join("|", names);
    }

    public void Kill(int mapId)
    {
        throw new NotImplementedException();
    }

    public void Rest(bool full = false, int timeout = -1)
    {
        throw new NotImplementedException();
    }
}
