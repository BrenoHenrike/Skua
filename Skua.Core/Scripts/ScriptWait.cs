using Microsoft.Toolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models;
using Skua.Core.Models.Items;

namespace Skua.Core.Scripts;

public class ScriptWait : IScriptWait
{
    private readonly Lazy<IFlashUtil> _lazyFlash;
    private readonly Lazy<IScriptPlayer> _lazyPlayer;
    private readonly Lazy<IScriptManager> _lazyManager;
    private readonly Lazy<IScriptInventory> _lazyInventory;
    private readonly Lazy<IScriptCombat> _lazyCombat;
    private readonly Lazy<IScriptMap> _lazyMap;
    private readonly Lazy<IScriptMonster> _lazyMonsters;
    private readonly Lazy<IScriptHouseInv> _lazyHouse;
    private readonly Lazy<IScriptQuest> _lazyQuests;
    private readonly Lazy<IScriptDrop> _lazyDrops;
    private readonly Lazy<IScriptSkill> _lazySkills;
    private readonly IMessenger _messenger;

    private IFlashUtil Flash => _lazyFlash.Value;
    private IScriptPlayer Player => _lazyPlayer.Value;
    private IScriptManager Manager => _lazyManager.Value;
    private IScriptInventory Inventory => _lazyInventory.Value;
    private IScriptCombat Combat => _lazyCombat.Value;
    private IScriptMap Map => _lazyMap.Value;
    private IScriptMonster Monsters => _lazyMonsters.Value;
    private IScriptHouseInv House => _lazyHouse.Value;
    private IScriptQuest Quests => _lazyQuests.Value;
    private IScriptDrop Drops => _lazyDrops.Value;
    private IScriptSkill Skills => _lazySkills.Value;

    public ScriptWait(
        Lazy<IFlashUtil> flash,
        Lazy<IScriptPlayer> player,
        Lazy<IScriptManager> manager,
        Lazy<IScriptMap> map,
        Lazy<IScriptCombat> combat,
        Lazy<IScriptInventory> inventory,
        Lazy<IScriptMonster> monsters,
        Lazy<IScriptHouseInv> house,
        Lazy<IScriptQuest> quests,
        Lazy<IScriptDrop> drops,
        Lazy<IScriptSkill> skills,
        IMessenger messenger)
    {
        _lazyFlash = flash;
        _lazyPlayer = player;
        _lazyManager = manager;
        _lazyInventory = inventory;
        _lazyCombat = combat;
        _lazyMap = map;
        _lazyMonsters = monsters;
        _lazyHouse = house;
        _lazyQuests = quests;
        _lazyDrops = drops;
        _lazySkills = skills;
        _messenger = messenger;

        _messenger.Register<ScriptWait, ItemBoughtMessage>(this, (r, m) => r.ItemBuyEvent.Set());
        _messenger.Register<ScriptWait, ItemSoldMessage>(this, (r, m) => r.ItemSellEvent.Set());
        _messenger.Register<ScriptWait, BankLoadedMessage>(this, (r, m) => r.BankLoadEvent.Set());
    }

    public int WAIT_SLEEP { get; set; } = 250;

    private AutoResetEvent ItemBuyEvent { get; } = new(false);
    private AutoResetEvent ItemSellEvent { get; } = new(false);
    private AutoResetEvent BankLoadEvent { get; } = new(false);



    public bool OverrideTimeout { get; set; } = false;
    public int PlayerActionTimeout { get; set; } = 15;
    public int MonsterActionTimeout { get; set; } = 15;
    public int MapActionTimeout { get; set; } = 25;
    public int DropActionTimeout { get; set; } = 10;
    public int ItemActionTimeout { get; set; } = 14;
    public int QuestActionTimeout { get; set; } = 14;
    public int GameActionTimeout { get; set; } = 40;

    public bool ForPlayerPosition(int x, int y, int timeout = 10)
    {
        return ForTrue(() => !Player.Playing || (Player.X == x && Player.Y == y), OverrideTimeout ? PlayerActionTimeout : timeout);
    }

    public bool ForCombatExit(int timeout = 10)
    {
        return ForTrue(() => !Player.Playing || !Player.InCombat, OverrideTimeout ? PlayerActionTimeout : timeout);
    }

    public bool ForMonsterDeath(int timeout = -1)
    {
        return ForTrue(() => !Player.Playing || !Player.HasTarget, () =>
        {
            Combat.UntargetSelf();
            //Combat.ApproachTarget();
        }, timeout, WAIT_SLEEP / 5);
    }

    public bool ForMonsterSpawn(string name, int timeout = 10)
    {
        return ForTrue(() => !Player.Playing || Monsters.Exists(name), OverrideTimeout ? MonsterActionTimeout : timeout);
    }

