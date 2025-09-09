using Skua.Core.Interfaces;
using Skua.Core.Models;

namespace Skua.Core.Services;

public class GrabberService : IGrabberService
{
    public GrabberService(
        IScriptShop shops,
        IScriptQuest quests,
        IScriptInventory inventory,
        IScriptBank bank,
        IScriptHouseInv house,
        IScriptTempInv tempInv,
        IScriptMonster monsters,
        IScriptMap map)
    {
        _shops = shops;
        _quests = quests;
        _inventory = inventory;
        _bank = bank;
        _house = house;
        _tempInv = tempInv;
        _monsters = monsters;
        _map = map;
    }

    private readonly IScriptShop _shops;
    private readonly IScriptQuest _quests;
    private readonly IScriptInventory _inventory;
    private readonly IScriptBank _bank;
    private readonly IScriptHouseInv _house;
    private readonly IScriptTempInv _tempInv;
    private readonly IScriptMonster _monsters;
    private readonly IScriptMap _map;

    public List<object> Grab(GrabberTypes grabType)
    {
        List<object> items = new();
        switch (grabType)
        {
            case GrabberTypes.Shop_Items:
                items.AddRange(_shops.Items);
                break;

            case GrabberTypes.Shop_IDs:
                items.AddRange(_shops.LoadedCache);
                break;

            case GrabberTypes.Quests:
                items.AddRange(_quests.Tree);
                break;

            case GrabberTypes.Inventory_Items:
                items.AddRange(_inventory.Items);
                break;

            case GrabberTypes.House_Inventory_Items:
                items.AddRange(_house.Items);
                break;

            case GrabberTypes.Temp_Inventory_Items:
                items.AddRange(_tempInv.Items);
                break;

            case GrabberTypes.Bank_Items:
                items.AddRange(_bank.Items);
                break;

            case GrabberTypes.Cell_Monsters:
                items.AddRange(_monsters.CurrentAvailableMonsters);
                break;

            case GrabberTypes.Map_Monsters:
                items.AddRange(_monsters.MapMonstersWithCurrentData);
                break;

            case GrabberTypes.GetMap_Item_IDs:
                items.AddRange(_map.FindMapItems() ?? new());
                break;

            default:
                return new();
        }
        return items;
    }
}