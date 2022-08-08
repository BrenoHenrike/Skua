using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models;
using Skua.Core.Models.Monsters;
using Skua.Core.Utils;

namespace Skua.Core.Scripts;
public class ScriptHunt : IScriptHunt
{
    public ScriptHunt(
        Lazy<IScriptOption> options,
        Lazy<IScriptCombat> combat,
        Lazy<IScriptPlayer> player,
        Lazy<IScriptMonster> monsters,
        Lazy<IScriptDrop> drops,
        Lazy<IScriptInventory> inventory,
        Lazy<IScriptTempInv> tempInv,
        Lazy<IScriptManager> manager,
        Lazy<IScriptMap> map,
        Lazy<IScriptKill> kill,
        ILogService logger)
    {
        _lazyOptions = options;
        _lazyCombat = combat;
        _lazyPlayer = player;
        _lazyMonsters = monsters;
        _lazyDrops = drops;
        _lazyInventory = inventory;
        _lazyTempInv = tempInv;
        _lazyManager = manager;
        _lazyMap = map;
        _lazyKill = kill;
        _logger = logger;
        _lastHuntTick = Environment.TickCount;

        StrongReferenceMessenger.Default.Register<ScriptHunt, ScriptStoppedMessage, int>(this, (int)MessageChannels.ScriptStatus, ScriptStopped);
    }

    private readonly Lazy<IScriptOption> _lazyOptions;
    private readonly Lazy<IScriptCombat> _lazyCombat;
    private readonly Lazy<IScriptPlayer> _lazyPlayer;
    private readonly Lazy<IScriptMonster> _lazyMonsters;
    private readonly Lazy<IScriptDrop> _lazyDrops;
    private readonly Lazy<IScriptInventory> _lazyInventory;
    private readonly Lazy<IScriptTempInv> _lazyTempInv;
    private readonly Lazy<IScriptManager> _lazyManager;
    private readonly Lazy<IScriptMap> _lazyMap;
    private readonly Lazy<IScriptKill> _lazyKill;
    private readonly ILogService _logger;

    private IScriptOption Options => _lazyOptions.Value;
    private IScriptCombat Combat => _lazyCombat.Value;
    private IScriptPlayer Player => _lazyPlayer.Value;
    private IScriptMonster Monsters => _lazyMonsters.Value;
    private IScriptDrop Drops => _lazyDrops.Value;
    private IScriptInventory Inventory => _lazyInventory.Value;
    private IScriptTempInv TempInv => _lazyTempInv.Value;
    private IScriptMap Map => _lazyMap.Value;
    private IScriptKill Kill => _lazyKill.Value;
    private IScriptManager Manager => _lazyManager.Value;

    private int _lastHuntTick;

    public void Monster(string name) => _Hunt(name, null);
    public void Monster(string name, CancellationToken? token) => _Hunt(name, token);

    public void Monster(int id) => _Hunt(id, null);
    public void Monster(int id, CancellationToken? token) => _Hunt(id, token);

    public void Monster(Monster monster) => _Hunt(monster.ID, null);
    public void Monster(Monster monster, CancellationToken? token) => _Hunt(monster.ID, token);

    public void WithPriority(string name, HuntPriorities priority) => _HuntWithPriority(name, priority, null);
    public void WithPriority(string name, HuntPriorities priority, CancellationToken? token) => _HuntWithPriority(name, priority, token);

    public void WithPriority(Monster monster, HuntPriorities priority) => _HuntWithPriority(monster.ID, priority, null);
    public void WithPriority(Monster monster, HuntPriorities priority, CancellationToken? token) => _HuntWithPriority(monster.ID, priority, token);

    public void WithPriority(int id, HuntPriorities priority) => _HuntWithPriority(id, priority, null);
    public void WithPriority(int id, HuntPriorities priority, CancellationToken? token) => _HuntWithPriority(id, priority, token);

