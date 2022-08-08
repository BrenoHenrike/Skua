using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Quests;
using Skua.Core.Models.Shops;
using Skua.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Skua.App.WPF.AppStartup;
internal class Grabber
{
    internal static GrabberViewModel CreateViewModel(IServiceProvider s)
    {
        return new GrabberViewModel(s.GetRequiredService<IEnumerable<GrabberListViewModel>>());
    }

    internal static IEnumerable<GrabberListViewModel> CreateListViewModels(IServiceProvider s)
    {
        IGrabberService grabberService = s.GetService<IGrabberService>()!;
        IDialogService dialogService = s.GetService<IDialogService>()!;
        IScriptInventory inventory = s.GetService<IScriptInventory>()!;
        IScriptShop shops = s.GetService<IScriptShop>()!;
        List<GrabberTaskViewModel> baseQuestCommands = new()
        {
            new("Open", OpenQuests),
            new("Accept", AcceptQuests)
        };
        List<GrabberTaskViewModel> questCommands = new(baseQuestCommands)
        {
            new("Register", RegisterQuests),
            new("Unregister All", async (i, p, t) =>
            {
                await Task.Run(() => Ioc.Default.GetService<IScriptQuest>()!.UnregisterAllQuests(), t);
            })
        };
        List<GrabberTaskViewModel> inventoryCommands = new()
        {
            new("Equip", EquipItems),
            new("Sell", SellItem),
            new("To Bank", InvToBank)
        };
        List<GrabberTaskViewModel> mapMonstersCommands = new()
        {
            new("Kill", KillMonster),
            new("Teleport To", TeleportToMonster)
        };
        List<GrabberTaskViewModel> mapItemCommands = new(baseQuestCommands)
        {
            new("Get Map Item", GetMapItem)
        };
        return new List<GrabberListViewModel>()
        {
            new GrabberListViewModel("Shop Items", grabberService, GrabberTypes.Shop_Items, new GrabberTaskViewModel("Buy", BuyItems), true),
            new GrabberListViewModel("Shop IDs", grabberService, GrabberTypes.Shop_IDs, new GrabberTaskViewModel("Load Shop", LoadShop), false),
            new GrabberListViewModel("Quests", grabberService, GrabberTypes.Quests, questCommands, true),
            new GrabberListViewModel("Inventory", grabberService, GrabberTypes.Inventory_Items, inventoryCommands, true),
            new GrabberListViewModel("House Inventory", grabberService, GrabberTypes.House_Inventory_Items, new GrabberTaskViewModel("To Bank", HouseInvToBank), true),
            new GrabberListViewModel("Temp Inventory", grabberService, GrabberTypes.Temp_Inventory_Items, false),
            new GrabberListViewModel("Bank Items", grabberService, GrabberTypes.Bank_Items, new GrabberTaskViewModel("To Inventory", BankToInv), true),
            new GrabberListViewModel("Cell Monsters", grabberService, GrabberTypes.Cell_Monsters, new GrabberTaskViewModel("Kill", KillMonster), true),
            new GrabberListViewModel("Map Monsters", grabberService, GrabberTypes.Map_Monsters, mapMonstersCommands, true),
            new GrabberListViewModel("GetMap Item IDs", grabberService, GrabberTypes.GetMap_Item_IDs, mapItemCommands, true)
        };
    }

