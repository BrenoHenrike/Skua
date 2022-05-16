using Skua.Core.Flash;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Items;
using Skua.Core.Models.Shops;
using Skua.Core.PostSharp;
using System.Dynamic;

namespace Skua.Core.Scripts;

public class ScriptShop : ScriptableObject, IScriptShop
{
    [ObjectBinding("world.shopinfo.items")]
    public List<ShopItem> Items { get; } = new();
    public bool IsLoaded => !Bot.Flash.IsNull("world.shopinfo");
    [ObjectBinding("world.shopinfo.ShopID")]
    public int ID { get; }
    [ObjectBinding("world.shopinfo.sName")]
    public string Name { get; } = string.Empty;
    public List<ShopInfo> LoadedCache { get; set; } = new();

    public void Load(int id)
    {
        if (Bot.Options.SafeTimings)
            Bot.Wait.ForActionCooldown(GameActions.LoadShop);
        Bot.Flash.CallGameFunction("world.sendLoadShopRequest", id);
        if (Bot.Options.SafeTimings)
            Bot.Wait.ForTrue(() => ID == id, 10);
    }
    // TODO Rework BuyItem
    public void BuyItem(string name)
    {
        int index;
        List<ShopItem> items = Items;
        if (IsLoaded && (index = items.FindIndex(x => x.Name == name)) > -1)
        {
            if (Bot.Options.SafeTimings)
            {
                Bot.Wait.ForActionCooldown(GameActions.BuyItem);
                Bot.Wait.ItemBuyEvent.Reset();
            }
            ExpandoObject item;
            using (var fItems = (FlashArray<ExpandoObject>)Bot.Flash.CreateFlashObject<ExpandoObject>("world.shopinfo.items").ToArray())
            using (FlashObject<ExpandoObject> fItem = (FlashObject<ExpandoObject>)fItems.Get(index))
                item = fItem.Value;
            Bot.Flash.CallGameFunction("world.sendBuyItemRequest", item);
            if (Bot.Options.SafeTimings)
                Bot.Wait.ForItemBuy();
        }
    }

    public void BuyItem(int id)
    {
        var item = Items.Find(i => i.ID == id);
        Bot.Send.Packet($"%xt%zm%buyItem%{Bot.Map.RoomID}%{item.ID}%{ID}%{item.ShopItemID}%");
    }

    public void SellItem(string name)
    {
        if (Bot.Inventory.TryGetItem(name, out InventoryItem? item))
        {
            if (Bot.Options.SafeTimings)
            {
                Bot.Wait.ForActionCooldown(GameActions.SellItem);
                Bot.Wait.ItemSellEvent.Reset();
            }
            Bot.Send.Packet($"%xt%zm%sellItem%{Bot.Map.RoomID}%{item!.ID}%{item.Quantity}%{item.CharItemID}%");
            if (Bot.Options.SafeTimings)
                Bot.Wait.ForItemSell();
        }
    }

    public void SellItem(int id)
    {
        if (Bot.Inventory.TryGetItem(id, out InventoryItem? item))
        {
            if (Bot.Options.SafeTimings)
            {
                Bot.Wait.ForActionCooldown(GameActions.SellItem);
                Bot.Wait.ItemSellEvent.Reset();
            }
            Bot.Send.Packet($"%xt%zm%sellItem%{Bot.Map.RoomID}%{item.ID}%{item.Quantity}%{item.CharItemID}%");
            if (Bot.Options.SafeTimings)
                Bot.Wait.ForItemSell();
        }
    }

    [MethodCallBinding("world.sendLoadHairShopRequest", RunMethodPre = true, GameFunction = true)]
    public void LoadHairShop(int id)
    {
        if (Bot.Options.SafeTimings)
            Bot.Wait.ForActionCooldown(GameActions.LoadHairShop);
    }

    [MethodCallBinding("openArmorCustomize", GameFunction = true)]
    public void LoadArmourCustomizer() { }
}
