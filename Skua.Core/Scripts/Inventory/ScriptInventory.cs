using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Items;
using Skua.Core.PostSharp;
using System.Dynamic;

namespace Skua.Core.Scripts;

public class ScriptInventory : ScriptableObject, IScriptInventory
{
    [ObjectBinding("world.myAvatar.items")]
    public List<InventoryItem> Items { get; } = new();
    [ObjectBinding("world.myAvatar.objData.iBagSlots")]
    public int Slots { get; }
    [ObjectBinding("world.myAvatar.items.length")]
    public int UsedSlots { get; }

    public void EquipItem(int id)
    {
        if (Bot.Options.SafeTimings)
            Bot.Wait.ForActionCooldown(GameActions.EquipItem);
        dynamic item = new ExpandoObject();
        item.ItemID = id;
        Bot.Flash.CallGameFunction("world.sendEquipItemRequest", item);
        if (Bot.Options.SafeTimings)
            Bot.Wait.ForItemEquip(id);
    }

    public bool ToBank(InventoryItem item)
    {
        Bot.Send.Packet($"%xt%zm%bankFromInv%{Bot.Map.RoomID}%{item.ID}%{item.CharItemID}%");
        if (Bot.Options.SafeTimings)
            Bot.Wait.ForInventoryToBank(item.Name);
        return !((IScriptInventory)this).Contains(item.Name);
    }

    public void EnsureToBank(string name)
    {
        if (!((IScriptInventory)this).TryGetItem(name, out InventoryItem? item))
            return;
        int i = 0;
        while (!ToBank(item!) && !Bot.ShouldExit && Bot.Player.Playing && ++i < Bot.Options.MaximumTries)
            Bot.Sleep(Bot.Options.ActionDelay);
    }

    public void EnsureToBank(int id)
    {
        if (!((IScriptInventory)this).TryGetItem(id, out InventoryItem? item))
            return;
        int i = 0;
        while (!ToBank(item!) && !Bot.ShouldExit && Bot.Player.Playing && ++i < Bot.Options.MaximumTries)
            Bot.Sleep(Bot.Options.ActionDelay);
    }
}
