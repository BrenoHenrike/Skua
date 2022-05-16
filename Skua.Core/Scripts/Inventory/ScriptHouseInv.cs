using Skua.Core.PostSharp;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

namespace Skua.Core.Scripts;
public class ScriptHouseInv : ScriptableObject, IScriptHouseInv
{
    [ObjectBinding("world.myAvatar.houseitems")]
    public List<InventoryItem> Items { get; } = new();
    [ObjectBinding("world.myAvatar.objData.iHouseSlots")]
    public int Slots { get; }
    [ObjectBinding("world.myAvatar.houseitems.length")]
    public int UsedSlots { get; }

    public bool ToBank(InventoryItem item)
    {
        Bot.Send.Packet($"%xt%zm%bankFromInv%{Bot.Map.RoomID}%{item.ID}%{item.CharItemID}%");
        if (Bot.Options.SafeTimings)
            Bot.Wait.ForHouseInventoryToBank(item.Name);
        return !((IScriptHouseInv)this).Contains(item.Name);
    }

    public void EnsureToBank(string name)
    {
        if (!((IScriptHouseInv)this).TryGetItem(name, out InventoryItem? item))
            return;
        int i = 0;
        while (!ToBank(item!) && !Bot.ShouldExit && Bot.Player.Playing && ++i < Bot.Options.MaximumTries)
            Bot.Sleep(Bot.Options.ActionDelay);
    }

    public void EnsureToBank(int id)
    {
        if (!((IScriptHouseInv)this).TryGetItem(id, out InventoryItem? item))
            return;
        int i = 0;
        while (!ToBank(item!) && !Bot.ShouldExit && Bot.Player.Playing && ++i < Bot.Options.MaximumTries)
            Bot.Sleep(Bot.Options.ActionDelay);
    }
}
