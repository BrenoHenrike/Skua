using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Items;

namespace Skua.Core.Scripts;

public class ScriptWait : ScriptableObject, IScriptWait
{
    public int WAIT_SLEEP { get; set; } = 250;

    public AutoResetEvent ItemBuyEvent => new(false);
    public AutoResetEvent ItemSellEvent => new(false);
    public AutoResetEvent BankLoadEvent => new(false);

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
        return ForTrue(() => !Bot.Player.Playing || (Bot.Player.X == x && Bot.Player.Y == y), OverrideTimeout ? PlayerActionTimeout : timeout);
    }

    public bool ForCombatExit(int timeout = 10)
    {
        return ForTrue(() => !Bot.Player.Playing || !Bot.Player.InCombat, OverrideTimeout ? PlayerActionTimeout : timeout);
    }

    public bool ForMonsterDeath(int timeout = -1)
    {
        return ForTrue(() => !Bot.Player.Playing || !Bot.Player.HasTarget, () =>
        {
            Bot.Combat.UntargetSelf();
            Bot.Combat.ApproachTarget();
        }, timeout, WAIT_SLEEP / 5);
    }

    public bool ForMonsterSpawn(string name, int timeout = 10)
    {
        return ForTrue(() => !Bot.Player.Playing || Bot.Monsters.Exists(name), OverrideTimeout ? MonsterActionTimeout : timeout);
    }

    public bool ForMonsterSpawn(int id, int timeout = 10)
    {
        return ForTrue(() => !Bot.Player.Playing || Bot.Monsters.Exists(id), OverrideTimeout ? MonsterActionTimeout : timeout);
    }

    public bool ForFullyRested(int timeout = -1)
    {
        return ForTrue(() => !Bot.Player.Playing || (Bot.Player.Health >= Bot.Player.MaxHealth && Bot.Player.Mana >= Bot.Player.MaxMana), timeout);
    }

    public bool ForMapLoad(string name, int timeout = 20)
    {
        string cleanName = name.Split('-')[0].ToLower();
        bool mapNameWait = ForTrue(() => Bot.Map.Name == cleanName, OverrideTimeout ? MapActionTimeout : timeout);
        return mapNameWait && ForTrue(() => !Bot.Player.Playing || Bot.Map.Loaded, OverrideTimeout ? MapActionTimeout : timeout);
    }

    public bool ForCellChange(string name)
    {
        return ForTrue(() => !Bot.Player.Playing || Bot.Player.Cell == name, OverrideTimeout ? MapActionTimeout : WAIT_SLEEP / 4);
    }

    public bool ForPickup(string name, int timeout = 10)
    {
        return ForTrue(() => !Bot.Player.Playing || !Bot.Drops.Exists(name), OverrideTimeout ? DropActionTimeout : timeout);
    }

    public bool ForPickup(int id, int timeout = 10)
    {
        return ForTrue(() => !Bot.Player.Playing || !Bot.Drops.Exists(id), OverrideTimeout ? DropActionTimeout : timeout);
    }

    public bool ForDrop(string name, int timeout = 10)
    {
        return ForTrue(() => !Bot.Player.Playing || Bot.Drops.Exists(name), OverrideTimeout ? DropActionTimeout : timeout);
    }

    public bool ForDrop(int id, int timeout = 10)
    {
        return ForTrue(() => !Bot.Player.Playing || Bot.Drops.Exists(id), OverrideTimeout ? DropActionTimeout : timeout);
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
        return ForTrue(() => !Bot.Player.Playing || (Bot.Inventory.TryGetItem(id, out InventoryItem? i) && i!.Equipped), OverrideTimeout ? ItemActionTimeout : timeout);
    }

    public bool ForItemEquip(string name, int timeout = 10)
    {
        return ForTrue(() => !Bot.Player.Playing || !Bot.Inventory.TryGetItem(name, out InventoryItem? i) || i!.Equipped, OverrideTimeout ? ItemActionTimeout : timeout);
    }

    public bool ForBankToInventory(string name, int timeout = 14)
    {
        return ForTrue(() => !Bot.Player.Playing || Bot.Inventory.Contains(name), OverrideTimeout ? ItemActionTimeout : timeout, WAIT_SLEEP / 2);
    }

    public bool ForBankToInventory(int id, int timeout = 14)
    {
        return ForTrue(() => !Bot.Player.Playing || Bot.Inventory.Contains(id), OverrideTimeout ? ItemActionTimeout : timeout, WAIT_SLEEP / 2);
    }

    public bool ForBankToHouseInventory(string name, int timeout = 14)
    {
        return ForTrue(() => !Bot.Player.Playing || Bot.House.Contains(name), OverrideTimeout ? ItemActionTimeout : timeout, WAIT_SLEEP / 2);
    }

    public bool ForBankToHouseInventory(int id, int timeout = 14)
    {
        return ForTrue(() => !Bot.Player.Playing || Bot.House.Contains(id), OverrideTimeout ? ItemActionTimeout : timeout, WAIT_SLEEP / 2);
    }

    public bool ForInventoryToBank(string name, int timeout = 14)
    {
        return ForTrue(() => !Bot.Player.Playing || !Bot.Inventory.Contains(name), OverrideTimeout ? ItemActionTimeout : timeout, WAIT_SLEEP / 2);
    }

    public bool ForInventoryToBank(int id, int timeout = 14)
    {
        return ForTrue(() => !Bot.Player.Playing || !Bot.Inventory.Contains(id), OverrideTimeout ? ItemActionTimeout : timeout, WAIT_SLEEP / 2);
    }

    public bool ForHouseInventoryToBank(string name, int timeout = 14)
    {
        return ForTrue(() => !Bot.Player.Playing || !Bot.House.Contains(name), OverrideTimeout ? ItemActionTimeout : timeout, WAIT_SLEEP);
    }

    public bool ForHouseInventoryToBank(int id, int timeout = 14)
    {
        return ForTrue(() => !Bot.Player.Playing || !Bot.House.Contains(id), OverrideTimeout ? ItemActionTimeout : timeout, WAIT_SLEEP);
    }

    public bool ForBankLoad(int timeout = 20)
    {
        return BankLoadEvent.WaitOne(timeout * WAIT_SLEEP);
    }

    public bool ForQuestAccept(int id, int timeout = 14)
    {
        return ForTrue(() => !Bot.Player.Playing || Bot.Quests.IsInProgress(id), OverrideTimeout ? QuestActionTimeout : timeout, WAIT_SLEEP / 2);
    }

    public bool ForQuestComplete(int id, int timeout = 10)
    {
        return ForTrue(() => !Bot.Player.Playing || !Bot.Quests.IsInProgress(id), OverrideTimeout ? QuestActionTimeout : timeout, WAIT_SLEEP / 2);
    }

    public bool ForSkillCooldown(int index, int timeout = 50)
    {
        return ForTrue(() => Bot.Skills.CanUseSkill(index), timeout, WAIT_SLEEP);
    }

    public bool For(Func<object> function, object value, int timeout = 10)
    {
        return ForTrue(() => function() == value, timeout);
    }

    public bool ForTrue(Func<bool> predicate, int timeout, int sleepOverride = -1)
    {
        return ForTrue(predicate, null, timeout, sleepOverride);
    }

    public bool ForTrue(Func<bool> predicate, Action? loopFunction, int timeout, int sleepOverride = -1)
    {
        if (timeout <= 0)
            return predicate();
        for (int counter = 0; counter < timeout; counter++)
        {
            if (predicate())
                return true;
            if (timeout > 0 && counter >= timeout)
                return false;
            loopFunction?.Invoke();
            Thread.Sleep(sleepOverride == -1 ? WAIT_SLEEP : sleepOverride);
            counter++;
        }
        if (Bot.ShouldExit)
            Thread.Sleep(1000);
        return true;
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
        if (!Bot.Player.Playing)
            return false;
        long time = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
        GameActionLock locked = JsonConvert.DeserializeObject<GameActionLock>(Bot.Flash.GetGameObject($"world.lock.{action}")!);
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
