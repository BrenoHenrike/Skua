using Skua.Core.Flash;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

namespace Skua.Core.Scripts;
public partial class ScriptHouseInv : IScriptHouseInv
{
    private readonly Lazy<IFlashUtil> _lazyFlash;
    private readonly Lazy<IScriptSend> _lazySend;
    private readonly Lazy<IScriptOption> _lazyOptions;
    private readonly Lazy<IScriptWait> _lazyWait;
    private readonly Lazy<IScriptMap> _lazyMap;
    private readonly Lazy<IScriptManager> _lazyManager;
    private readonly Lazy<IScriptPlayer> _lazyPlayer;
    private IFlashUtil Flash => _lazyFlash.Value;
    private IScriptOption Options => _lazyOptions.Value;
    private IScriptWait Wait => _lazyWait.Value;
    private IScriptMap Map => _lazyMap.Value;
    private IScriptManager Manager => _lazyManager.Value;
    private IScriptPlayer Player => _lazyPlayer.Value;
    private IScriptSend Send => _lazySend.Value;

    public ScriptHouseInv(
        Lazy<IFlashUtil> flash,
        Lazy<IScriptSend> send,
        Lazy<IScriptOption> options,
        Lazy<IScriptWait> wait,
        Lazy<IScriptMap> map,
        Lazy<IScriptManager> manager,
        Lazy<IScriptPlayer> player)
    {
        _lazyFlash = flash;
        _lazySend = send;
        _lazyOptions = options;
        _lazyWait = wait;
        _lazyMap = map;
        _lazyManager = manager;
        _lazyPlayer = player;
    }

    [ObjectBinding("world.myAvatar.houseitems", Default = "new()")]
    private List<InventoryItem> _items;
    [ObjectBinding("world.myAvatar.objData.iHouseSlots")]
    private int _slots;
    [ObjectBinding("world.myAvatar.houseitems.length")]
    private int _usedSlots;

    public bool ToBank(InventoryItem item)
    {
        Send.Packet($"%xt%zm%bankFromInv%{Map.RoomID}%{item.ID}%{item.CharItemID}%");
        if (Options.SafeTimings)
            Wait.ForHouseInventoryToBank(item.Name);
        return !((IScriptHouseInv)this).Contains(item.Name);
    }

    public bool EnsureToBank(string name)
    {
        if (!((IScriptHouseInv)this).TryGetItem(name, out InventoryItem? item))
            return false;
        int i = 0;
        while (!ToBank(item!) && !Manager.ShouldExit && Player.Playing && ++i < Options.MaximumTries)
            Thread.Sleep(Options.ActionDelay);

        return !((IScriptHouseInv)this).Contains(name);
    }

    public bool EnsureToBank(int id)
    {
        if (!((IScriptHouseInv)this).TryGetItem(id, out InventoryItem? item))
            return false;
        int i = 0;
        while (!ToBank(item!) && !Manager.ShouldExit && Player.Playing && ++i < Options.MaximumTries)
            Thread.Sleep(Options.ActionDelay);

        return !((IScriptHouseInv)this).Contains(id);
    }
}