    public bool ForMonsterSpawn(int id, int timeout = 10)
    {
        return ForTrue(() => !Player.Playing || Monsters.Exists(id), OverrideTimeout ? MonsterActionTimeout : timeout);
    }

    public bool ForFullyRested(int timeout = -1)
    {
        return ForTrue(() => !Player.Playing || (Player.Health >= Player.MaxHealth && Player.Mana >= Player.MaxMana), timeout);
    }

    public bool ForMapLoad(string name, int timeout = 20)
    {
        string cleanName = name.Split('-')[0].ToLower();
        bool mapNameWait = ForTrue(() => Map.Name == cleanName, OverrideTimeout ? MapActionTimeout : timeout);
        return mapNameWait && ForTrue(() => !Player.Playing || Map.Loaded, OverrideTimeout ? MapActionTimeout : timeout);
    }

    public bool ForCellChange(string name)
    {
        return ForTrue(() => !Player.Playing || Player.Cell == name, OverrideTimeout ? MapActionTimeout : WAIT_SLEEP / 4);
    }

    public bool ForPickup(string name, int timeout = 10)
    {
        return ForTrue(() => !Player.Playing || !Drops.Exists(name), OverrideTimeout ? DropActionTimeout : timeout);
    }

    public bool ForPickup(int id, int timeout = 10)
    {
        return ForTrue(() => !Player.Playing || !Drops.Exists(id), OverrideTimeout ? DropActionTimeout : timeout);
    }

    public bool ForDrop(string name, int timeout = 10)
    {
        return ForTrue(() => !Player.Playing || Drops.Exists(name), OverrideTimeout ? DropActionTimeout : timeout);
    }

    public bool ForDrop(int id, int timeout = 10)
    {
        return ForTrue(() => !Player.Playing || Drops.Exists(id), OverrideTimeout ? DropActionTimeout : timeout);
    }

    public bool ForItemBuy(int timeout = 10)
    {
        return ItemBuyEvent.WaitOne((OverrideTimeout ? ItemActionTimeout : timeout) * WAIT_SLEEP);
    }

    public bool ForItemSell(int timeout = 10)
    {
        return ItemSellEvent.WaitOne((OverrideTimeout ? ItemActionTimeout : timeout) * WAIT_SLEEP);
    }

    public bool ForItemEquip(int id, int timeout = 10)
    {
        return ForTrue(() => !Player.Playing || (Inventory.TryGetItem(id, out InventoryItem? i) && i!.Equipped), OverrideTimeout ? ItemActionTimeout : timeout);
    }

    public bool ForItemEquip(string name, int timeout = 10)
    {
        return ForTrue(() => !Player.Playing || !Inventory.TryGetItem(name, out InventoryItem? i) || i!.Equipped, OverrideTimeout ? ItemActionTimeout : timeout);
    }

    public bool ForBankToInventory(string name, int timeout = 14)
    {
        return ForTrue(() => !Player.Playing || Inventory.Contains(name), OverrideTimeout ? ItemActionTimeout : timeout, WAIT_SLEEP / 2);
    }

    public bool ForBankToInventory(int id, int timeout = 14)
    {
        return ForTrue(() => !Player.Playing || Inventory.Contains(id), OverrideTimeout ? ItemActionTimeout : timeout, WAIT_SLEEP / 2);
    }

    public bool ForBankToHouseInventory(string name, int timeout = 14)
    {
        return ForTrue(() => !Player.Playing || House.Contains(name), OverrideTimeout ? ItemActionTimeout : timeout, WAIT_SLEEP / 2);
    }

    public bool ForBankToHouseInventory(int id, int timeout = 14)
    {
        return ForTrue(() => !Player.Playing || House.Contains(id), OverrideTimeout ? ItemActionTimeout : timeout, WAIT_SLEEP / 2);
    }

    public bool ForInventoryToBank(string name, int timeout = 14)
    {
        return ForTrue(() => !Player.Playing || !Inventory.Contains(name), OverrideTimeout ? ItemActionTimeout : timeout, WAIT_SLEEP / 2);
    }

    public bool ForInventoryToBank(int id, int timeout = 14)
    {
        return ForTrue(() => !Player.Playing || !Inventory.Contains(id), OverrideTimeout ? ItemActionTimeout : timeout, WAIT_SLEEP / 2);
    }

    public bool ForHouseInventoryToBank(string name, int timeout = 14)
    {
        return ForTrue(() => !Player.Playing || !House.Contains(name), OverrideTimeout ? ItemActionTimeout : timeout, WAIT_SLEEP);
    }

    public bool ForHouseInventoryToBank(int id, int timeout = 14)
    {
        return ForTrue(() => !Player.Playing || !House.Contains(id), OverrideTimeout ? ItemActionTimeout : timeout, WAIT_SLEEP);
    }