    private static async Task GetMapItem(IList<object>? i, IProgress<string> p, CancellationToken t)
    {
        if (i is null || i.Count == 0)
        {
            p.Report("No map items found/selected.");
            return;
        }

        List<MapItem> mapItems = i.Cast<MapItem>().ToList();
        var map = Ioc.Default.GetService<IScriptMap>()!;
        var dialogService = Ioc.Default.GetService<IDialogService>()!;
        if(mapItems.Count == 1)
            p.Report($"Getting Map Item [{mapItems[0].ID}, input quantity...");
        else
            p.Report($"Getting {mapItems.Count} Map Items, input quantity...");
        InputDialogViewModel diag = new($"{(mapItems.Count == 1 ? $"Getting {mapItems[0].ID}" : $"Getting {mapItems.Count} Map Items")}", $"Quantity:");
        if (dialogService.ShowDialog(diag) != true)
        {
            p.Report("Cancelled.");
            return;
        }

        if (!int.TryParse(diag.DialogTextInput, out int result))
            return;
        try
        {
            if(mapItems.Count == 1)
            {
                await Task.Run(() => map.GetMapItem(mapItems[0].ID, result), t);
                p.Report("Map item acquired.");
                return;
            }

            for(int index = 0; index < mapItems.Count; index++)
            {
                p.Report($"Getting {mapItems[index].ID} x{result}");
                await Task.Run(() => map.GetMapItem(mapItems[index].ID, result), t);
                if(index != mapItems.Count - 1)
                    await Task.Delay(1000, t);
            }
            p.Report("Map items acquired");
        }
        catch
        {
            if (t.IsCancellationRequested)
                p.Report("Task cancelled.");
        }
    }
    private static async Task TeleportToMonster(IList<object>? i, IProgress<string> p, CancellationToken t)
    {
        if (i is null || i.Count == 0)
        {
            p.Report("No monsters found/selected.");
            return;
        }

        Monster monster = i.Cast<Monster>().ToList()[0];
        var map = Ioc.Default.GetService<IScriptMap>()!;
        try
        {
            await Task.Run(() => map.Jump(monster.Cell, "Left"), t);
        }
        catch
        {
            if (t.IsCancellationRequested)
                p.Report("Task cancelled.");
        }
    }

    private static async Task KillMonster(IList<object>? i, IProgress<string> p, CancellationToken t)
    {
        if (i is null || i.Count == 0)
        {
            p.Report("No monsters found/selected.");
            return;
        }
        List<Monster> monsters = i.Cast<Monster>().ToList();
        try
        {
            await Task.Run(async () =>
            {
                if (monsters.Count == 1)
                {
                    Monster monster = monsters[0];
                    p.Report($"Killing {monster.Name}.");
                    Kill(monster, t);
                    p.Report($"Killed {monster.Name}.");
                    return;
                }

                for (int index = 0; index < monsters.Count; index++)
                {
                    p.Report($"Killing {monsters[index].Name}.");
                    Kill(monsters[index], t);
                    await Task.Delay(1000, t);
                    p.Report($"Killed {monsters[index].Name}.");
                }
            }, t);
        }
        catch
        {
            if (t.IsCancellationRequested)
                p.Report("Task cancelled.");
        }

        void Kill(Monster monster, CancellationToken token)
        {
            if (monster.Cell != Ioc.Default.GetService<IScriptPlayer>()!.Cell)
                Ioc.Default.GetService<IScriptMap>()!.Jump(monster.Cell, "Left");

            Ioc.Default.GetService<IScriptKill>()!.Monster(monster, token);
        }
    }

    private static async Task BankToInv(IList<object>? i, IProgress<string> p, CancellationToken t)
    {
        await DefaultItemBaseTask("Unbanking", id => Ioc.Default.GetService<IScriptBank>()!.ToInventory(id), i, p, t);
    }

    private static async Task HouseInvToBank(IList<object>? i, IProgress<string> p, CancellationToken t)
    {
        await DefaultItemBaseTask("Banking", id => Ioc.Default.GetService<IScriptHouseInv>()!.ToBank(id), i, p, t);
    }

    private static async Task RegisterQuests(IList<object>? i, IProgress<string> p, CancellationToken t)
    {
        await DefaultQuestTask("Registering", Ioc.Default.GetService<IScriptQuest>()!.RegisterQuests, i, p, t);
    }

    private static async Task AcceptQuests(IList<object>? i, IProgress<string> p, CancellationToken t)
    {
        await DefaultQuestTask("Accepting", Ioc.Default.GetService<IScriptQuest>()!.EnsureAccept, i, p, t);
    }

    private static async Task OpenQuests(IList<object>? i, IProgress<string> p, CancellationToken t)
    {
        await DefaultQuestTask("Showing", Ioc.Default.GetService<IScriptQuest>()!.Load, i, p, t);
    }

