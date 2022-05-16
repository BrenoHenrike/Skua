using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Skua.App.Flash;
using Skua.Core.Interfaces;
using Skua.Core.Interfaces.Skills;
using Skua.Core.Scripts;
using Skua.Core.Skills;
using Skua.Core.ViewModels;

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
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var bot = Ioc.Default.GetService<IScriptInterface>();
        var flash = Ioc.Default.GetService<IFlashUtil>();
        MainWindow main = new(bot, flash);
        main.Show();
        bot.Initialize();
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
        var services = new ServiceCollection();

        services.AddSingleton<IFlashUtil, FlashUtil>();

        services.AddSingleton<IScriptInterface, ScriptInterface>();
        services.AddSingleton<IScriptManager, ScriptManager>();

        services.AddSingleton<IScriptInventory, ScriptInventory>();
        services.AddSingleton<IScriptHouseInv, ScriptHouseInv>();
        services.AddSingleton<IScriptTempInv, ScriptTempInv>();
        services.AddSingleton<IScriptBank, ScriptBank>();

        services.AddSingleton<IAdvancedSkillContainer, AdvancedSkillContainer>();
        services.AddSingleton<IScriptCombat, ScriptCombat>();
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

        services.AddSingleton<MainViewModel>();

        var provider = services.BuildServiceProvider();
        Ioc.Default.ConfigureServices(provider);

        return provider;
    }
}
