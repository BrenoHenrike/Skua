using Skua.Core.Flash;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Items;
using Skua.Core.Models.Shops;
using System.Dynamic;

namespace Skua.Core.Scripts;

public partial class ScriptShop : IScriptShop
{
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
    }

    [ObjectBinding("world.shopinfo.items", Default = "new()")]
    private List<ShopItem> _items = new();
    public bool IsLoaded => !Flash.IsNull("world.shopinfo");
    [ObjectBinding("world.shopinfo.ShopID")]
    private int _ID;
    [ObjectBinding("world.shopinfo.sName", Default = "string.Empty")]
    private string _name;

    public List<ShopInfo> LoadedCache { get; set; } = new();

    public void Load(int id)
    {
        if (Options.SafeTimings)
            Wait.ForActionCooldown(Skua.Core.Models.GameActions.LoadShop);
        Flash.CallGameFunction("world.sendLoadShopRequest", id);
        if (Options.SafeTimings)
            Wait.ForTrue(() => ID == id, 10);
    }
    // TODO Rework BuyItem
    public void BuyItem(string name)
    {
        int index;
        List<ShopItem> items = Items;
        if (IsLoaded && Player.Playing && (index = items.FindIndex(x => x.Name == name)) > -1)
        {
            if (Options.SafeTimings)
            {
                Wait.ForActionCooldown(GameActions.BuyItem);
                Wait.ItemBuyEvent.Reset();
            }
            ExpandoObject item;
            using (FlashArray<ExpandoObject> fItems = (FlashArray<ExpandoObject>)Flash.CreateFlashObject<ExpandoObject>("world.shopinfo.items").ToArray())
            using (FlashObject<ExpandoObject> fItem = (FlashObject<ExpandoObject>)fItems.Get(index))
                item = fItem.Value!;
            Flash.CallGameFunction("world.sendBuyItemRequest", item);
            if (Options.SafeTimings)
                Wait.ForItemBuy();
        }
    }

    public void BuyItem(int id)
    {
        ShopItem? item = Items.Find(i => i.ID == id);
        if (item is null)
            return;
        Send.Packet($"%xt%zm%buyItem%{Map.RoomID}%{item.ID}%{ID}%{item.ShopItemID}%");
    }

    public void SellItem(string name)
    {
        if (Inventory.TryGetItem(name, out InventoryItem? item))
        {
            if (Options.SafeTimings)
            {
                Wait.ForActionCooldown(GameActions.SellItem);
                Wait.ItemSellEvent.Reset();
            }
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
            {
                Wait.ForActionCooldown(GameActions.SellItem);
                Wait.ItemSellEvent.Reset();
            }
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
