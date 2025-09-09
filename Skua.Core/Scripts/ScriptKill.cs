using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models.Monsters;
using Skua.Core.Utils;

namespace Skua.Core.Scripts;

public class ScriptKill : IScriptKill
{
    public ScriptKill(Lazy<IScriptWait> wait,
        Lazy<IScriptOption> options,
        Lazy<IScriptCombat> combat,
        Lazy<IScriptPlayer> player,
        Lazy<IScriptDrop> drops,
        Lazy<IScriptInventory> inventory,
        Lazy<IScriptTempInv> tempInv,
        Lazy<IScriptManager> manager,
        Lazy<IScriptMap> map,
        ILogService logger,
        Lazy<IScriptMonster> monsters)
    {
        _lazyWait = wait;
        _lazyOptions = options;
        _lazyCombat = combat;
        _lazyPlayer = player;
        _lazyDrops = drops;
        _lazyInventory = inventory;
        _lazyTempInv = tempInv;
        _lazyManager = manager;
        _lazyMap = map;
        _logger = logger;
        _lazyMonsters = monsters;
    }

    internal string _saveCell = "Enter", _savePad = "Spawn";
    private readonly Lazy<IScriptWait> _lazyWait;
    private readonly Lazy<IScriptOption> _lazyOptions;
    private readonly Lazy<IScriptCombat> _lazyCombat;
    private readonly Lazy<IScriptPlayer> _lazyPlayer;
    private readonly Lazy<IScriptDrop> _lazyDrops;
    private readonly Lazy<IScriptInventory> _lazyInventory;
    private readonly Lazy<IScriptTempInv> _lazyTempInv;
    private readonly Lazy<IScriptManager> _lazyManager;
    private readonly Lazy<IScriptMap> _lazyMap;
    private readonly Lazy<IScriptMonster> _lazyMonsters;
    private readonly ILogService _logger;

    private IScriptWait Wait => _lazyWait.Value;
    private IScriptOption Options => _lazyOptions.Value;
    private IScriptCombat Combat => _lazyCombat.Value;
    private IScriptPlayer _player => _lazyPlayer.Value;
    private IScriptDrop Drops => _lazyDrops.Value;
    private IScriptInventory Inventory => _lazyInventory.Value;
    private IScriptTempInv TempInv => _lazyTempInv.Value;
    private IScriptMap Map => _lazyMap.Value;
    private IScriptManager Manager => _lazyManager.Value;
    private IScriptMonster Monsters => _lazyMonsters.Value;

    public void Player(string name)
    {
        Combat.AttackPlayer(name);
        if (Options.SafeTimings)
            Wait.ForMonsterDeath();
    }

    public void Monster(string name) => _Kill(name, null);

    public void Monster(string name, CancellationToken? token) => _Kill(name, token);

    public void Monster(Monster monster) => _Kill(monster.MapID, null);

    public void Monster(Monster monster, CancellationToken? token) => _Kill(monster.MapID, token);

    public void Monster(int id) => _Kill(id, null);

    public void Monster(int id, CancellationToken? token) => _Kill(id, token);

    private void _Kill(string name, CancellationToken? token)
    {
        if (Options.SafeTimings)
            Wait.ForMonsterSpawn(name);
        if (Combat.Attack(name))
        {
            Thread.Sleep(Options.ActionDelay);
            if (token is null)
            {
                Wait.ForMonsterDeath();
                return;
            }
            WaitMonsterDeathOrCancellation((CancellationToken)token);
        }
    }

    private void _Kill(int id, CancellationToken? token)
    {
        if (Options.SafeTimings)
            Wait.ForMonsterSpawn(id);
        if (Combat.Attack(id))
        {
            Thread.Sleep(Options.ActionDelay);
            if (token is null)
            {
                Wait.ForMonsterDeath();
                return;
            }
            WaitMonsterDeathOrCancellation((CancellationToken)token);
        }
    }

    private (string name, int quantity, bool isTemp) _item = (string.Empty, 0, false);
    private CancellationTokenSource? _ctsKill;