    private void _Hunt(string name, CancellationToken? token)
    {
        string[] names = name.Split('|');
        while (!token?.IsCancellationRequested ?? !Manager.ShouldExit)
        {
            List<string> cells = names.SelectMany(n => Monsters.GetLivingMonsterCells(n)).Distinct().ToList();
            foreach (string cell in cells)
            {
                if (token?.IsCancellationRequested ?? false)
                    break;
                if (!cells.Contains(Player.Cell) && (!token?.IsCancellationRequested ?? true))
                {
                    if (Environment.TickCount - _lastHuntTick < Options.HuntDelay)
                        Thread.Sleep(Options.HuntDelay - Environment.TickCount + _lastHuntTick);
                    Map.Jump(cell, "Left");
                    _lastHuntTick = Environment.TickCount;
                }
                foreach (string mon in names)
                {
                    if (token?.IsCancellationRequested ?? false)
                        break;
                    if (Monsters.Exists(mon) && (!token?.IsCancellationRequested ?? true))
                    {
                        Kill.Monster(mon, token);
                        return;
                    }
                }
                Thread.Sleep(200);
            }
        }
    }
    private void _Hunt(int id, CancellationToken? token)
    {
        while (!token?.IsCancellationRequested ?? !Manager.ShouldExit)
        {
            List<string> cells = Monsters.GetLivingMonsterCells(id);
            foreach (string cell in cells)
            {
                if (token?.IsCancellationRequested ?? false)
                    break;
                if (!cells.Contains(Player.Cell) && (!token?.IsCancellationRequested ?? true))
                {
                    if (Environment.TickCount - _lastHuntTick < Options.HuntDelay)
                        Thread.Sleep(Options.HuntDelay - Environment.TickCount + _lastHuntTick);
                    Map.Jump(cell, "Left");
                    _lastHuntTick = Environment.TickCount;
                }
                if (token?.IsCancellationRequested ?? false)
                    break;
                if (Monsters.Exists(id) && (!token?.IsCancellationRequested ?? true))
                {
                    Kill.Monster(id, token);
                    return;
                }
                Thread.Sleep(200);
            }
        }
    }
    private void _HuntWithPriority(string name, HuntPriorities priority, CancellationToken? token)
    {
        if (priority == HuntPriorities.None)
        {
            _Hunt(name, token);
            return;
        }
        while (!token?.IsCancellationRequested ?? !Manager.ShouldExit)
        {
            string[] names = name.Split('|').Select(x => x.ToLower()).ToArray();
            IOrderedEnumerable<Monster> ordered = Monsters.MapMonsters.OrderBy(x => 0);
            if (priority.HasFlag(HuntPriorities.HighHP))
                ordered = ordered.OrderByDescending(x => x.HP);
            else if (priority.HasFlag(HuntPriorities.LowHP))
                ordered = ordered.OrderBy(x => x.HP);
            if (priority.HasFlag(HuntPriorities.Close))
                ordered = ordered.OrderBy(x => x.Cell == Player.Cell ? 0 : 1);
            List<Monster> targets = ordered.Where(m => names.Any(n => n == "*" || n.Trim().Equals(m.Name.Trim(), StringComparison.OrdinalIgnoreCase)) && m.Alive).ToList();
            foreach (Monster target in targets)
            {
                if (token?.IsCancellationRequested ?? false)
                    break;
                bool sameCell = Monsters.Exists(target.Name);
                if (sameCell || CanJumpForHunt())
                {
                    if (!sameCell && (!token?.IsCancellationRequested ?? true))
                    {
                        if (Player.Cell == target.Cell)
                            continue;
                        Map.Jump(target.Cell, "Left");
                        _lastHuntTick = Environment.TickCount;
                    }
                    Kill.Monster(target.MapID, token);
                    return;
                }
            }
            Thread.Sleep(200);
        }
    }
    private void _HuntWithPriority(int id, HuntPriorities priority, CancellationToken? token)
    {
        if (priority == HuntPriorities.None)
        {
            _Hunt(id, token);
            return;
        }
        while (!token?.IsCancellationRequested ?? !Manager.ShouldExit)
        {
            IOrderedEnumerable<Monster> ordered = Monsters.MapMonsters.OrderBy(x => 0);
            if (priority.HasFlag(HuntPriorities.HighHP))
                ordered = ordered.OrderByDescending(x => x.HP);
            else if (priority.HasFlag(HuntPriorities.LowHP))
                ordered = ordered.OrderBy(x => x.HP);
            if (priority.HasFlag(HuntPriorities.Close))
                ordered = ordered.OrderBy(x => x.Cell == Player.Cell ? 0 : 1);
            List<Monster> targets = ordered.Where(m => m.ID == id && m.Alive).ToList();
            foreach (Monster target in targets)
            {
                if (token?.IsCancellationRequested ?? false)
                    break;
                bool sameCell = Monsters.Exists(target.Name);
                if (sameCell || CanJumpForHunt())
                {
                    if (!sameCell && (!token?.IsCancellationRequested ?? true))
                    {
                        if (Player.Cell == target.Cell)
                            continue;
                        Map.Jump(target.Cell, "Left");
                        _lastHuntTick = Environment.TickCount;
                    }
                    Kill.Monster(target.ID, token);
                    return;
                }
            }
            Thread.Sleep(200);
        }
    }