    private static async Task DefaultQuestTask(string identifier, Action<int[]> action, IList<object>? i, IProgress<string> p, CancellationToken t)
    {
        if (i is null || i.Count == 0)
        {
            p.Report("No quests found/selected.");
            return;
        }
        IEnumerable<int>? questIds = null;
        switch (i.First())
        {
            case Quest:
                questIds = i.Cast<Quest>().Select(q => q.ID);
                break;
            case MapItem:
                questIds = i.Cast<MapItem>().Select(m => m.QuestID);
                break;
        }
        try
        {
            if (questIds is not null)
            {
                p.Report($"{identifier} {questIds.Count()} quests...");
                await Task.Run(() => action(questIds.ToArray()), t);
                p.Report("Finished.");
            }
        }
        catch
        {
            if (t.IsCancellationRequested)
                p.Report("Task cancelled.");
        }
    }

    private static async Task LoadShop(IList<object>? i, IProgress<string> p, CancellationToken t)
    {
        if (i is null || i.Count == 0)
        {
            p.Report("No items found/selected.");
            return;
        }

        ShopInfo shopInfo = i.Cast<ShopInfo>().First();
        try
        {
            await Task.Run(() => Ioc.Default.GetService<IScriptShop>()!.Load(shopInfo.ID), t);
            p.Report($"Shop {shopInfo.Name} [{shopInfo.ID}] loaded.");
        }
        catch
        {
            if (t.IsCancellationRequested)
                p.Report("Task cancelled.");
        }
    }

    private static async Task BuyItems(IList<object>? i, IProgress<string> p, CancellationToken t)
    {
        if (i is null || i.Count == 0)
        {
            p.Report("No items found/selected.");
            return;
        }
        var dialogService = Ioc.Default.GetService<IDialogService>()!;
        var shop = Ioc.Default.GetService<IScriptShop>()!;
        var player = Ioc.Default.GetService<IScriptPlayer>()!;

        List<ShopItem> items = i.Cast<ShopItem>().ToList();
        if(items.Count == 1)
        {
            ShopItem item = items[0];
            if(item.Coins && item.Cost > 0)
            {
                ACWarning(p, dialogService);
                return;
            }
            p.Report($"Buying {item.Name}, input quantity...");
            InputDialogViewModel diag = new($"Buying {item.Name}", $"Buy quantity (Cost: {item.Cost} {(item.Coins ? "AC" : "Gold")})");
            if (dialogService.ShowDialog(diag) != true)
            {
                p.Report("Cancelled.");
                return;
            }

            if (!int.TryParse(diag.DialogTextInput, out int result))
                return;

            if (result > item.MaxStack)
                result = item.MaxStack;
            int totalCost = item.Cost * result;
            if(!item.Coins && totalCost > player.Gold)
            {
                p.Report($"Not enough gold. Total: {totalCost:#,0}");
                dialogService.ShowMessageBox($"Not enough gold to buy {result} {item.Name}.\r\nTotal: {totalCost:#,0}\r\nNeeded: {totalCost - player.Gold:#,0}", "Not enough gold");
                return;
            }
            try
            {
                if (result == 1)
                {
                    await Task.Run(() => shop.BuyItem(item.ID, item.ShopItemID), t);
                    p.Report($"Bought {result} {item.Name}");
                    return;
                }
                for (int index = 0; index < result; index++)
                {
                    await Task.Run(() => shop.BuyItem(item.ID, item.ShopItemID), t);
                    p.Report($"Bought {index + 1}");
                    if (index != result - 1)
                        await Task.Delay(1000, t);
                }
                p.Report($"Bought {result} {item.Name}");
                return;
            }
            catch
            {
                if (t.IsCancellationRequested)
                    p.Report("Task cancelled.");
            }
        }

        List<ShopItem> coinItems = items.Where(i => i.Coins).ToList();
        List<ShopItem> goldItems = items.Where(i => !i.Coins).ToList();

        if(coinItems.Count > 0 && coinItems.Sum(i => i.Cost) > 0)
        {
            ACWarning(p, dialogService);
            return;
        }
        int totalGoldCost = 0;
        if(goldItems.Count > 0 && (totalGoldCost = goldItems.Sum(i => i.Cost)) > player.Gold)
        {
            p.Report($"Not enough gold. Total: {totalGoldCost}");
            dialogService.ShowMessageBox($"Not enough gold to buy the {items.Count} items.\r\nTotal: {totalGoldCost:#,0}\r\nNeeded: {totalGoldCost - player.Gold:#,0}", "Not enough gold");
            return;
        }
        try
        {
            for(int index = 0; index < items.Count; index++)
            {
                await Task.Run(() => shop.BuyItem(items[index].ID), t);
                p.Report($"Bought {items[index].Name}");
                if (index != items.Count - 1)
                    await Task.Delay(1000, t);
            }

        }
        catch
        {
            if (t.IsCancellationRequested)
                p.Report("Task cancelled.");
        }

        void ACWarning(IProgress<string> p, IDialogService dialogService)
        {
            p.Report("AC item - Cancelled");
            dialogService.ShowMessageBox("Don't use this to buy AC items that aren't 0 AC.", "AC Item");
        }
    }

