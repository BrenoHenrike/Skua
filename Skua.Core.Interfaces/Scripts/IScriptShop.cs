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
    /// <param name="quantity">Quantity to buy of the item, if -1 it will buy the default quantity.</param>
    void BuyItem(string name, int quantity = -1);

    /// <summary>
    /// Buys the item with specified <paramref name="id"/> from the <see cref="ID">currently loaded shop</see>.
    /// </summary>
    /// <param name="id">ID of the item to buy.</param>
    /// <param name="shopItemId">ID of the item inside the shop.</param>
    /// <param name="quantity">Quantity to buy of the item, if -1 it will buy the default quantity.</param>
    void BuyItem(int id, int shopItemId = -1, int quantity = -1);

    /// <summary>
    /// Buys the item with specified <paramref name="name"/> from the specified <paramref name="shopId"/>.
    /// </summary>
    /// <param name="shopId">Shop to buy the item from.</param>
    /// <param name="name">Name of the item to buy.</param>
    /// <param name="quantity">Quantity to buy of the item, if -1 it will buy the default quantity.</param>
    /// <remarks>This loads the shop, waits until it is fully loaded, and then sends the buy item request.</remarks>
    void LoadAndBuyItem(int shopId, string name, int quantity = -1)
    {
        Load(shopId);
        BuyItem(name, quantity);
    }

    /// <summary>
    /// Buys the item with specified <paramref name="itemId"/> from the specified <paramref name="shopId"/>.
    /// </summary>
    /// <param name="shopId">ID of the shop to buy the item from.</param>
    /// <param name="itemId">ID of the item to buy.</param>
    /// <param name="shopItemId">Shop Id of the item to buy.</param>
    /// <param name="quantity">Quantity to buy of the item, if -1 it will buy the default quantity.</param>
    /// <remarks>This loads the shop, waits until it is fully loaded, and then sends the buy item request.</remarks>
    void LoadAndBuyItem(int shopId, int itemId, int shopItemId = -1, int quantity = -1)
    {
        Load(shopId);
        BuyItem(itemId, shopItemId, quantity);
    }

    /// <summary>
    /// Buys the item with specified <paramref name="itemName"/> from the specified <paramref name="shopId"/>.
    /// This works the same as the <see cref="LoadAndBuyItem(int, string, int)">LoadAndBuyItem function</see> but bypasses safe timing.
    /// </summary>
    /// <param name="shopId">Shop to buy the item from.</param>
    /// <param name="itemName">Name of the item to buy.</param>
    /// <param name="quantity">Quantity to buy of the item, if -1 it will buy the default quantity.</param>
    /// <remarks>This loads the shop, waits until it is fully loaded, and then sends the buy item request.</remarks>
    void EnsureLoadAndBuyItem(int shopId, string itemName, int quantity = -1);

    /// <summary>
    /// Buys the item with specified <paramref name="itemId"/> and <paramref name="shopItemId"/> from the specified <paramref name="shopId"/>.
    /// This works the same as the <see cref="LoadAndBuyItem(int, int, int, int)">LoadAndBuyItem function</see> but bypasses safe timing.
    /// </summary>
    /// <param name="shopId">ID of the shop to buy the item from.</param>
    /// <param name="itemId">ID of the item to buy.</param>
    /// <param name="shopItemId">Shop Id of the item to buy.</param>
    /// <param name="quantity">Quantity to buy of the item, if -1 it will buy the default quantity.</param>
    /// <remarks>This loads the shop, waits until it is fully loaded, and then sends the buy item request.</remarks>
    void EnsureLoadAndBuyItem(int shopId, int itemId, int shopItemId = -1, int quantity = -1);

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
}