    private (string name, int quantity, bool isTemp) _item = (string.Empty, 0, false);
    private CancellationTokenSource? _ctsHunt;

    public void ForItem(string name, string item, int quantity, bool tempItem = false)
    {
        _ctsHunt = new();
        _item = (item, quantity, tempItem);
        StrongReferenceMessenger.Default.Register<ScriptHunt, ItemDroppedMessage, int>(this, (int)MessageChannels.GameEvents, ItemHunted);
        while (!Manager.ShouldExit
            && (tempItem || !Inventory.Contains(item, quantity))
            && (!tempItem || !TempInv.Contains(item, quantity)))
        {
            if (_ctsHunt.IsCancellationRequested)
                break;
            _HuntWithPriority(name, Options.HuntPriority, _ctsHunt.Token);
            Drops.Pickup(item);
        }
        StrongReferenceMessenger.Default.Unregister<ItemDroppedMessage, int>(this, (int)MessageChannels.GameEvents);
        _ctsHunt?.Dispose();
        _ctsHunt = null;
    }

    private void ItemHunted(ScriptHunt recipient, ItemDroppedMessage message)
    {
        if (message.Item.Name != recipient._item.name)
            return;

        if (message.AddedToInv && !message.Item.Temp && message.QuantityNow >= recipient._item.quantity)
        {
            recipient._ctsHunt?.Cancel();
            return;
        }
        recipient.Drops.Pickup(message.Item.Name);
        int quant = recipient._item.isTemp ? recipient.TempInv.GetQuantity(message.Item.Name) : recipient.Inventory.GetQuantity(message.Item.Name);
        if (quant >= recipient._item.quantity)
            recipient._ctsHunt?.Cancel();
    }

    public void ForItems(string name, IEnumerable<string> items, IEnumerable<int> quantities, IEnumerable<bool> tempItems)
    {
        if (items.Count() != quantities.Count() || items.Count() != tempItems.Count())
        {
            _logger.ScriptLog("Item count does not match quantity/temp item count.");
            return;
        }
        List<string> itemList = items.ToList();
        List<int> quantList = quantities.ToList();
        List<bool> tempList = tempItems.ToList();
        for (int i = 0; i < itemList.Count; i++)
        {
            ForItem(name, itemList[i], quantList[i], tempList[i]);
        }
    }

    public void ForItem(IEnumerable<string> names, string item, int quantity, bool tempItem = false)
    {
        ForItem(names.JoinWithPipeCharacter(), item, quantity, tempItem);
    }

    public void ForItems(string name, IEnumerable<string> items, IEnumerable<int> quantities, bool tempItems = false)
    {
        ForItems(name, items, quantities, Enumerable.Range(0, items.Count()).Select(i => tempItems));
    }

    public void ForItems(IEnumerable<string> names, IEnumerable<string> items, IEnumerable<int> quantities, IEnumerable<bool> tempItems)
    {
        ForItems(names.JoinWithPipeCharacter(), items, quantities, tempItems);
    }

    public void ForItems(IEnumerable<string> names, IEnumerable<string> items, IEnumerable<int> quantities, bool tempItems = false)
    {
        ForItems(names, items, quantities, Enumerable.Range(0, items.Count()).Select(i => tempItems));
    }

    private bool CanJumpForHunt()
    {
        return Environment.TickCount - _lastHuntTick >= Options.HuntDelay;
    }

    private void ScriptStopped(ScriptHunt recipient, ScriptStoppedMessage message)
    {
        StrongReferenceMessenger.Default.Unregister<ItemDroppedMessage, int>(recipient, (int)MessageChannels.GameEvents);
        recipient._ctsHunt?.Dispose();
        recipient._ctsHunt = null;
    }
}
