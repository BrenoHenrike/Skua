using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Skua.WPF.Services;
using Skua.App.WPF.Properties;
using Skua.App.WPF.Services;
using Skua.Core.Interfaces;
using Westwind.Scripting;
using Skua.Core.AppStartup;
using Skua.WPF;

namespace Skua.App.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public sealed partial class App : Application
{
    /// <summary>
    /// Gets the current <see cref="App"/> instance in use
    /// </summary>
    public new static App Current => (App)Application.Current;
    public IServiceProvider Services { get; }
    private readonly IScriptInterface _bot;
    
    public App()
    {
        InitializeComponent();

        if (Settings.Default.UpgradeRequired)
        {
            Settings.Default.Upgrade();
            Settings.Default.UpgradeRequired = false;
            Settings.Default.Save();
        }

        Services = ConfigureServices();
        Services.GetRequiredService<IClientFilesService>().CreateDirectories();
        Services.GetRequiredService<IClientFilesService>().CreateFiles();
        Task.Factory.StartNew(async () => await Services.GetRequiredService<IScriptServers>().GetServers());
        
        _bot = Services.GetRequiredService<IScriptInterface>();
        _ = Services.GetRequiredService<ILogService>();
        
        var args = Environment.GetCommandLineArgs();
        var startup = new SkuaStartupHandler(args, _bot, Services.GetRequiredService<ISettingsService>(), Services.GetRequiredService<IThemeService>());
        startup.Execute();

        RoslynLifetimeManager.WarmupRoslyn();
        Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = Services.GetRequiredService<ISettingsService>().Get<int>("AnimationFrameRate") });
        
        Application.Current.Exit += App_Exit;
    }

    private async void App_Exit(object? sender, EventArgs e)
    {
        Services.GetRequiredService<ICaptureProxy>().Stop();

        await ((IAsyncDisposable)Services.GetRequiredService<IScriptBoost>()).DisposeAsync();
        await ((IAsyncDisposable)Services.GetRequiredService<IScriptBotStats>()).DisposeAsync();
        await ((IAsyncDisposable)Services.GetRequiredService<IScriptDrop>()).DisposeAsync();
        await Ioc.Default.GetRequiredService<IScriptManager>().StopScriptAsync();
        await ((IScriptInterfaceManager)_bot).StopTimerAsync();

        Services.GetRequiredService<IFlashUtil>().Dispose();

        WeakReferenceMessenger.Default.Cleanup();
        WeakReferenceMessenger.Default.Reset();
        StrongReferenceMessenger.Default.Reset();

        RoslynLifetimeManager.ShutdownRoslyn();
        Application.Current.Exit -= App_Exit;
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "VSCode")))
            Settings.Default.UseLocalVSC = false;

        MainWindow main = new();
        main.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Application.Current.MainWindow = main;
        main.Show();
        
        IDialogService dialogService = Services.GetRequiredService<IDialogService>();
        var getScripts = Services.GetRequiredService<IGetScriptsService>();
        if (Settings.Default.CheckBotScriptsUpdates)
        {
            Task.Factory.StartNew(async () =>
            {
                await getScripts.GetScriptsAsync(null, default);
                if ((getScripts.Missing > 0 || getScripts.Outdated > 0)
                    && (Settings.Default.AutoUpdateBotScripts || Services.GetRequiredService<IDialogService>().ShowMessageBox("Would you like to update your scripts?", "Script Update", true) == true))
                {
                    int count = await getScripts.DownloadAllWhereAsync(s => !s.Downloaded || s.Outdated);
                    Services.GetRequiredService<IDialogService>().ShowMessageBox($"Downloaded {count} scripts.\r\nYou can disable auto script updates in Options > Application.", "Script Update");
                }
            });
        }

        if (Settings.Default.CheckAdvanceSkillSetsUpdates)
        {
            var skillsFileSize = getScripts.GetSkillsSetsTextFileSize();
            var advanceSkillSets = Services.GetRequiredService<IAdvancedSkillContainer>();
            Task.Factory.StartNew(async () =>
            {
                if ((skillsFileSize < await getScripts.CheckAdvanceSkillSetsUpdates())
                    && (Settings.Default.AutoUpdateAdvanceSkillSetsUpdates || Services.GetRequiredService<IDialogService>().ShowMessageBox("Would you like to update your AdvanceSkill Sets?", "AdvanceSkill Sets Update", true) == true))
                {                
                    if (await getScripts.UpdateSkillSetsFile())
                    {
                        if (Settings.Default.AutoUpdateAdvanceSkillSetsUpdates)
                            Services.GetRequiredService<IDialogService>().ShowMessageBox($"AdvanceSkill Sets has been updated.\r\nYou can disable auto AdvanceSkill Sets updates in Options > Application.", "AdvanceSkill Sets Update");
                        else
                            Services.GetRequiredService<IDialogService>().ShowMessageBox($"AdvanceSkill Sets has been updated.\r\nYou can enable auto AdvanceSkill Sets updates in Options > Application.", "AdvanceSkill Sets Update");

                        advanceSkillSets.SyncSkills();
                    }
                    else
                    {
                        Services.GetRequiredService<IDialogService>().ShowMessageBox($"AdvanceSkill Sets update error.\r\nYou can disable auto AdvanceSkill Sets updates in Options > Application.", "AdvanceSkill Sets Update");
                    }
                }
            });
        }

        Services.GetRequiredService<IPluginManager>().Initialize();
    }

    /// <summary>
    /// Configures the services for the application.
    /// </summary>
    private IServiceProvider ConfigureServices()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddSingleton<ISettingsService, SettingsService>();

        services.AddWindowsServices();

        services.AddCommonServices();

        services.AddScriptableObjects();

        services.AddCompiler();

        services.AddSkuaMainAppViewModels();

        ServiceProvider provider = services.BuildServiceProvider();
        Ioc.Default.ConfigureServices(provider);

        return provider;
    }
}
