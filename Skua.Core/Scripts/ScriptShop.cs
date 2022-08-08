using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Flash;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models;
using Skua.Core.Models.Items;
using Skua.Core.Models.Shops;

namespace Skua.Core.Scripts;

public partial class ScriptShop : IScriptShop
{
    public ScriptShop(
        Lazy<IFlashUtil> flash,
        Lazy<IScriptOption> options,
        Lazy<IScriptWait> wait,
        Lazy<IScriptMap> map,
        Lazy<IScriptPlayer> player,
        Lazy<IScriptSend> send,
        Lazy<IScriptInventory> inventory)
    {
        _lazyFlash = flash;
        _lazyOptions = options;
        _lazyWait = wait;
        _lazyMap = map;
        _lazyPlayer = player;
        _lazySend = send;
        _lazyInventory = inventory;

        StrongReferenceMessenger.Default.Register<ScriptShop, ShopLoadedMessage, int>(this, (int)MessageChannels.GameEvents, ShopLoaded);
    }

    private void ShopLoaded(ScriptShop recipient, ShopLoadedMessage message)
    {
        if (!recipient.LoadedCache.Contains(message.Info))
            recipient.LoadedCache.Add(message.Info);
    }

    private readonly Lazy<IFlashUtil> _lazyFlash;
    private readonly Lazy<IScriptOption> _lazyOptions;
    private readonly Lazy<IScriptWait> _lazyWait;
    private readonly Lazy<IScriptMap> _lazyMap;
    private readonly Lazy<IScriptPlayer> _lazyPlayer;
    private readonly Lazy<IScriptSend> _lazySend;
    private readonly Lazy<IScriptInventory> _lazyInventory;

    private IFlashUtil Flash => _lazyFlash.Value;
    private IScriptOption Options => _lazyOptions.Value;
    private IScriptWait Wait => _lazyWait.Value;
    private IScriptMap Map => _lazyMap.Value;
    private IScriptPlayer Player => _lazyPlayer.Value;
    private IScriptSend Send => _lazySend.Value;
    private IScriptInventory Inventory => _lazyInventory.Value;

    [ObjectBinding("world.shopinfo.items", Default = "new()")]
    private List<ShopItem> _items = new();
    public bool IsLoaded => !Flash.IsNull("world.shopinfo");
    [ObjectBinding("world.shopinfo.ShopID")]
    private int _ID;
    [ObjectBinding("world.shopinfo.sName", Default = "string.Empty")]
    private string _name = string.Empty;

    public List<ShopInfo> LoadedCache { get; set; } = new();

    public void Load(int id)
    {
        if (Options.SafeTimings)
            Wait.ForActionCooldown(GameActions.LoadShop);
        Flash.CallGameFunction("world.sendLoadShopRequest", id);
        if (Options.SafeTimings)
            Wait.ForTrue(() => ID == id, 10);
    }

    public void BuyItem(string name)
    {
        if (Options.SafeTimings)
            Wait.ForActionCooldown(GameActions.BuyItem);
        Flash.Call("buyItemByName", name);
        if (Options.SafeTimings)
            Wait.ForItemBuy();
    }

    public void BuyItem(int id, int shopItemId = 0)
    {
        if (Options.SafeTimings)
            Wait.ForActionCooldown(GameActions.BuyItem);
        Flash.Call("buyItemByID", id, shopItemId);
        if (Options.SafeTimings)
            Wait.ForItemBuy();
    }

    public void SellItem(string name)
    {
        if (Inventory.TryGetItem(name, out InventoryItem? item))
        {
            if (Options.SafeTimings)
                Wait.ForActionCooldown(GameActions.SellItem);
            Send.Packet($"%xt%zm%sellItem%{Map.RoomID}%{item!.ID}%{item!.Quantity}%{item!.CharItemID}%");
            if (Options.SafeTimings)
                Wait.ForItemSell();
        }
    }

    public void SellItem(int id)
    {
        if (Inventory.TryGetItem(id, out InventoryItem? item))
        {
            if (Options.SafeTimings)
                Wait.ForActionCooldown(GameActions.SellItem);
            Send.Packet($"%xt%zm%sellItem%{Map.RoomID}%{item!.ID}%{item.Quantity}%{item.CharItemID}%");
            if (Options.SafeTimings)
                Wait.ForItemSell();
        }
    }

    [MethodCallBinding("world.sendLoadHairShopRequest", RunMethodPre = true, GameFunction = true)]
    private void _loadHairShop(int id)
    {
        if (Options.SafeTimings)
            Wait.ForActionCooldown(Skua.Core.Models.GameActions.LoadHairShop);
    }

    [MethodCallBinding("openArmorCustomize", GameFunction = true)]
    private void _loadArmourCustomizer() { }
}
