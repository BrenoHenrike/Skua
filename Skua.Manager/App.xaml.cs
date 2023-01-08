using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using Skua.Core.AppStartup;
using Skua.WPF.Services;
using Skua.Core.Interfaces;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Messaging;
using Skua.Manager.Properties;
using System.Threading.Tasks;
using Skua.Core.ViewModels.Manager;
using System.Diagnostics;

namespace Skua.Manager;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        var args = Environment.GetCommandLineArgs();
        Services = ConfigureServices();

        _ = Services.GetRequiredService<IThemeService>();
        var settings = Services.GetRequiredService<ISettingsService>();

        for (int i = 0; i < args.Length; i++)
        {
            switch(args[i])
            {
                case "--user-themes":
                    StringCollection themes = new();
                    themes.AddRange(args[i+1].Split('|'));
                    settings.Set("UserThemes", themes);
                    i++;
                    break;
                case "--curr-theme":
                    settings.Set("CurrentTheme", args[++i]);
                    break;
                case "--gh-token":
                    settings.Set("UserGitHubToken", args[++i]);
                    break;
                case "--download-path":
                    settings.Set("ClientDownloadPath", args[++i]);
                    break;
                case "--client-releases":
                    settings.Set("CheckClientUpdates", Convert.ToBoolean(args[++i]));
                    break;
                case "--client-pre-releases":
                    settings.Set("CheckClientPrereleases", Convert.ToBoolean(args[++i]));
                    break;
                case "--delete-zip":
                    settings.Set("DeleteZipFileAfter", Convert.ToBoolean(args[++i]));
                    break;
            }
        }

        Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        StrongReferenceMessenger.Default.Register<App, UpdateFinishedMessage>(this, CloseManager);

        if(Settings.Default.CheckClientUpdates)
        {
            Task.Run(async () =>
            {
                var updateVM = Ioc.Default.GetRequiredService<ClientUpdatesViewModel>();
                await updateVM.Refresh();

                if (updateVM.UpdateVisible && Ioc.Default.GetRequiredService<IDialogService>().ShowMessageBox("New update available, download?", "Update Available", true) == true)
                    await updateVM.Update();
            });
        }
    }

    private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
    {
        StrongReferenceMessenger.Default.Reset();
    }

    private void CloseManager(App recipient, UpdateFinishedMessage message)
    {
        Application.Current.Shutdown();
    }

    // app on exit event
    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        var launcher = Ioc.Default.GetRequiredService<LauncherViewModel>();
        launcher.KillAllSkuaProcesses();
    }

    public new static App Current => (App)Application.Current;

    public IServiceProvider Services { get; }

    private IServiceProvider ConfigureServices()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddWindowsServices();

        services.AddCommonServices();

        services.AddSkuaManagerViewModels();

        services.AddSingleton<ISettingsService, SettingsService>();

        ServiceProvider provider = services.BuildServiceProvider();
        Ioc.Default.ConfigureServices(provider);

        return provider;
    }
}
