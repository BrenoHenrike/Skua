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
using Skua.Core.Utils;
using Westwind.Scripting;
using Skua.Core.AppStartup;
using Skua.WPF;
using System.Threading;

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
        Services.GetRequiredService<IClientFilesService>().CreateDirectories();
        Services.GetRequiredService<IClientFilesService>().CreateFiles();

        _bot = Services.GetRequiredService<IScriptInterface>();
        _bot.Flash.FlashCall += Flash_FlashCall;
        _ = Services.GetRequiredService<ILogService>();
        
        var themes = Services.GetRequiredService<IThemeService>();
        var settings = Services.GetRequiredService<ISettingsService>();
        var args = Environment.GetCommandLineArgs();
        for(int i = 0; i < args.Length; i++)
        {
            switch(args[i])
            {
                case "--usr":
                    if (args[i + 2] != "--psw")
                        break;
                    if (args[i + 4] == "--sv")
                        _server = args[i + 5];
                    _bot.Servers.SetLoginInfo(args[++i], args[++i + 1]);
                    _login = true;
                    break;
                case "--use-theme":
                    string theme = args[++i];
                    if (!string.IsNullOrWhiteSpace(theme) && theme != "no-theme")
                        themes.SetCurrentTheme(ThemeItem.FromString(theme));
                    break;
                case "--gh-token":
                    if (string.IsNullOrEmpty(Settings.Default.UserGitHubToken))
                        Settings.Default.UserGitHubToken = args[++i];
                    Settings.Default.Save();
                    break;
                case "--bot-script-updates":
                    settings.Set("CheckBotScriptsUpdates", Convert.ToBoolean(args[++i]));
                    break;
                case "--auto-update-bot-scripts":
                    settings.Set("AutoUpdateBotScripts", Convert.ToBoolean(args[++i]));
                    break;
                case "--auto-update-advanceskill-sets":
                    settings.Set("AutoUpdateAdvanceSkillSetsUpdates", Convert.ToBoolean(args[++i]));
                    break;
                case "--advanceskill-sets-updates":
                    settings.Set("CheckAdvanceSkillSetsUpdates", Convert.ToBoolean(args[++i]));
                    break;
            }
        }

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

        Ioc.Default.GetRequiredService<IFlashUtil>().Dispose();

        WeakReferenceMessenger.Default.Cleanup();
        WeakReferenceMessenger.Default.Reset();
        StrongReferenceMessenger.Default.Reset();

        RoslynLifetimeManager.ShutdownRoslyn();
        Application.Current.Exit -= App_Exit;
    }

    private readonly IScriptInterface _bot;
    private bool _login = false;
    private string _server = string.Empty;
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        Task.Run(async () => await Ioc.Default.GetRequiredService<IScriptServers>().GetServers());

        if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "VSCode")))
            Settings.Default.UseLocalVSC = false;

        MainWindow main = new();
        main.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Application.Current.MainWindow = main;

        IDialogService dialogService = Services.GetRequiredService<IDialogService>();
        string? token = Settings.Default.UserGitHubToken;
        if (!string.IsNullOrWhiteSpace(token))
            HttpClients.UserGitHubClient = new(token);

        main.Show();

        var getScripts = Ioc.Default.GetRequiredService<IGetScriptsService>();
        if (Settings.Default.CheckBotScriptsUpdates)
        {
            Task.Factory.StartNew(async () =>
            {
                await getScripts.GetScriptsAsync(null, default);
                if ((getScripts.Missing > 0 || getScripts.Outdated > 0)
                    && (Settings.Default.AutoUpdateBotScripts || Ioc.Default.GetRequiredService<IDialogService>().ShowMessageBox("Would you like to update your scripts?", "Script Update", true) == true))
                {
                    int count = await getScripts.DownloadAllWhereAsync(s => !s.Downloaded || s.Outdated);
                    Ioc.Default.GetRequiredService<IDialogService>().ShowMessageBox($"Downloaded {count} scripts.\r\nYou can disable auto script updates in Options > Application.", "Script Update");
                }
            });
        }

        if (Settings.Default.CheckAdvanceSkillSetsUpdates)
        {
            var skillsFileSize = getScripts.GetSkillsSetsTextFileSize();
            var advanceSkillSets = Ioc.Default.GetRequiredService<IAdvancedSkillContainer>();
            Task.Factory.StartNew(async () =>
            {
                if ((skillsFileSize < await getScripts.CheckAdvanceSkillSetsUpdates())
                    && (Settings.Default.AutoUpdateAdvanceSkillSetsUpdates || Ioc.Default.GetRequiredService<IDialogService>().ShowMessageBox("Would you like to update your AdvanceSkill Sets?", "AdvanceSkill Sets Update", true) == true))
                {                
                    if (await getScripts.UpdateSkillSetsFile())
                    {
                        if (Settings.Default.AutoUpdateAdvanceSkillSetsUpdates)
                            Ioc.Default.GetRequiredService<IDialogService>().ShowMessageBox($"AdvanceSkill Sets has been updated.\r\nYou can disable auto AdvanceSkill Sets updates in Options > Application.", "AdvanceSkill Sets Update");
                        else
                            Ioc.Default.GetRequiredService<IDialogService>().ShowMessageBox($"AdvanceSkill Sets has been updated.\r\nYou can enable auto AdvanceSkill Sets updates in Options > Application.", "AdvanceSkill Sets Update");

                        advanceSkillSets.SyncSkills();
                    }
                    else
                    {
                        Ioc.Default.GetRequiredService<IDialogService>().ShowMessageBox($"AdvanceSkill Sets update error.\r\nYou can disable auto AdvanceSkill Sets updates in Options > Application.", "AdvanceSkill Sets Update");
                    }
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

    private void Flash_FlashCall(string function, params object[] args)
    {
        switch (function)
        {
            case "requestLoadGame":
                _bot.Flash.Call("loadClient");
                break;
            case "loaded":
                _bot.Flash.FlashCall -= Flash_FlashCall;
                
                if (!_login)
                    break;

                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(2000);

                    if (string.IsNullOrEmpty(_server))
                    {
                        _bot.Servers.Relogin("Twilly");
                        return;
                    }

                    _bot.Servers.Relogin(_server);
                });
                break;
        }
    }
}
