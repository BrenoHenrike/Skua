using Skua.Core.Models.Items;

namespace Skua.Core.Interfaces;
public interface IScriptBank : ICheckInventory<InventoryItem>, ILimitedInventory
{
    /// <summary>
    /// A boolean indicating whether the bank is loaded or not. This resets on relogin.
    /// </summary>
    bool Loaded { get; set; }
    /// <summary>
    /// Displays the bank.
    /// </summary>
    void Open();
    /// <summary>
    /// Loads the bank so it can be used in scripts.
    /// </summary>
    /// <param name="waitForLoad">Whether wait for the bank to completely load.</param>
    void Load(bool waitForLoad = true);
    /// <summary>
    /// Swaps the specified items between the inventory and the bank.
    /// </summary>
    /// <param name="invItem">Name of the item in the inventory to be transferred to the bank.</param>
    /// <param name="bankItem">Name of the item in the bank to be transferred to the inventory.</param>
    /// <returns><see langword="true"/> if the swap was sucessfull</returns>
    bool Swap(string invItem, string bankItem);
    /// <summary>
    /// Swaps the specified items between the inventory and the bank.
    /// </summary>
    /// <param name="invItem">Name of the item in the inventory to be transferred to the bank.</param>
    /// <param name="bankItem">Name of the item in the bank to be transferred to the inventory.</param>
    /// <returns><see langword="true"/> if the swap was sucessfull</returns>
    bool Swap(int invItem, int bankItem);
    /// <summary>
    /// Swaps the specified items between the inventory and the bank.
    /// </summary>
    /// <param name="invItem">Name of the item in the inventory to be transferred to the bank.</param>
    /// <param name="bankItem">Name of the item in the bank to be transferred to the inventory.</param>
    /// <returns><see langword="true"/> if the swap was sucessfull</returns>
    bool Swap(InventoryItem invItem, InventoryItem bankItem);
    /// <summary>
    /// Ensures the swap of the specified items between the inventory and the bank is successful.
    /// </summary>
    /// <param name="invItem">Name of the item in the inventory to be transferred to the bank.</param>
    /// <param name="bankItem">Name of the item in the bank to be transferred to the inventory.</param>
    /// <remarks>It will try <see cref="IScriptOption.MaximumTries"/> then move on even if the swap was unsuccessful.</remarks>
    bool EnsureSwap(string invItem, string bankItem, bool loadBank = true);
    /// <summary>
    /// Ensures the swap of the specified items between inventory and the bank is successful.
    /// </summary>
    /// <param name="invItem">ID of the item in the inventory to be transferred to the bank.</param>
    /// <param name="bankItem">ID of the item in the bank to be transferred to the inventory.</param>
    /// <remarks>It will try <see cref="IScriptOption.MaximumTries"/> then move on even if the swap was unsuccessful.</remarks>
    bool EnsureSwap(int invItem, int bankItem, bool loadBank = true);
    /// <summary>
    /// Transfers the item with specified <paramref name="name"/> to inventory.
    /// </summary>
    /// <param name="name">Name of the item to transfer.</param>
    /// <returns><see langword="true"/> if the item was moved to the inventory.</returns>
    bool ToInventory(string name)
    {
        if (TryGetItem(name, out InventoryItem? item))
            ToInventory(item!);
        return false;
    }
    /// <summary>
    /// Transfers the item with specified <paramref name="id"/> to inventory.
    /// </summary>
    /// <param name="id">Name of the item to transfer.</param>
    /// <returns><see langword="true"/> if the item was moved to the inventory.</returns>
    bool ToInventory(int id)
    {
        if (TryGetItem(id, out InventoryItem? item))
            ToInventory(item!);
        return false;
    }
    /// <summary>
    /// Transfers the specified <paramref name="item"/> to inventory.
    /// </summary>
    /// <param name="item">Name of the item to transfer.</param>
    /// <returns><see langword="true"/> if the item was moved to the inventory.</returns>
    bool ToInventory(InventoryItem item);
    /// <summary>
    /// Transfers all items with specified <paramref name="names"/> to inventory.
    /// </summary>
    /// <param name="names">Name of the items to transfer.</param>
    void ToInventory(params string[] names)
    {
        for (int i = 0; i < names.Length; i++)
            ToInventory(names[i]);
    }
    /// <summary>
    /// Transfers all items with specified <paramref name="ids"/> to inventory.
    /// </summary>
    /// <param name="ids">IDs of the items to transfer.</param>
    void ToInventory(params int[] ids)
    {
        for (int i = 0; i < ids.Length; i++)
            ToInventory(ids[i]);
    }
    /// <summary>
    /// Ensures the item with specified <paramref name="name"/> will be moved to inventory.
    /// </summary>
    /// <param name="name">Name of the item to transfer.</param>
    /// <param name="loadBank">Whether to load the bank first.</param>
    /// <remarks>It will try <see cref="IScriptOption.MaximumTries"/> then move on even if the transfer was unsuccessful.</remarks>
    bool EnsureToInventory(string name, bool loadBank = true);
    /// <summary>
    /// Ensures the item with specified <paramref name="id"/> will be moved to inventory.
    /// </summary>
    /// <param name="id">ID of the item to transfer.</param>
    /// <param name="loadBank">Whether to load the bank first.</param>
    /// <remarks>It will try <see cref="IScriptOption.MaximumTries"/> then move on even if the transfer was unsuccessful.</remarks>
    bool EnsureToInventory(int id, bool loadBank = true);
    /// <summary>
    /// Ensures the items with specified <paramref name="names"/> will be moved to inventory.
    /// </summary>
    /// <param name="loadBank">Whether to load the bank first.</param>
    /// <param name="names">Names of the items to transfer.</param>
    /// <remarks>It will try <see cref="IScriptOption.MaximumTries"/> then move on even if the transfer was unsuccessful.</remarks>
    void EnsureToInventory(bool loadBank, params string[] names)
    {
        if (loadBank)
            Load();
        for (int i = 0; i < names.Length; i++)
            EnsureToInventory(names[i], false);
    }
    /// <summary>
    /// Ensures the items with specified <paramref name="ids"/> will be moved to inventory.
    /// </summary>
    /// <param name="loadBank">Whether to load the bank first.</param>
    /// <param name="ids">IDs of the items to transfer.</param>
    /// <remarks>It will try <see cref="IScriptOption.MaximumTries"/> then move on even if the transfer was unsuccessful.</remarks>
    void EnsureToInventory(bool loadBank, params int[] ids)
    {
        if (loadBank)
            Load();
        for (int i = 0; i < ids.Length; i++)
            EnsureToInventory(ids[i], false);
    }
}