    public void ForItem(string name, string item, int quantity, bool tempItem = false)
    {
        if ((!tempItem && Inventory.Contains(item, quantity)) || (tempItem && TempInv.Contains(item, quantity)))
            return;

        _saveCell = _player.Cell;
        _savePad = _player.Pad;
        _ctsKill = new();
        _item = (item, quantity, tempItem);
        string[] names = name.Split('|');

        StrongReferenceMessenger.Default.Register<ScriptKill, CellChangedMessage, int>(this, (int)MessageChannels.GameEvents, CellChanged);
        StrongReferenceMessenger.Default.Register<ScriptKill, ItemDroppedMessage, int>(this, (int)MessageChannels.GameEvents, ItemDropped);
        try
        {
            if (name == "*")
            {
                while (!Manager.ShouldExit && !_ctsKill.IsCancellationRequested)
                {
                    foreach (var mon in Monsters.CurrentAvailableMonsters)
                    {
                        if (_ctsKill.IsCancellationRequested)
                            break;

                        Monster(mon, _ctsKill.Token);
                        Thread.Sleep(Options.ActionDelay);
                    }
                }
            }
            else
            {
                while (!Manager.ShouldExit && !_ctsKill.IsCancellationRequested)
                {
                    foreach (var mon in names)
                    {
                        if (_ctsKill.IsCancellationRequested)
                            break;

                        Monster(mon, _ctsKill.Token);
                        Thread.Sleep(Options.ActionDelay);
                    }
                }
            }
        }
        finally
        {
            _saveCell = _savePad = string.Empty;
            _ctsKill?.Dispose();
            _ctsKill = null;
            StrongReferenceMessenger.Default.Unregister<CellChangedMessage, int>(this, (int)MessageChannels.GameEvents);
            StrongReferenceMessenger.Default.Unregister<ItemDroppedMessage, int>(this, (int)MessageChannels.GameEvents);
        }

        void CellChanged(ScriptKill recipient, CellChangedMessage message)
        {
            if (message.Cell != recipient._saveCell)
                recipient.Map.Jump(recipient._saveCell, recipient._savePad);
        }
    }

    private void ItemDropped(ScriptKill recipient, ItemDroppedMessage message)
    {
        if (message.Item.Name != recipient._item.name)
            return;

        if (message.AddedToInv && !message.Item.Temp && message.QuantityNow >= recipient._item.quantity)
        {
            recipient._ctsKill?.Cancel();
            return;
        }
        recipient.Drops.Pickup(message.Item.Name);
        int quant = recipient._item.isTemp ? recipient.TempInv.GetQuantity(message.Item.Name) : recipient.Inventory.GetQuantity(message.Item.Name);
        if (quant >= recipient._item.quantity)
            recipient._ctsKill?.Cancel();
    }

    public void ForItem(IEnumerable<string> names, string item, int quantity, bool tempItem = false)
    {
        ForItem(names.JoinWithPipeCharacter(), item, quantity, tempItem);
    }

    public void ForItems(string name, IEnumerable<string> items, IEnumerable<int> quantities, bool tempItems = false)
    {
        ForItems(name, items, quantities, Enumerable.Range(0, items.Count()).Select(i => tempItems));
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

    public void ForItems(IEnumerable<string> names, IEnumerable<string> items, IEnumerable<int> quantities, bool tempItems = false)
    {
        ForItems(names.JoinWithPipeCharacter(), items, quantities, Enumerable.Range(0, items.Count()).Select(i => tempItems));
    }

    public void ForItems(IEnumerable<string> names, IEnumerable<string> items, IEnumerable<int> quantities, IEnumerable<bool> tempItems)
    {
        ForItems(names.JoinWithPipeCharacter(), items, quantities, tempItems);
    }

    private void WaitMonsterDeathOrCancellation(CancellationToken token)
    {
        Wait.ForTrue(() => !_player.Playing || !_player.HasTarget, () => Combat.UntargetSelf(), -1, token);
        Combat.CancelAutoAttack();
        Combat.CancelTarget();
    }
}