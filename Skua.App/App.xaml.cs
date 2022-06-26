using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Animation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Skua.App.Flash;
using Skua.App.Properties;
using Skua.App.Services;
using Skua.Core.GameProxy;
using Skua.Core.Interfaces;
using Skua.Core.Interfaces.Services;
using Skua.Core.Interfaces.Skills;
using Skua.Core.Messaging;
using Skua.Core.Models;
using Skua.Core.Models.GitHub;
using Skua.Core.Models.Items;
using Skua.Core.Scripts;
using Skua.Core.Services;
using Skua.Core.Skills;
using Skua.Core.Utils;
using Skua.Core.ViewModels;
using Westwind.Scripting;

namespace Skua.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public sealed partial class App : Application
{
    public App()
    {
        InitializeComponent();
        Services = ConfigureServices();
        Bot = Services.GetService<IScriptInterface>()!;
        Bot.Flash.FlashCall += Flash_FlashCall;
        var logService = Services.GetService<ILogService>();
        Dispatcher.ShutdownStarted += (s, e) => logService!.Dispose();
        //Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 30 });
    }

    private readonly IScriptInterface Bot;
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        MainWindow main = new(Bot);
        main.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        main.Show();
    }

    /// <summary>
    /// Gets the current <see cref="App"/> instance in use
    /// </summary>
    public new static App Current => (App)Application.Current;

    public IServiceProvider Services { get; }
    /// <summary>
    /// Configures the services for the application.
    /// </summary>

    private static IServiceProvider ConfigureServices()
    {
        ServiceCollection services = new();

        services.AddTransient(typeof(Lazy<>), typeof(LazyInstance<>));

        services.AddSingleton<IFlashUtil, FlashUtil>();

        services.AddSingleton<IScriptInterface, ScriptInterface>();
        services.AddSingleton<IScriptManager, ScriptManager>();

        services.AddSingleton<IScriptInventory, ScriptInventory>();
        services.AddSingleton<IScriptHouseInv, ScriptHouseInv>();
        services.AddSingleton<IScriptTempInv, ScriptTempInv>();
        services.AddSingleton<IScriptBank, ScriptBank>();

        services.AddSingleton<IAdvancedSkillContainer, AdvancedSkillContainer>();
        services.AddSingleton<IScriptCombat, ScriptCombat>();
        services.AddSingleton<IScriptKill, ScriptKill>();
        services.AddSingleton<IScriptHunt, ScriptHunt>();
        services.AddSingleton<IScriptSkill, ScriptSkill>();

        services.AddSingleton<IScriptFaction, ScriptFaction>();
        services.AddSingleton<IScriptMonster, ScriptMonster>();
        services.AddSingleton<IScriptPlayer, ScriptPlayer>();
        services.AddSingleton<IScriptQuest, ScriptQuest>();
        services.AddSingleton<IScriptBoost, ScriptBoost>();
        services.AddSingleton<IScriptShop, ScriptShop>();
        services.AddSingleton<IScriptDrop, ScriptDrop>();
        services.AddSingleton<IScriptMap, ScriptMap>();

        services.AddSingleton<IScriptServers, ScriptServers>();
        services.AddSingleton<IScriptEvent, ScriptEvent>();
        services.AddSingleton<IScriptSend, ScriptSend>();

        services.AddSingleton<IScriptOption, ScriptOption>();
        services.AddSingleton<IScriptLite, ScriptLite>();

        services.AddSingleton<IScriptBotStats, ScriptBotStats>();
        services.AddSingleton<IScriptHandlers, ScriptHandlers>();
        services.AddSingleton<IScriptWait, ScriptWait>();

        services.AddSingleton<ICaptureProxy, CaptureProxy>();

        services.AddSingleton<IMapService, MapService>();
        services.AddSingleton<IDropService, DropService>();
        services.AddSingleton<ILogService, LogService>();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<IDecamelizer, Decamelizer>();
        services.AddSingleton<IClipboardService, ClipboardService>();
        services.AddSingleton<IQuestDataLoaderService, QuestDataLoaderService>();
        services.AddSingleton<IGrabberService, GrabberService>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IGetScriptsService, GetScriptsService>();
        services.AddSingleton<IFileDialogService, FileDialogService>();
        services.AddSingleton<IVSCodeService, VSCodeService>();

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainMenuViewModel>();
        services.AddTransient<MainMenuItemViewModel>();
        services.AddTransient(s => CreateBotWindow(s));
        services.AddSingleton<IWindowService, WindowService>();
        services.AddTransient<LoaderViewModel>();
        services.AddTransient(s => CreateGrabberView(s));
        services.AddTransient<JumpViewModel>();
        services.AddSingleton<FastTravelViewModel>();
        services.AddSingleton<LogsViewModel>();
        services.AddSingleton(typeof(IEnumerable<LogTabItemViewModel>), s => CreateLogTabs(s));
        services.AddSingleton(s => CreateOptionsView(s));
        services.AddSingleton(s => CreatePacketLoggerView(s));
        services.AddSingleton<PacketSpammerViewModel>();
        services.AddTransient<ConsoleViewModel>();
        services.AddSingleton<ScriptRepoViewModel>();
        services.AddSingleton<ScriptLoaderViewModel>();
        services.AddSingleton<PacketInterceptorViewModel>();
        services.AddSingleton<AdvancedSkillsViewModel>();
        services.AddSingleton<AdvancedSkillEditorViewModel>();
        services.AddTransient<SkillRulesViewModel>();

        services.AddSingleton(s => CreateCompiler(s));

        ServiceProvider provider = services.BuildServiceProvider();
        Ioc.Default.ConfigureServices(provider);

        return provider;
    }

    private static IEnumerable<LogTabItemViewModel> CreateLogTabs(IServiceProvider s)
    {
        ILogService logService = s.GetService<ILogService>()!;
        IClipboardService clipBoard = s.GetService<IClipboardService>()!;
        IFileDialogService fileDialog = s.GetService<IFileDialogService>()!;
        return new[]
        {
            new LogTabItemViewModel("Debug", logService, clipBoard, fileDialog, LogType.Debug),
            new LogTabItemViewModel("Script", logService, clipBoard, fileDialog, LogType.Script),
            new LogTabItemViewModel("Flash", logService, clipBoard, fileDialog, LogType.Flash)
        };
    }

    private static CSharpScriptExecution CreateCompiler(IServiceProvider s)
    {
        CSharpScriptExecution compiler = new();
        compiler.AddLoadedReferences();
        compiler.SaveGeneratedCode = true;
        return compiler;
    }

    private static PacketLoggerViewModel CreatePacketLoggerView(IServiceProvider s)
    {
        List<PacketLogFilterViewModel> filters = new()
        {
            new PacketLogFilterViewModel("Combat", p =>
            {
                return p.Length >= 3 &&
                    (p[2] == "restRequest" || p[2] == "gar" || p[2] == "aggroMon");
            }),
            new PacketLogFilterViewModel("User Data", p =>
            {
                return p.Length >= 3 &&
                    (p[2] == "retrieveUserData" || p[2] == "retrieveUserDatas");
            }),
            new PacketLogFilterViewModel("Join", p =>
            {
                return p.Length >= 5 &&
                    (p[4] == "tfer" || p[2] == "house");
            }),
            new PacketLogFilterViewModel("Jump", p =>
            {
                return p.Length >= 3 &&
                    p[2] == "moveToCell";
            }),
            new PacketLogFilterViewModel("Movement", p =>
            {
                return p.Length >= 3 &&
                    p[2] == "mv" || p[2] == "mtcid";
            }),
            new PacketLogFilterViewModel("Get Map", p =>
            {
                return p.Length >= 3 &&
                    p[2] == "getMapItem";
            }),
            new PacketLogFilterViewModel("Quest", p =>
            {
                return p.Length >= 3 &&
                    (p[2] == "getQuest" || p[2] == "acceptQuest" || p[2] == "tryQuestComplete" || p[2] == "updateQuest");
            }),
            new PacketLogFilterViewModel("Shop", p =>
            {
                return p.Length >= 3 &&
                    (p[2] == "loadShop" || p[2] == "buyItem" || p[2] == "sellItem");
            }),
            new PacketLogFilterViewModel("Equip", p =>
            {
                return p.Length >= 3 &&
                    p[2] == "equipItem";
            }),
            new PacketLogFilterViewModel("Drop", p =>
            {
                return p.Length >= 3 &&
                    p[2] == "getDrop";
            }),
            new PacketLogFilterViewModel("Chat", p =>
            {
                return p.Length >= 3 &&
                    (p[2] == "message" || p[2] == "cc");
            }),
            new PacketLogFilterViewModel("Misc", p =>
            {
                return p.Length >= 3 &&
                    (p[2] == "crafting" || p[2] == "setHomeTown" || p[2] == "afk" || p[2] == "summonPet");
            })
        };

        return new PacketLoggerViewModel(filters, s.GetService<IFlashUtil>()!, s.GetService<IFileDialogService>()!);
    }

    private static GrabberViewModel CreateGrabberView(IServiceProvider s)
    {
        IGrabberService grabberService = s.GetService<IGrabberService>()!;
        IDialogService dialogService = s.GetService<IDialogService>()!;
        IScriptInventory inventory = s.GetService<IScriptInventory>()!;
        IScriptShop shops = s.GetService<IScriptShop>()!;
        List<GrabberTaskViewModel> inventoryCommand = new()
        {
            new GrabberTaskViewModel("Equip", async (i, p, t) =>
            {
                if(i is null || i.Count == 0)
                {
                    p.Report("No items found/selected.");
                    return;
                }
                List<ItemBase> items = i.Cast<ItemBase>().ToList();
                p.Report("Equipping items...");
                try
                {
                    foreach (ItemBase item in items)
                    {
                        p.Report($"Equipping {item.Name}.");
                        inventory.EquipItem(item.ID);
                        await Task.Delay(1000, t);
                    }
                }
                catch
                {
                    if(t.IsCancellationRequested)
                    {
                        p.Report("Task cancelled.");
                    }
                }
            }),
            new GrabberTaskViewModel("Sell", async (items, p, t) =>
            {
                if(items is null || items.Count == 0)
                {
                    p.Report("No items found/selected.");
                    return;
                }
                if(items.Count > 1)
                {
                    p.Report("Warning");
                    dialogService.ShowMessageBox($"ATTENTION - {items.Count} items selected!\nPlease sell 1 item at a time to prevent losses.", "Selling item - Warning");
                    return;
                }
                InventoryItem item = items.Cast<InventoryItem>().First();
                if(item.Equipped)
                {
                    dialogService.ShowMessageBox("Cannot sell equipped item.", "Sell item");
                    return;
                }
                p.Report($"Selling {item.Name}, input quantity...");
                try
                {
                    InputDialogViewModel diag = new($"Selling {item.Name}", $"Sell quantity (Currently has: {(item.Category == ItemCategory.Class ? 1 : item.Quantity)})");
                    if (dialogService.ShowDialog(diag) == true)
                    {
                        if(!int.TryParse(diag.DialogTextInput, out int result))
                            return;
                        for(int i = 0; i < result; i++)
                        {
                            shops.SellItem(item.ID);
                            p.Report($"Sold {i}");
                            await Task.Delay(500, t);
                        }
                        p.Report($"Sold {result} {item.Name}");
                    }
                    else
                        p.Report("Cancelled.");
                }
                catch
                {
                    if(t.IsCancellationRequested)
                        p.Report("Task cancelled.");
                }
            })
        };
        List<GrabberTaskViewModel> commands = new()
        {
            new GrabberTaskViewModel("Test", async (o, p, t) =>
            {
                try
                {
                    p.Report("Start");
                    await Task.Delay(3000, t);
                    p.Report("Mid");
                    await Task.Delay(3000, t);
                    p.Report("End");
                    await Task.Delay(10000, t);
                }
                catch
                {
                    p.Report("Task Cancelled");
                    await Task.Delay(3000);
                }
            })
            //new GrabberCommandsViewModel("Suco", new AsyncRelayCommand<IList<object>>(async o => Debug.WriteLine("Suco de Pia")))
        };
        List<GrabberListViewModel> list = new()
        {
            new GrabberListViewModel("Shop Items", grabberService, GrabberTypes.Shop_Items, commands, true),
            new GrabberListViewModel("Shop IDs", grabberService, GrabberTypes.Shop_IDs, commands, false),
            new GrabberListViewModel("Quests", grabberService, GrabberTypes.Quests, commands, true),
            new GrabberListViewModel("Inventory", grabberService, GrabberTypes.Inventory_Items, inventoryCommand, true),
            new GrabberListViewModel("House Inventory", grabberService, GrabberTypes.House_Inventory_Items,commands, true),
            new GrabberListViewModel("Temp Inventory", grabberService, GrabberTypes.Temp_Inventory_Items, commands, false),
            new GrabberListViewModel("Bank Items", grabberService, GrabberTypes.Bank_Items, commands, true),
            new GrabberListViewModel("Cell Monsters", grabberService, GrabberTypes.Cell_Monsters, commands, false),
            new GrabberListViewModel("Map Monsters", grabberService, GrabberTypes.Map_Monsters, commands, true),
            new GrabberListViewModel("GetMap Item IDs", grabberService, GrabberTypes.GetMap_Item_IDs, commands, true)
        };
        return new GrabberViewModel(list, s.GetService<IDialogService>()!);
    }

    private static OptionsViewModel CreateOptionsView(IServiceProvider s)
    {
        IScriptOption options = s.GetService<IScriptOption>()!;
        IDropService dropService = s.GetService<IDropService>()!;
        IFlashUtil flash = s.GetService<IFlashUtil>()!;
        IScriptPlayer player = s.GetService<IScriptPlayer>()!;
        IScriptMap map = s.GetService<IScriptMap>()!;
        List<OptionItemViewModel> optionItems = new()
        {
            CreateOptionItem(s, nameof(options.InfiniteRange), CreateOptionCommand(b => options.InfiniteRange = b)),
            CreateOptionItem(s, nameof(options.SafeTimings), CreateOptionCommand(b => options.SafeTimings = b)),
            CreateOptionItem(s, nameof(options.AggroMonsters), CreateOptionCommand(b => options.AggroMonsters = b)),
            CreateOptionItem(s, nameof(options.AggroAllMonsters), CreateOptionCommand(b => options.AggroAllMonsters = b)),
            CreateOptionItem(s, nameof(options.Magnetise), CreateOptionCommand(b => options.Magnetise = b)),
            CreateOptionItem(s, nameof(options.AttackWithoutTarget), CreateOptionCommand(b => options.AttackWithoutTarget = b)),
            CreateOptionItem(s, nameof(options.PrivateRooms), CreateOptionCommand(b => options.PrivateRooms = b)),
            CreateOptionItem(s, nameof(options.SkipCutscenes), CreateOptionCommand(b => options.SkipCutscenes = b)),
            CreateOptionItem(s, nameof(options.LagKiller), CreateOptionCommand(b => options.LagKiller = b)),
            CreateOptionItem(s, nameof(options.HidePlayers), CreateOptionCommand(b => options.HidePlayers = b)),
            CreateOptionItem(s, nameof(options.AcceptACDrops), CreateOptionCommand(b => options.AcceptACDrops = b)),
            CreateOptionItem(s, nameof(options.DisableFX), CreateOptionCommand(b => options.DisableFX = b)),
            CreateOptionItem(s, nameof(options.DisableDeathAds), CreateOptionCommand(b => options.DisableDeathAds = b)),
            CreateOptionItem(s, nameof(options.DisableCollisions), CreateOptionCommand(b => options.DisableCollisions = b)),
            CreateOptionItem(s, nameof(options.RestPackets), CreateOptionCommand(b => options.RestPackets = b)),
            CreateOptionItem(s, nameof(options.AutoRelogin), CreateOptionCommand(b => options.AutoRelogin = b)),
            CreateOptionItem(s, nameof(options.AutoReloginAny), CreateOptionCommand(b => options.AutoReloginAny = b)),
            CreateOptionItem(s, nameof(options.RetryRelogin), CreateOptionCommand(b => options.RetryRelogin = b)),
            CreateOptionItem(s, nameof(options.SafeRelogin), CreateOptionCommand(b => options.SafeRelogin = b)),
            CreateOptionItem(s, nameof(options.CustomName), new RelayCommand<string>(value => options.CustomName = string.IsNullOrEmpty(value) ? player.Username : value), OptionDisplayType.Text),
            CreateOptionItem(s, nameof(options.CustomGuild), new RelayCommand<string>(value => options.CustomGuild = string.IsNullOrEmpty(value) ? player.Guild : value), OptionDisplayType.Text),
            CreateOptionItem(s, nameof(options.WalkSpeed), new RelayCommand<string>(value =>
            {
                if (!int.TryParse(value, out int result))
                    result = 8;
                options.WalkSpeed = result;
            }), OptionDisplayType.NumericAndButton),
            CreateOptionItem(s, nameof(options.SetFPS), new RelayCommand<string>(value =>
            {
                if (!int.TryParse(value, out int result))
                    result = 30;
                options.SetFPS = result;
            }), OptionDisplayType.NumericAndButton),
        };
        optionItems.Add(new("Accept All Drops", CreateOptionCommand(b =>
        {
            dropService.ToggleAcceptAllDrops(b);
        })));
        optionItems.Add(new("Reject All Drops", CreateOptionCommand(b =>
        {
            dropService.ToggleRejectAllDrops(b);
        })));
        optionItems.Add(new("Upgrade", CreateOptionCommand(b =>
        {
            player.Upgrade = b;
            flash.SetGameObject("world.myAvatar.pMC.pname.ti.textColor", b ? 0x8CD5FF : 0xFFFFFF);
        })));
        optionItems.Add(new("Staff", CreateOptionCommand(b =>
        {
            player.AccessLevel = b ? 100 : 10;
            flash.SetGameObject("world.myAvatar.pMC.pname.ti.textColor", b ? 0xFECB38 : 0xFFFFFF);
        })));
        optionItems.Add(new("FPSCounter", CreateOptionCommand(b =>
        {
            flash.CallGameFunction("world.toggleFPS");
        })));
        optionItems.Add(new("Reload Map", new RelayCommand(() =>
        {
            map.Reload();
        }), OptionDisplayType.Button));
        return new OptionsViewModel(optionItems, s.GetService<IScriptServers>()!, s.GetService<ISettingsService>()!, options);
    }

    private static RelayCommand<bool> CreateOptionCommand(Action<bool> action)
    {
        return new RelayCommand<bool>(action);
    }
    private static OptionItemViewModel CreateOptionItem(IServiceProvider s, string binding, IRelayCommand command, OptionDisplayType displayType = OptionDisplayType.CheckBox)
    {
        return new OptionItemViewModel(binding, s.GetService<IDecamelizer>()!.Decamelize(binding, null), s.GetService<IScriptOption>()!, command, displayType);
    }

    private static BotWindowViewModel CreateBotWindow(IServiceProvider s)
    {
        return new BotWindowViewModel(new BotControlViewModelBase[]
        {
            s.GetService<AdvancedSkillsViewModel>(),
            s.GetService<PacketInterceptorViewModel>(),
            s.GetService<ScriptLoaderViewModel>(),
            s.GetService<ScriptRepoViewModel>(),
            s.GetService<ConsoleViewModel>(),
            s.GetService<PacketSpammerViewModel>(),
            s.GetService<PacketLoggerViewModel>(),
            s.GetService<LoaderViewModel>(),
            s.GetService<GrabberViewModel>(),
            s.GetService<JumpViewModel>(),
            s.GetService<FastTravelViewModel>(),
            s.GetService<OptionsViewModel>(),
            s.GetService<LogsViewModel>()
        });
    }

    private void Flash_FlashCall(string function, params object[] args)
    {
        switch (function)
        {
            case "requestLoadGame":
                Bot.Flash.Call("loadClient", typeof(void), new object[] { null });
                break;
            case "loaded":
                Bot.Flash.FlashCall -= Flash_FlashCall;
                break;
        }
    }
}
