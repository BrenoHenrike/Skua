using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.PostSharp;

namespace Skua.Core.Scripts;
public class ScriptBank : ScriptableObject, IScriptBank
{
    public bool Loaded { get; set; } = false;
    [ObjectBinding("world.bankinfo.items")]
    public List<InventoryItem> Items { get; } = new();
    [ObjectBinding("world.myAvatar.objData.iBankSlots")]
    public int Slots { get; }
    [ObjectBinding("world.myAvatar.iBankCount")]
    public int UsedSlots { get; }

    [MethodCallBinding("world.toggleBank", GameFunction = true)]
    public void Open() { }

    public void Load(bool waitForLoad = true)
    {
        Bot.Send.Packet($"%xt%zm%loadBank%{Bot.Map.RoomID}%All%");
        if (waitForLoad)
            Bot.Wait.ForBankLoad(20);
    }

    public bool Swap(string invItem, string bankItem)
    {
        if (((IScriptBank)this).TryGetItem(bankItem, out InventoryItem? bank) && Bot.Inventory.TryGetItem(invItem, out InventoryItem? inv))
            Swap(bank!, inv!);
        return false;
    }

    public bool Swap(int invItem, int bankItem)
    {
        if (((IScriptBank)this).TryGetItem(bankItem, out InventoryItem? bank) && Bot.Inventory.TryGetItem(invItem, out InventoryItem? inv))
            return Swap(bank!, inv!);
        return false;
    }

    public bool Swap(InventoryItem bankItem, InventoryItem invItem)
    {
        Bot.Send.Packet($"%xt%zm%bankSwapInv%{Bot.Map.RoomID}%{invItem.ID}%{invItem.CharItemID}%{bankItem.ID}%{bankItem.CharItemID}%");
        if (Bot.Options.SafeTimings)
            Bot.Wait.ForInventoryToBank(invItem.ID);
        return Bot.Inventory.Contains(bankItem.ID);
    }

    public void EnsureSwap(string invItem, string bankItem, bool loadBank = true)
    {
        if (loadBank)
            Load();
        int i = 0;
        while (!Swap(invItem, bankItem) && !Bot.ShouldExit && Bot.Player.Playing && ++i < Bot.Options.MaximumTries)
            Bot.Sleep(Bot.Options.ActionDelay);
    }

    public void EnsureSwap(int invItem, int bankItem, bool loadBank = true)
    {
        if (loadBank)
            Load();
        int i = 0;
        while (!Swap(invItem, bankItem) && !Bot.ShouldExit && Bot.Player.Playing && ++i < Bot.Options.MaximumTries)
            Bot.Sleep(Bot.Options.ActionDelay);
    }

    public bool ToInventory(InventoryItem item)
    {
        Bot.Send.Packet($"%xt%zm%bankToInv%{Bot.Map.RoomID}%{item.ID}%{item.CharItemID}%");
        if (Bot.Options.SafeTimings)
            Bot.Wait.ForBankToInventory(item!.Name);
        return Bot.Inventory.Contains(item.ID);
    }

    public void EnsureToInventory(string item, bool loadBank = true)
    {
        if (loadBank)
            Load();
        int i = 0;
        while (!((IScriptBank)this).ToInventory(item) && !Bot.ShouldExit && Bot.Player.Playing && ++i < Bot.Options.MaximumTries)
            Bot.Sleep(Bot.Options.ActionDelay);
    }

    public void EnsureToInventory(int id, bool loadBank = true)
    {
        if (loadBank)
            Load();
        int i = 0;
        while (!((IScriptBank)this).ToInventory(id) && !Bot.ShouldExit && Bot.Player.Playing && ++i < Bot.Options.MaximumTries)
            Bot.Sleep(Bot.Options.ActionDelay);
    }
}
