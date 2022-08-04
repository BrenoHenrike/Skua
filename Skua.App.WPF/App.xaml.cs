using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using Skua.WPF.Flash;
using Skua.WPF.Services;
using Skua.App.WPF.AppStartup;
using Skua.App.WPF.Properties;
using Skua.App.WPF.Services;
using Skua.Core;
using Skua.Core.GameProxy;
using Skua.Core.Interfaces;
using Skua.Core.Options;
using Skua.Core.Plugins;
using Skua.Core.Scripts;
using Skua.Core.Services;
using Skua.Core.Skills;
using Skua.Core.Utils;
using Skua.Core.ViewModels;
using Westwind.Scripting;

namespace Skua.App.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public sealed partial class App : Application
{
    public App()
    {
        InitializeComponent();
        Services = ConfigureServices();
        _bot = Services.GetRequiredService<IScriptInterface>();
        _bot.Flash.FlashCall += Flash_FlashCall;
        _ = Services.GetRequiredService<ILogService>();
        _ = Services.GetRequiredService<IThemeService>();
        Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = Services.GetRequiredService<ISettingsService>().Get<int>("AnimationFrameRate") });
    }

    private async void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
    {
        Services.GetRequiredService<ICaptureProxy>().Stop();
        
        await ((IAsyncDisposable)Services.GetRequiredService<IScriptBoost>()).DisposeAsync();
        await ((IAsyncDisposable)Services.GetRequiredService<IScriptBotStats>()).DisposeAsync();
        await ((IAsyncDisposable)Services.GetRequiredService<IScriptDrop>()).DisposeAsync();
        WeakReferenceMessenger.Default.Cleanup();
        WeakReferenceMessenger.Default.Reset();

        Dispatcher.ShutdownStarted -= Dispatcher_ShutdownStarted;
    }

    private readonly IScriptInterface _bot;
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        Task.Run(async () => await Ioc.Default.GetRequiredService<IScriptServers>().GetServers());

        MainWindow main = new(_bot);
        main.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Application.Current.MainWindow = main;

        if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "VSCode")))
            Settings.Default.UseLocalVSC = false;

        IDialogService dialogService = Services.GetRequiredService<IDialogService>();
        string? token = Settings.Default.UserGitHubToken;
        if (string.IsNullOrWhiteSpace(token))
        {
            if (Settings.Default.IgnoreGHAuth)
                return;

            dialogService.ShowDialog(Services.GetRequiredService<GitHubAuthViewModel>());

            token = Settings.Default.UserGitHubToken;
            if (!string.IsNullOrWhiteSpace(token))
                HttpClients.UserGitHubClient = new(token);
        }
        else
            HttpClients.UserGitHubClient = new(token);

        main.Show();

        if (Settings.Default.CheckScriptUpdates)
        {
            Task.Run(async () =>
            {
                var getScripts = Ioc.Default.GetRequiredService<IGetScriptsService>();
                await getScripts.GetScriptsAsync(null, default);
                if (getScripts.Missing > 0 && (Settings.Default.AutoUpdateScripts || Ioc.Default.GetRequiredService<IDialogService>().ShowMessageBox("Would you like to update your scripts?", "Script Update", true) == true))
                {
                    int count = await getScripts.DownloadAllWhereAsync(s => !s.Downloaded || s.Outdated);
                    Ioc.Default.GetRequiredService<IDialogService>().ShowMessageBox($"Downloaded {count} scripts.\r\nYou can disable auto script updates in Options.", "Script Update");
                }
            });
        }

        Services.GetRequiredService<IPluginManager>().Initialize();
    }

    /// <summary>
    /// Gets the current <see cref="App"/> instance in use
    /// </summary>
    public new static App Current => (App)Application.Current;

    public IServiceProvider Services { get; }
    /// <summary>
    /// Configures the services for the application.
    /// </summary>

    private IServiceProvider ConfigureServices()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddTransient(typeof(Lazy<>), typeof(LazyInstance<>));

        services.AddSingleton(typeof(IMessenger), GetMessenger());
        services.AddSingleton<IFlashUtil, FlashUtil>();

        services.AddSingleton<IScriptInterface, ScriptInterface>();
        services.AddSingleton<IScriptManager, ScriptManager>();
        services.AddSingleton<IScriptStatus, ScriptManager>();

        services.AddSingleton<IScriptInventoryHelper, ScriptInventoryHelper>();
        services.AddSingleton<IScriptInventory, ScriptInventory>();
        services.AddSingleton<IScriptHouseInv, ScriptHouseInv>();
        services.AddSingleton<IScriptTempInv, ScriptTempInv>();
        services.AddSingleton<IScriptBank, ScriptBank>();

        services.AddSingleton<IAdvancedSkillContainer, AdvancedSkillContainer>();
        services.AddSingleton<IScriptCombat, ScriptCombat>();
        services.AddSingleton<IScriptKill, ScriptKill>();
        services.AddSingleton<IScriptHunt, ScriptHunt>();
        services.AddSingleton<IScriptSkill, ScriptSkill>();
        services.AddSingleton<IScriptAuto, ScriptAuto>();

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

        services.AddTransient<IScriptOptionContainer, ScriptOptionContainer>();
        services.AddTransient<IOptionContainer, OptionContainer>();
        services.AddSingleton<IScriptOption, ScriptOption>();
        services.AddSingleton<IScriptLite, ScriptLite>();

        services.AddSingleton<IScriptBotStats, ScriptBotStats>();
        services.AddSingleton<IScriptHandlers, ScriptHandlers>();
        services.AddSingleton<IScriptWait, ScriptWait>();

        services.AddSingleton<ICaptureProxy, CaptureProxy>();

        services.AddSingleton<IPluginManager, PluginManager>();
        services.AddTransient<IPluginContainer, PluginContainer>();

        services.AddSingleton<IMapService, MapService>();
        services.AddSingleton<ILogService, LogService>();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<IDecamelizer, Decamelizer>();
        services.AddSingleton<IClipboardService, ClipboardService>();
        services.AddSingleton<IQuestDataLoaderService, QuestDataLoaderService>();
        services.AddSingleton<IGrabberService, GrabberService>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IWindowService, WindowService>();
        services.AddSingleton<IGetScriptsService, GetScriptsService>();
        services.AddSingleton<IFileDialogService, FileDialogService>();
        services.AddSingleton<IProcessStartService, ProcessStartService>();
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<ThemeUserSettingsService>();

        services.AddSingleton<MainViewModel>();
        services.AddSingleton(MainMenu.CreateViewModel);
        services.AddTransient<BotWindowViewModel>();
        services.AddSingleton<IEnumerable<BotControlViewModelBase>>(s =>
        {
            return new List<BotControlViewModelBase>()
            {
                s.GetRequiredService<ScriptLoaderViewModel>(),
                s.GetRequiredService<ScriptRepoViewModel>(),
                s.GetRequiredService<LogsViewModel>(),
                s.GetRequiredService<AutoViewModel>(),
                s.GetRequiredService<JumpViewModel>(),
                s.GetRequiredService<FastTravelViewModel>(),
                s.GetRequiredService<CurrentDropsViewModel>(),
                s.GetRequiredService<RuntimeHelpersViewModel>(),
                s.GetRequiredService<LoaderViewModel>(),
                s.GetRequiredService<GrabberViewModel>(),
                s.GetRequiredService<GameOptionsViewModel>(),
                s.GetRequiredService<ApplicationOptionsViewModel>(),
                s.GetRequiredService<ConsoleViewModel>(),
                s.GetRequiredService<AdvancedSkillsViewModel>(),
                s.GetRequiredService<PacketInterceptorViewModel>(),
                s.GetRequiredService<PacketSpammerViewModel>(),
                s.GetRequiredService<PacketLoggerViewModel>(),
                s.GetRequiredService<ApplicationThemesViewModel>(),
                s.GetRequiredService<GitHubAuthViewModel>(),
                s.GetRequiredService<PluginsViewModel>(),
            };
        });
        services.AddTransient<LoaderViewModel>();
        services.AddTransient(Grabber.CreateViewModel);
        services.AddSingleton(Grabber.CreateListViewModels);
        services.AddSingleton<JumpViewModel>();
        services.AddSingleton<FastTravelViewModel>();
        services.AddTransient<FastTravelEditorViewModel>();
        services.AddTransient<FastTravelEditorDialogViewModel>();
        services.AddSingleton<LogsViewModel>();
        services.AddSingleton(LogTabs.CreateViewModels);
        services.AddSingleton(Options.CreateGameOptions);
        services.AddSingleton(Options.CreateAppOptions);
        services.AddSingleton(PacketLogger.CreateViewModel);
        services.AddSingleton<PacketSpammerViewModel>();
        services.AddTransient<ConsoleViewModel>();
        services.AddSingleton<ScriptRepoViewModel>();
        services.AddSingleton<ScriptLoaderViewModel>();
        services.AddSingleton<PacketInterceptorViewModel>();
        services.AddSingleton<AdvancedSkillsViewModel>();
        services.AddSingleton<AdvancedSkillEditorViewModel>();
        services.AddSingleton<SavedAdvancedSkillsViewModel>();
        services.AddTransient<SkillRulesViewModel>();
        services.AddSingleton<AutoViewModel>();
        services.AddSingleton<BoostsViewModel>();
        services.AddSingleton<ScriptStatsViewModel>();
        services.AddSingleton<RuntimeHelpersViewModel>();
        services.AddSingleton<ToPickupDropsViewModel>();
        services.AddSingleton<RegisteredQuestsViewModel>();
        services.AddSingleton<CurrentDropsViewModel>();
        services.AddSingleton<GitHubAuthViewModel>();
        services.AddSingleton<ApplicationThemesViewModel>();
        services.AddSingleton<ThemeSettingsViewModel>();
        services.AddSingleton<ColorSchemeEditorViewModel>();
        services.AddSingleton<PluginsViewModel>();

        services.AddSingleton(CoreBots.CreateViewModel);
        services.AddSingleton(CoreBots.CreateOptions);
        services.AddSingleton<CBOClassEquipmentViewModel>();
        services.AddSingleton<CBOClassSelectViewModel>();
        services.AddSingleton<CBOLoadoutViewModel>();

        services.AddTransient(CreateCompiler);

        ServiceProvider provider = services.BuildServiceProvider();
        Ioc.Default.ConfigureServices(provider);

        return provider;
    }

    private static WeakReferenceMessenger GetMessenger()
    {
        return WeakReferenceMessenger.Default;
    }

    private CSharpScriptExecution CreateCompiler(IServiceProvider s)
    {
        CSharpScriptExecution compiler = new();
        var refPaths = new[]
        {
            typeof(object).GetTypeInfo().Assembly.Location,
            typeof(Console).GetTypeInfo().Assembly.Location,
            typeof(object).Assembly.Location,
            typeof(Enumerable).Assembly.Location,
            typeof(ScriptManager).Assembly.Location,
            Path.Combine(Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location)!, "System.Runtime.dll")
        };
        var refs = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic)
            .Select(a => a.Location)
            .Where(s => !string.IsNullOrEmpty(s))
            .Where(s => !s.Contains("xunit"))
            .Select(s => MetadataReference.CreateFromFile(s))
            .ToList();
        compiler.AddAssemblies(refs);
        compiler.AddAssemblies(refPaths.Select(s=> MetadataReference.CreateFromFile(s)));
        compiler.AddNamespaces(new[]
        {
            "Skua.Core",
            "Skua.Core.Interfaces",
            "Skua.Core.Models",
            "Skua.Core.Models.Items",
            "Skua.Core.Models.Monsters",
            "Skua.Core.Models.Players",
            "Skua.Core.Models.Quests",
            "Skua.Core.Models.Servers",
            "Skua.Core.Models.Shops",
            "Skua.Core.Models.Skills",
        });
        compiler.SaveGeneratedCode = true;
        return compiler;
    }

    private void Flash_FlashCall(string function, params object[] args)
    {
        switch (function)
        {
            case "requestLoadGame":
                _bot.Flash.Call("loadClient");
                break;
            case "loaded":
                _bot.Flash.FlashCall -= Flash_FlashCall;
                break;
        }
    }
}
