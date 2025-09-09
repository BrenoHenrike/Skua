using CommunityToolkit.Mvvm.Messaging;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Skua.Core.GameProxy;
using Skua.Core.Interfaces;
using Skua.Core.Interfaces.Auras;
using Skua.Core.Interfaces.Services;
using Skua.Core.Options;
using Skua.Core.Plugins;
using Skua.Core.Scripts;
using Skua.Core.Scripts.Auras;
using Skua.Core.Services;
using Skua.Core.Skills;
using Skua.Core.Utils;
using Skua.Core.ViewModels;
using Skua.Core.ViewModels.Manager;
using System.Reflection;

namespace Skua.Core.AppStartup;

public static class Services
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services)
    {
        services.AddTransient(typeof(Lazy<>), typeof(LazyInstance<>));

        services.AddSingleton(typeof(IMessenger), WeakReferenceMessenger.Default);
        services.AddSingleton(typeof(StrongReferenceMessenger), StrongReferenceMessenger.Default);

        services.AddSingleton<IDecamelizer, Decamelizer>();
        services.AddSingleton<IGetScriptsService, GetScriptsService>();
        services.AddSingleton<IProcessService, ProcessStartService>();

        return services;
    }

    public static IServiceCollection AddCompiler(this IServiceCollection services)
    {
        services.AddTransient(CreateCompiler);

        return services;
    }

    public static IServiceCollection AddScriptableObjects(this IServiceCollection services)
    {
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
        services.AddSingleton<IScriptSelfAuras, ScriptSelfAuras>();
        services.AddSingleton<IScriptTargetAuras, ScriptTargetAuras>();

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
        services.AddSingleton<IPluginHelper, PluginHelper>();

        services.AddSingleton<IMapService, MapService>();
        services.AddSingleton<ILogService, LogService>();
        services.AddSingleton<IQuestDataLoaderService, QuestDataLoaderService>();
        services.AddSingleton<IGrabberService, GrabberService>();
        services.AddSingleton<IClientFilesService, ClientFilesService>();
        services.AddSingleton<IAuraMonitorService, AuraMonitorService>();

        return services;
    }

    public static IServiceCollection AddSkuaMainAppViewModels(this IServiceCollection services)
    {
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
                s.GetRequiredService<HotKeysViewModel>(),
                s.GetRequiredService<PluginsViewModel>()
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
        services.AddSingleton<PacketInterceptorViewModel>();

        services.AddTransient<ConsoleViewModel>();

        services.AddSingleton<ScriptRepoViewModel>();
        services.AddSingleton<ScriptLoaderViewModel>();

        services.AddSingleton<AdvancedSkillsViewModel>();
        services.AddSingleton<AdvancedSkillEditorViewModel>();
        services.AddSingleton<SavedAdvancedSkillsViewModel>();
        services.AddTransient<SkillRulesViewModel>();

        services.AddSingleton<AutoViewModel>();

        services.AddSingleton<BoostsViewModel>();
        services.AddSingleton<ScriptStatsViewModel>();
        services.AddSingleton<RuntimeHelpersViewModel>();
        services.AddSingleton<NotifyDropViewModel>();
        services.AddSingleton<ToPickupDropsViewModel>();
        services.AddSingleton<RegisteredQuestsViewModel>();
        services.AddSingleton<CurrentDropsViewModel>();

        services.AddThemeViewModels();

        services.AddSingleton<PluginsViewModel>();

        services.AddSingleton<HotKeysViewModel>();
        services.AddSingleton(HotKeys.CreateHotKeys);

        services.AddSingleton(CoreBots.CreateViewModel);
        services.AddSingleton(CoreBots.CreateOptions);
        services.AddSingleton<CBOClassEquipmentViewModel>();
        services.AddSingleton<CBOClassSelectViewModel>();
        services.AddSingleton<CBOLoadoutViewModel>();

        return services;
    }

    public static IServiceCollection AddThemeViewModels(this IServiceCollection services)
    {
        services.AddSingleton<ApplicationThemesViewModel>();
        services.AddSingleton<ThemeSettingsViewModel>();
        services.AddSingleton<ColorSchemeEditorViewModel>();

        return services;
    }

    public static IServiceCollection AddSkuaManagerViewModels(this IServiceCollection services)
    {
        services.AddThemeViewModels();
        services.AddSingleton<AccountManagerViewModel>();
        services.AddSingleton<LauncherViewModel>();
        services.AddSingleton<IClientUpdateService, ClientUpdateService>();
        services.AddSingleton<IClientFilesService, ClientFilesService>();
        services.AddSingleton<ClientUpdatesViewModel>();
        services.AddSingleton<GitHubAuthViewModel>();
        services.AddSingleton<ScriptRepoManagerViewModel>();
        services.AddSingleton<GoalsViewModel>();
        services.AddSingleton<AboutViewModel>();
        services.AddSingleton<ChangeLogsViewModel>();
        services.AddSingleton(SkuaManager.CreateViewModel);
        services.AddSingleton(SkuaManager.CreateOptionsViewModel);

        return services;
    }

    private static Compiler CreateCompiler(IServiceProvider s)
    {
        Compiler compiler = new();
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
        compiler.AddAssemblies(refPaths.Select(s => MetadataReference.CreateFromFile(s)));
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
}