using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

namespace Skua.Core.Scripts;

public class ScriptInventoryHelper : IScriptInventoryHelper
{
    private readonly Lazy<IScriptInventory> _lazyInventory;
    private readonly Lazy<IScriptHouseInv> _lazyHouseInv;
    private readonly Lazy<IScriptTempInv> _lazyTempInv;
    private readonly Lazy<IScriptBank> _lazyBank;
    private IScriptInventory Inventory => _lazyInventory.Value;
    private IScriptHouseInv HouseInv => _lazyHouseInv.Value;
    private IScriptTempInv TempInv => _lazyTempInv.Value;
    private IScriptBank Bank => _lazyBank.Value;

    public ScriptInventoryHelper(
        Lazy<IScriptInventory> inventory,
        Lazy<IScriptHouseInv> houseInv,
        Lazy<IScriptTempInv> tempInv,
        Lazy<IScriptBank> bank)
    {
        _lazyInventory = inventory;
        _lazyHouseInv = houseInv;
        _lazyTempInv = tempInv;
        _lazyBank = bank;
    }

    public bool Check(string name, int quantity = 1, bool moveToInventory = true)
    {
        if (TempInv.Contains(name, quantity))
            return true;

        if (Inventory.Contains(name, quantity))
            return true;

        if (Bank.Contains(name))
        {
            if (moveToInventory)
                Bank.ToInventory(name);

            if ((moveToInventory && Inventory.GetQuantity(name) >= quantity) ||
               (!moveToInventory && Bank.GetQuantity(name) >= quantity))
                return true;
        }

        if (HouseInv.Contains(name))
            return true;

        return false;
    }

    public bool Check(int id, int quantity = 1, bool moveToInventory = true)
    {
        if (TempInv.Contains(id, quantity))
            return true;

        if (Inventory.Contains(id, quantity))
            return true;

        if (Bank.Contains(id))
        {
            if (moveToInventory)
                Bank.ToInventory(id);

            if ((moveToInventory && Inventory.GetQuantity(id) >= quantity) ||
               (!moveToInventory && Bank.GetQuantity(id) >= quantity))
                return true;
        }

        if (HouseInv.Contains(id))
            return true;

        return false;
    }

    public bool HasAll(IEnumerable<string> itemNames, int quantity = 1, bool moveToInventory = true)
    {
        if (itemNames is null || !itemNames.Any())
            return true;

        Bank.Load();

        Dictionary<string, InventoryItem> bank = Bank.Items.ToDictionary(i => i.Name, i => i);
        Dictionary<string, int> inv = Inventory.Items.ToDictionary(i => i.Name, i => i.Quantity);
        Dictionary<string, int> tempInv = TempInv.Items.ToDictionary(i => i.Name, i => i.Quantity);
        Dictionary<string, int> houseInv = HouseInv.Items.ToDictionary(i => i.Name, i => i.Quantity);

        int quant = 0;
        foreach (string name in itemNames)
        {
            if (tempInv.TryGetValue(name, out quant) && quant >= quantity)
                continue;

            if (inv.TryGetValue(name, out quant) && quant >= quantity)
                continue;

            if (bank.TryGetValue(name, out InventoryItem? item) && item.Quantity >= quantity)
            {
                if (!moveToInventory)
                    continue;
                Bank.ToInventory(bank[name]);
                continue;
            }

            if (houseInv.TryGetValue(name, out quant) && quant >= quantity)
                continue;

            return false;
        }
        return true;
    }

    public bool HasAny(IEnumerable<string> itemNames, int quantity = 1, bool moveToInventory = true)
    {
        if (itemNames is null || !itemNames.Any())
            return true;

        Bank.Load();

        Dictionary<string, InventoryItem> bank = Bank.Items.ToDictionary(i => i.Name, i => i);
        Dictionary<string, int> inv = Inventory.Items.ToDictionary(i => i.Name, i => i.Quantity);
        Dictionary<string, int> tempInv = TempInv.Items.ToDictionary(i => i.Name, i => i.Quantity);
        Dictionary<string, int> houseInv = HouseInv.Items.ToDictionary(i => i.Name, i => i.Quantity);

        int quant = 0;
        foreach (string name in itemNames)
        {
            if (tempInv.TryGetValue(name, out quant) && quant >= quantity)
                return true;

            if (inv.TryGetValue(name, out quant) && quant >= quantity)
                return true;

            if (bank.TryGetValue(name, out InventoryItem? item) && item.Quantity >= quantity)
            {
                if (!moveToInventory)
                    return true;
                Bank.ToInventory(item);
                return true;
            }

            if (houseInv.TryGetValue(name, out quant) && quant >= quantity)
                return true;
        }
        return false;
    }

    public bool HasAll(IEnumerable<int> itemIds, int quantity = 1, bool moveToInventory = true)
    {
        if (itemIds is null || !itemIds.Any())
            return true;

        Bank.Load();

        Dictionary<int, InventoryItem> bank = Bank.Items.ToDictionary(i => i.ID, i => i);
        Dictionary<int, int> inv = Inventory.Items.ToDictionary(i => i.ID, i => i.Quantity);
        Dictionary<int, int> tempInv = TempInv.Items.ToDictionary(i => i.ID, i => i.Quantity);
        Dictionary<int, int> houseInv = HouseInv.Items.ToDictionary(i => i.ID, i => i.Quantity);

        int quant = 0;
        foreach (int id in itemIds)
        {
            if (tempInv.TryGetValue(id, out quant) && quant >= quantity)
                continue;

            if (inv.TryGetValue(id, out quant) && quant >= quantity)
                continue;

            if (bank.TryGetValue(id, out InventoryItem? item) && item.Quantity >= quantity)
            {
                if (!moveToInventory)
                    continue;
                Bank.ToInventory(item);
                continue;
            }

            if (houseInv.TryGetValue(id, out quant) && quant >= quantity)
                continue;

            return false;
        }
        return true;
    }

    public bool HasAny(IEnumerable<int> itemIds, int quantity = 1, bool moveToInventory = true)
    {
        if (itemIds is null || !itemIds.Any())
            return true;

        Bank.Load();

        Dictionary<int, InventoryItem> bank = Bank.Items.ToDictionary(i => i.ID, i => i);
        Dictionary<int, int> inv = Inventory.Items.ToDictionary(i => i.ID, i => i.Quantity);
        Dictionary<int, int> tempInv = TempInv.Items.ToDictionary(i => i.ID, i => i.Quantity);
        Dictionary<int, int> houseInv = HouseInv.Items.ToDictionary(i => i.ID, i => i.Quantity);

        int quant = 0;
        foreach (int id in itemIds)
        {
            if (tempInv.TryGetValue(id, out quant) && quant >= quantity)
                return true;

            if (inv.TryGetValue(id, out quant) && quant >= quantity)
                return true;

            if (bank.TryGetValue(id, out InventoryItem? item) && item.Quantity >= quantity)
            {
                if (!moveToInventory)
                    return true;
                Bank.ToInventory(item);
                return true;
            }

            if (houseInv.TryGetValue(id, out quant) && quant >= quantity)
                return true;
        }
        return false;
    }
}