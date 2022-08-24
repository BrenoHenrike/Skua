using Microsoft.Extensions.DependencyInjection;
using Skua.Core.Interfaces;
using Skua.Core.ViewModels;
using System;
using System.Collections.Generic;

namespace Skua.Core.AppStartup;
internal class CoreBots
{
    internal static CoreBotsViewModel CreateViewModel(IServiceProvider s)
    {
        List<TabItemViewModel> tabs = new()
        {
            new("Loadout", s.GetRequiredService<CBOLoadoutViewModel>()),
            new("Options", s.GetRequiredService<CBOptionsViewModel>()),
            new("Other", CreateOtherOptions(s)),
        };

        return new(tabs, s.GetRequiredService<IScriptPlayer>(), s.GetRequiredService<IDialogService>());
    }

    internal static CBOptionsViewModel CreateOptions(IServiceProvider s)
    {
        List<DisplayOptionItemViewModelBase> options = new()
        {
            new DisplayOptionItemViewModel<bool>("Private Rooms", "PrivateRooms", true),
            new DisplayOptionItemViewModel<bool>("Public on Difficult Parts", "PublicDifficult"),
            new DisplayOptionItemViewModel<bool>("Bank Misc. AC items on Start-Up", "BankMiscAC", true),
            new DisplayOptionItemViewModel<bool>("Logger in Chat", "LoggerInChat", true),

            new DisplayOptionItemViewModel<bool>("Force Off MessageBoxes", "MessageBoxCheck"),
            new DisplayOptionItemViewModel<bool>("Should Rest", "RestCheck"),
            new DisplayOptionItemViewModel<bool>("Disable AutoEnhance", "DisableAutoEnhance"),
            new DisplayOptionItemViewModel<bool>("Disable BestGear", "DisableBestGear"),
            new DisplayOptionItemViewModel<bool>("Anti Lag", "AntiLag", true),

            new DisplayOptionItemViewModel<int>("Room Number", "PrivateRoomNr", 100000),
            new DisplayOptionItemViewModel<int>("Action Delay", "ActionDelayNr", 700),
            new DisplayOptionItemViewModel<int>("Exit Combat Delay", "ExitCombatNr", 1600),
            new DisplayOptionItemViewModel<int>("Hunt Delay", "HuntDelayNr", 100),
            new DisplayOptionItemViewModel<int>("Accept and Complete Tries", "QuestTriesNr", 20),
            new DisplayOptionItemViewModel<int>("Loaded Quests Max.", "QuestMaxNr", 150),

            new DisplayOptionItemViewModel<string>("Map after stopping the Bot", "StopLocationSelect", "Home"),
        };

        return new(options, s.GetRequiredService<IDialogService>());
    }

    private static CBOOtherOptionsViewModel CreateOtherOptions(IServiceProvider s)
    {
        List<CBOOptionItemContainerViewModel> otherOptions = new()
        {
            new("Hollowborn DoomKnight", new CBOBoolOptionItemViewModel("Pre Farm Dark-/Doom Fragments", "HBDK_PreFarm")),

            new("Necrotic Sword of Doom", new List<DisplayOptionItemViewModelBase>()
            {
                new CBOBoolOptionItemViewModel("Max Stack Essence in Retrieve Void Auras", "NSOD_MaxStack", true),
                new CBOBoolOptionItemViewModel("Pre Farm Materials", "NSOD_PreFarm")
            }),

            new("Nation Farms", new List<DisplayOptionItemViewModelBase>()
            {
                new CBOBoolOptionItemViewModel("Sell Voucher of Nulgath if not needed", "Nation_SellMemVoucher", true),
                new CBOBoolOptionItemViewModel("Do Swindles Return during Supplies", "Nation_ReturnPolicyDuringSupplies", true)
            }),

            new("Void Highlord", new CBOBoolOptionItemViewModel("Use Sparrow's Blood Method", "If possible will use Sparrow's Blood and \"Assisting Crag and Bamboozlez\" to get an additional Elders' Blood per day.", "VHL_Sparrow", true)),

            new("Sepulchure's DoomKnight Armor", new CBOBoolChoiceOptionItemViewModel("DSO Farm Method", "Which quest to farm Dark Spirit Orbs", "SDKA_Quest", "Dark Spirit Orbs", "A Penny for Your Foughts")),

            new("Bludrut Brawl (PvP)", new CBOBoolOptionItemViewModel("Kill ads before boss", "Whether to kill brawlers and restorers in the PvP before the boss.", "PvP_SoloPvPBoss")),

            new("Bot Creators Only", new CBOBoolOptionItemViewModel("Story Bot Test Mode", "Toggles innate isCompleteBefore checks. It remains on the for Complete Once Quests", "BCO_Story_TestBot"))
        };

        return new(otherOptions);
    }
}