    public bool ForBankLoad(int timeout = 20)
    {
        return BankLoadEvent.WaitOne(timeout * WAIT_SLEEP);
    }

    public bool ForQuestAccept(int id, int timeout = 14)
    {
        return ForTrue(() => !Player.Playing || Quests.IsInProgress(id), OverrideTimeout ? QuestActionTimeout : timeout, WAIT_SLEEP / 2);
    }

    public bool ForQuestComplete(int id, int timeout = 10)
    {
        return ForTrue(() => !Player.Playing || !Quests.IsInProgress(id), OverrideTimeout ? QuestActionTimeout : timeout, WAIT_SLEEP / 2);
    }

    public bool ForSkillCooldown(int index, int timeout = 50)
    {
        return ForTrue(() => Skills.CanUseSkill(index), timeout, WAIT_SLEEP);
    }

    public bool For(Func<object> function, object value, int timeout = 10)
    {
        return ForTrue(() => function() == value, timeout);
    }

    public async ValueTask<bool> ForAsync(Func<object> function, object value, int timeout = 10, CancellationToken token = default)
    {
        return await ForTrueAsync(() => function() == value, timeout, token: token);
    }

    public bool ForTrue(Func<bool> predicate, int timeout, int sleepOverride = -1)
    {
        return ForTrue(predicate, null, timeout, sleepOverride);
    }

    public bool ForTrue(Func<bool> predicate, Action? loopFunction, int sleepOverride, CancellationToken token)
    {
        int counter = 0;
        while (!predicate() && !Manager.ShouldExit && !token.IsCancellationRequested)
        {
            loopFunction?.Invoke();
            if (token.IsCancellationRequested)
                break;
            Thread.Sleep(sleepOverride == -1 ? WAIT_SLEEP : sleepOverride);
            counter++;
        }
        return true;
    }

    public async ValueTask<bool> ForTrueAsync(Func<bool> predicate, int timeout, int sleepOverride = -1, CancellationToken token = default)
    {
        return await ForTrueAsync(predicate, null, timeout, sleepOverride, token);
    }

    public bool ForTrue(Func<bool> predicate, Action? loopFunction, int timeout, int sleepOverride = -1)
    {
        int counter = 0;
        while (!predicate() && !Manager.ShouldExit)
        {
            if (timeout > 0 && counter >= timeout)
                return false;
            loopFunction?.Invoke();
            Thread.Sleep(sleepOverride == -1 ? WAIT_SLEEP : sleepOverride);
            counter++;
        }
        return true;
    }

    public async ValueTask<bool> ForTrueAsync(Func<bool> predicate, Action? loopFunction, int timeout, int sleepOverride = -1, CancellationToken token = default)
    {
        try
        {
            int counter = 0;
            while (!predicate() && !Manager.ShouldExit && !token.IsCancellationRequested)
            {
                if (timeout > 0 && counter >= timeout)
                    return false;
                loopFunction?.Invoke();
                await Task.Delay(sleepOverride == -1 ? WAIT_SLEEP : sleepOverride, token);
                counter++;
            }
            return true;
        }
        catch { }
        return false;
    }

    public bool ForActionCooldown(GameActions action, int timeout = 40)
    {
        return ForActionCooldown(lockedActions[action], OverrideTimeout ? GameActionTimeout : timeout);
    }

    public bool ForActionCooldown(string action, int timeout = 40)
    {
        return ForTrue(() => IsActionAvailable(action), OverrideTimeout ? GameActionTimeout : timeout);
    }

    public bool IsActionAvailable(GameActions action)
    {
        return IsActionAvailable(lockedActions[action]);
    }

    public bool IsActionAvailable(string action)
    {
        if (!Player.Playing)
            return false;
        long time = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
        GameActionLock locked = JsonConvert.DeserializeObject<GameActionLock>(Flash.GetGameObject($"world.lock.{action}")!);
        return time - locked.TS >= locked.CD;
    }

    private readonly Dictionary<GameActions, string> lockedActions = new()
    {
        { GameActions.LoadShop, "loadShop" },
        { GameActions.LoadEnhShop, "loadEnhShop" },
        { GameActions.LoadHairShop, "loadHairShop" },
        { GameActions.EquipItem, "equipItem" },
        { GameActions.UnequipItem, "unequipItem" },
        { GameActions.BuyItem, "buyItem" },
        { GameActions.SellItem, "sellItem" },
        { GameActions.GetMapItem, "getMapItem" },
        { GameActions.TryQuestComplete, "tryQuestComplete" },
        { GameActions.AcceptQuest, "acceptQuest" },
        { GameActions.DoIA, "doIA" },
        { GameActions.Rest, "rest" },
        { GameActions.Who, "who" },
        { GameActions.Transfer, "tfer" }
    };
}
