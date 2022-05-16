using Skua.Core.Models.Shops;

namespace Skua.Core.Interfaces;

public interface IScriptShop
{
    /// <summary>
    /// A boolean indicated whether a shop is currently loaded or not.
    /// </summary>
    bool IsLoaded { get; }
    /// <summary>
    /// Gets the currently (or last loaded) shop id.
    /// </summary>
    int ID { get; }
    /// <summary>
    /// A list of items that were available in the last loaded shop.
    /// </summary>
    List<ShopItem> Items { get; }
    /// <summary>
    /// A list with all the loaded shops in the current session.
    /// </summary>
    List<ShopInfo> LoadedCache { get; set; }
    /// <summary>
    /// Gets the currently (or last loaded) shop's name.
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Buys the item with specified <paramref name="name"/> from the <see cref="ID">currently loaded shop</see>.
    /// </summary>
    /// <param name="name">Name of the item to buy.</param>
    void BuyItem(string name);
    /// <summary>
    /// Buys the item with specified <paramref name="id"/> from the <see cref="ID">currently loaded shop</see>.
    /// </summary>
    /// <param name="id">Name of the item to buy.</param>
    void BuyItem(int id);
    /// <summary>
    /// Buys the item with specified <paramref name="name"/> from the specified <paramref name="shopId"/>.
    /// </summary>
    /// <param name="shopId">Shop to buy the item from.</param>
    /// <param name="name">Name of the item to buy.</param>
    /// <remarks>This loads the shop, waits until it is fully loaded, and then sends the buy item request.</remarks>
    void BuyItem(int shopId, string name)
    {
        Load(shopId);
        BuyItem(name);
    }
    /// <summary>
    /// Buys the item with specified <paramref name="itemId"/> from the specified <paramref name="shopId"/>.
    /// </summary>
    /// <param name="shopId">ID of the shop to buy the item from.</param>
    /// <param name="itemId">ID of the item to buy.</param>
    /// <remarks>This loads the shop, waits until it is fully loaded, and then sends the buy item request.</remarks>
    void BuyItem(int shopId, int itemId)
    {
        Load(shopId);
        BuyItem(itemId);
    }
    /// <summary>
    /// Loads the shop with specified <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The id of the shop to be loaded.</param>
    /// <remarks>Loading invalid shop ids will get you kicked. Be sure to only use updated/recent lists.</remarks>
    void Load(int id);
    /// <summary>
    /// Loads the armour customizer interface.
    /// </summary>
    void LoadArmourCustomizer();
    /// <summary>
    /// Loads the specified hair shop in game.
    /// </summary>
    /// <param name="id">ID of the hair shop to be loaded.</param>
    void LoadHairShop(int id);
    /// <summary>
    /// Sells the item with specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">Name of the item to sell.</param>
    void SellItem(string name);
    /// <summary>
    /// Sells the item with specified <paramref name="id"/>.
    /// </summary>
    /// <param name="id">ID of the item to sell.</param>
    void SellItem(int id);
    void OnLoaded(int id, string name, List<ShopItem> items)
    {
        if (!LoadedCache.Any(s => s.ID == id))
            LoadedCache.Add(new ShopInfo(id, name, items));
    }
}