    private static async Task SellItem(IList<object>? i, IProgress<string> p, CancellationToken t)
    {
        var dialogService = Ioc.Default.GetService<IDialogService>()!;
        if (i is null || i.Count == 0)
        {
            p.Report("No items found/selected.");
            return;
        }
        if (i.Count > 1)
        {
            p.Report("Warning");
            dialogService.ShowMessageBox($"ATTENTION - {i.Count} items selected!\nPlease sell 1 item at a time to prevent losses.", "Selling item - Warning");
            return;
        }
        InventoryItem item = i.Cast<InventoryItem>().First();
        if (item.Equipped)
        {
            dialogService.ShowMessageBox("Cannot sell equipped item.", "Sell item");
            return;
        }
        p.Report($"Selling {item.Name}, input quantity...");
        var shop = Ioc.Default.GetService<IScriptShop>()!;
        try
        {
            InputDialogViewModel diag = new($"Selling {item.Name}", $"Sell quantity (Currently has: {(item.Category == ItemCategory.Class ? 1 : item.Quantity)})");
            if (dialogService.ShowDialog(diag) != true)
            { 
                p.Report("Cancelled.");
                return;
            }

            if (!int.TryParse(diag.DialogTextInput, out int result))
                return;
            if(result == 1)
            {
                await Task.Run(() => shop.SellItem(item.ID), t);
                p.Report($"Sold {result} {item.Name}");
                return;
            }

            for (int index = 0; index < result; index++)
            {
                await Task.Run(() => shop.SellItem(item.ID), t);
                p.Report($"Sold {index + 1}");
                if(index != result - 1)
                    await Task.Delay(1000, t);
            }
            p.Report($"Sold {result} {item.Name}");
        }
        catch
        {
            if (t.IsCancellationRequested)
                p.Report("Task cancelled.");
        }
    }

    private static async Task EquipItems(IList<object>? i, IProgress<string> p, CancellationToken t)
    {
        await DefaultItemBaseTask("Equipping", Ioc.Default.GetService<IScriptInventory>()!.EquipItem, i, p, t);
    }

    private static async Task InvToBank(IList<object>? i, IProgress<string> p, CancellationToken t)
    {
        await DefaultItemBaseTask("Banking", id => Ioc.Default.GetService<IScriptInventory>()!.ToBank(id), i, p, t);
    }

    private static async Task DefaultItemBaseTask(string identifier, Action<int> action, IList<object>? i, IProgress<string> p, CancellationToken token)
    {
        if (i is null || i.Count == 0)
        {
            p.Report("No items found/selected.");
            return;
        }
        List<ItemBase> items = i.Cast<ItemBase>().ToList();
        p.Report($"{identifier} items...");
        try
        {
            if (items.Count == 1)
            {
                p.Report($"{identifier} {items[0].Name}.");
                action(items[0].ID);
                return;
            }

            for (int index = 0; index < items.Count; index++)
            {
                p.Report($"{identifier} {items[index].Name}.");
                action(items[index].ID);
                if(index != items.Count - 1)
                    await Task.Delay(1000, token);
            }
        }
        catch
        {
            if (token.IsCancellationRequested)
                p.Report("Task cancelled.");
        }
    }
}
