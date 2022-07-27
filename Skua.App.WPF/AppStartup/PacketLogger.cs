using Microsoft.Extensions.DependencyInjection;
using Skua.Core.Interfaces;
using Skua.Core.ViewModels;
using System;
using System.Collections.Generic;

namespace Skua.App.WPF.AppStartup;
internal static class PacketLogger
{
    internal static PacketLoggerViewModel CreateViewModel(IServiceProvider s)
    {
        List<PacketLogFilterViewModel> filters = new()
        {
            new("Combat", p =>
            {
                return p.Length >= 3 &&
                    (p[2] == "restRequest" || p[2] == "gar" || p[2] == "aggroMon");
            }),
            new("User Data", p =>
            {
                return p.Length >= 3 &&
                    (p[2] == "retrieveUserData" || p[2] == "retrieveUserDatas");
            }),
            new("Join", p =>
            {
                return p.Length >= 5 &&
                    (p[4] == "tfer" || p[2] == "house");
            }),
            new("Jump", p =>
            {
                return p.Length >= 3 &&
                    p[2] == "moveToCell";
            }),
            new("Movement", p =>
            {
                return p.Length >= 3 &&
                    p[2] == "mv" || p[2] == "mtcid";
            }),
            new("Get Map", p =>
            {
                return p.Length >= 3 &&
                    p[2] == "getMapItem";
            }),
            new("Quest", p =>
            {
                return p.Length >= 3 &&
                    (p[2] == "getQuest" || p[2] == "acceptQuest" || p[2] == "tryQuestComplete" || p[2] == "updateQuest");
            }),
            new("Shop", p =>
            {
                return p.Length >= 3 &&
                    (p[2] == "loadShop" || p[2] == "buyItem" || p[2] == "sellItem");
            }),
            new("Equip", p =>
            {
                return p.Length >= 3 &&
                    p[2] == "equipItem";
            }),
            new("Drop", p =>
            {
                return p.Length >= 3 &&
                    p[2] == "getDrop";
            }),
            new("Chat", p =>
            {
                return p.Length >= 3 &&
                    (p[2] == "message" || p[2] == "cc");
            }),
            new("Misc", p =>
            {
                return p.Length >= 3 &&
                    (p[2] == "crafting" || p[2] == "setHomeTown" || p[2] == "afk" || p[2] == "summonPet");
            })
        };

        return new PacketLoggerViewModel(filters, s.GetService<IFlashUtil>()!, s.GetService<IFileDialogService>()!);
    }
}
