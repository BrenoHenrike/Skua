using Skua.Core.Interfaces;

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
        _lazyInventory  = inventory;
        _lazyHouseInv = houseInv;
        _lazyTempInv = tempInv;
        _lazyBank = bank;
    }

    public bool Check(string name, int quantity = 1, bool moveToInventory = true)
    {
        if (TempInv.Contains(name, quantity))
            return true;
        if (Bank.Contains(name))
        {
            if (!moveToInventory)
                return true;
            Bank.ToInventory(name);
        }
        if (Inventory.Contains(name, quantity))
            return true;

        return HouseInv.Contains(name, quantity);
    }

    public bool Check(int id, int quantity = 1, bool moveToInventory = true)
    {
        if (TempInv.Contains(id, quantity))
            return true;
        if (Bank.Contains(id))
        {
            if (!moveToInventory)
                return true;
            Bank.ToInventory(id);
        }
        if (Inventory.Contains(id, quantity))
            return true;
        return HouseInv.Contains(id);
    }

    public bool HasAll(IEnumerable<string> itemNames, int quantity = 1, bool moveToInventory = true)
    {
        if (itemNames is null || !itemNames.Any())
            return true;
        foreach (string name in itemNames)
        {
            if (Bank.Contains(name, quantity))
            {
                if (!moveToInventory)
                    continue;
                Bank.ToInventory(name);
                continue;
            }
            if (Inventory.Contains(name, quantity))
                continue;
            else
                return false;
        }
        return true;
    }

    public bool HasAny(IEnumerable<string> itemNames, int quantity = 1, bool moveToInventory = true)
    {
        if (itemNames is null || !itemNames.Any())
            return true;
        foreach (string name in itemNames)
        {
            if (Bank.Contains(name, quantity))
            {
                if (!moveToInventory)
                    return true;
                Bank.ToInventory(name);
                return true;
            }
            if (Inventory.Contains(name, quantity))
                return true;
            continue;
        }
        return false;
    }

    public bool HasAll(IEnumerable<int> itemIds, int quantity = 1, bool moveToInventory = true)
    {
        if (itemIds is null || !itemIds.Any())
            return true;
        foreach (int id in itemIds)
        {
            if (Bank.Contains(id, quantity))
            {
                if (!moveToInventory)
                    continue;
                Bank.ToInventory(id);
                continue;
            }
            if (Inventory.Contains(id, quantity))
                continue;
            else
                return false;
        }
        return true;
    }

    public bool HasAny(IEnumerable<int> itemIds, int quantity = 1, bool moveToInventory = true)
    {
        if (itemIds is null || !itemIds.Any())
            return true;
        foreach (int id in itemIds)
        {
            if (Bank.Contains(id, quantity))
            {
                if (!moveToInventory)
                    return true;
                Bank.ToInventory(id);
                return true;
            }
            if (Inventory.Contains(id, quantity))
                return true;
            continue;
        }
        return false;
    }
}
