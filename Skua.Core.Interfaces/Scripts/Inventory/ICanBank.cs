using Skua.Core.Models.Items;
using Skua.Core.Utils;

namespace Skua.Core.Interfaces;
public interface ICanBank : ICheckInventory<InventoryItem>, ILimitedInventory
{
    /// <summary>
    /// Transfers the item with specified <paramref name="name"/> to the bank.
    /// </summary>
    /// <param name="name">Name of the item to transfer.</param>
    /// <returns><see langword="true"/> if the item was moved to the bank.</returns>
    bool ToBank(string name)
    {
        if (TryGetItem(name, out InventoryItem? item))
            return ToBank(item!);
        return false;
    }
    /// <summary>
    /// Transfers the item with specified <paramref name="id"/> to the bank.
    /// </summary>
    /// <param name="name">ID of the item to transfer.</param>
    /// <returns><see langword="true"/> if the item was moved to the bank.</returns>
    bool ToBank(int id)
    {
        if (TryGetItem(id, out InventoryItem? item))
            return ToBank(item!);
        return false;
    }
    /// <summary>
    /// Transfers the item with specified <paramref name="item"/> to the bank.
    /// </summary>
    /// <param name="name">Name of the item to transfer.</param>
    /// <returns><see langword="true"/> if the item was moved to the bank.</returns>
    bool ToBank(InventoryItem item);
    /// <summary>
    /// Transfers the items with specified <paramref name="names"/> to the bank.
    /// </summary>
    /// <param name="name">Names of the items to transfer.</param>
    void ToBank(params string[] names)
    {
        for (int i = 0; i < names.Length; i++)
            ToBank(names[i]);
    }
    /// <summary>
    /// Transfers the items with specified <paramref name="ids"/> to the bank.
    /// </summary>
    /// <param name="ids">IDs of the items to transfer.</param>
    void ToBank(params int[] ids)
    {
        for (int i = 0; i < ids.Length; i++)
            ToBank(ids[i]);
    }
    /// <summary>
    /// Ensures the item with specified <paramref name="name"/> will be moved to the bank.
    /// </summary>
    /// <param name="name">Name of the item to transfer.</param>
    /// <remarks>It will try <see cref="IScriptOption.MaximumTries"/> then move on even if the transfer was unsuccessful.</remarks>
    void EnsureToBank(string name);
    /// <summary>
    /// Ensures the item with specified <paramref name="id"/> will be moved to the bank.
    /// </summary>
    /// <param name="name">ID of the item to transfer.</param>
    /// <remarks>It will try <see cref="IScriptOption.MaximumTries"/> then move on even if the transfer was unsuccessful.</remarks>
    void EnsureToBank(int id);
    /// <summary>
    /// Ensures the items with specified <paramref name="names"/> will be moved to the bank.
    /// </summary>
    /// <param name="names">Names of the items to transfer.</param>
    /// <remarks>It will try <see cref="IScriptOption.MaximumTries"/> then move on even if the transfer was unsuccessful.</remarks>
    void EnsureToBank(params string[] names)
    {
        for (int i = 0; i < names.Length; i++)
            EnsureToBank(names[i]);
    }
    /// <summary>
    /// Ensures the items with specified <paramref name="ids"/> will be moved to the bank.
    /// </summary>
    /// <param name="ids">IDs of the items to transfer.</param>
    /// <remarks>It will try <see cref="IScriptOption.MaximumTries"/> then move on even if the transfer was unsuccessful.</remarks>
    void EnsureToBank(params int[] ids)
    {
        {
            for (int i = 0; i < ids.Length; i++)
                EnsureToBank(ids[i]);
        }
    }
    /// <summary>
    /// Transfers all AC (coin) items that are not equipped to the bank.
    /// </summary>
    /// <remarks>If using from the <see cref="IScriptHouseInv"/>, make sure you have joined your house first.</remarks>
    void BankAllCoinItems()
    {
        Items.Where(i => i.Coins && !i.Equipped && i.Name != "treasure potion").ForEach(i => ToBank(i));
    }
    /// <summary>
    /// Transfers all AC (coin) items that are not equipped and don't have the category listed in <paramref name="filterOut"/> to the bank.
    /// </summary>
    /// <param name="filterOut">Categories of items that will not be banked.</param>
    /// <remarks>If using from the <see cref="IScriptHouseInv"/>, make sure you have joined your house first.</remarks>
    void BankAllCoinItems(ItemCategory[] filterOut)
    {
        Items.Where(i => i.Coins && !i.Equipped && i.Name != "treasure potion" && !filterOut.Contains(i.Category)).ForEach(i => ToBank(i));
    }
    /// <summary>
    /// Transfers all AC (coin) items that are not equipped and don't have the name listed in <paramref name="excludeNames"/> to the bank.
    /// </summary>
    /// <param name="excludeNames">Names of items that will not be banked.</param>
    /// <remarks>If using from the <see cref="IScriptHouseInv"/>, make sure you have joined your house first.</remarks>
    void BankAllCoinItems(params string[] excludeNames)
    {
        Items.Where(i => i.Coins && !i.Equipped && i.Name != "treasure potion" && !excludeNames.Contains(i.Name)).ForEach(i => ToBank(i));
    }
    /// <summary>
    /// Transfers all AC (coin) items that are not equipped and don't have the ID listed in <paramref name="excludeIds"/> to the bank.
    /// </summary>
    /// <param name="excludeIds">IDs of items that will not be banked.</param>
    /// <remarks>If using from the <see cref="IScriptHouseInv"/>, make sure you have joined your house first.</remarks>
    void BankAllCoinItems(params int[] excludeIds)
    {
        Items.Where(i => i.Coins && !i.Equipped && i.Name != "treasure potion" && !excludeIds.Contains(i.ID)).ForEach(i => ToBank(i));
    }
    /// <summary>
    /// Transfers all AC (coin) items that are not equipped and pass (returns <see langword="true"/>) the <paramref name="predicate"/> to the bank.
    /// </summary>
    /// <param name="predicate">Predicate funtion to apply in the AC items.</param>
    /// <remarks>If using from the <see cref="IScriptHouseInv"/>, make sure you have joined your house first.</remarks>
    void BankAllCoinItems(Predicate<InventoryItem> predicate)
    {
        Items.Where(i => i.Coins && !i.Equipped && i.Name != "treasure potion" && predicate(i)).ForEach(i => ToBank(i));
    }
}
