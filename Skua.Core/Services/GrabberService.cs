using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skua.Core.Interfaces;
using Skua.Core.Interfaces.Services;
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
        IScriptMonster monsters)
    {
        Shops = shops;
        Quests = quests;
        Inventory = inventory;
        Bank = bank;
        House = house;
        TempInv = tempInv;
        Monsters = monsters;
    }
    private readonly IScriptShop Shops;
    private readonly IScriptQuest Quests;
    private readonly IScriptInventory Inventory;
    private readonly IScriptBank Bank;
    private readonly IScriptHouseInv House;
    private readonly IScriptTempInv TempInv;
    private readonly IScriptMonster Monsters;
    public List<object> Grab(GrabberTypes grabType)
    {
        List<object> items = new();
        switch (grabType)
        {
            case GrabberTypes.Shop_Items:
                items.AddRange(Shops.Items);
                break;
            case GrabberTypes.Shop_IDs:
                items.AddRange(Shops.LoadedCache.Select(s => (object)s.ID).ToList());
                break;
            case GrabberTypes.Quests:
                items.AddRange(Quests.Tree);
                break;
            case GrabberTypes.Inventory_Items:
                items.AddRange(Inventory.Items);
                break;
            case GrabberTypes.House_Inventory_Items:
                items.AddRange(House.Items);
                break;
            case GrabberTypes.Temp_Inventory_Items:
                items.AddRange(TempInv.Items);
                break;
            case GrabberTypes.Bank_Items:
                items.AddRange(Bank.Items);
                break;
            case GrabberTypes.Cell_Monsters:
                items.AddRange(Monsters.CurrentMonsters);
                break;
            case GrabberTypes.Map_Monsters:
                items.AddRange(Monsters.MapMonsters);
                break;
            case GrabberTypes.GetMap_Item_IDs:
                return new();
            default:
                return new();
        }
        return items;
    }
}
