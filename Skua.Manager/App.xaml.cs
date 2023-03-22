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
using System.Threading;
using System.IO;
using Skua.Core.ViewModels;
using System.Windows.Navigation;

namespace Skua.Manager;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private const string _uniqueEventName = "Skua.Manager";
    private EventWaitHandle? _eventWaitHandle = null;
    
    public App()
    {
        InitializeComponent();
        SingleInstanceWatcher();

        if (Settings.Default.UpgradeRequired)
        {
            Settings.Default.Upgrade();
            Settings.Default.UpgradeRequired = false;
            Settings.Default.Save();
        }

        Services = ConfigureServices();
        Services.GetRequiredService<IClientFilesService>().CreateDirectories();
        Services.GetRequiredService<IClientFilesService>().CreateFiles();

        _ = Services.GetRequiredService<IThemeService>();
        var settings = Services.GetRequiredService<ISettingsService>();

        Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        StrongReferenceMessenger.Default.Register<App, UpdateFinishedMessage>(this, CloseManager);
        if (Settings.Default.CheckClientUpdates)
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

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        bool isChangeLogActivated = Services.GetRequiredService<ISettingsService>().Get<bool>("ChangeLogActivated");
        if (!isChangeLogActivated)
        {
            Ioc.Default.GetRequiredService<IWindowService>().ShowWindow<ChangeLogsViewModel>(600, 700);
            Services.GetRequiredService<ISettingsService>().Set("ChangeLogActivated", true);
        }
    }

    private void SingleInstanceWatcher()
    {
        try
        {
            _eventWaitHandle = EventWaitHandle.OpenExisting(_uniqueEventName);
            _eventWaitHandle.Set();
            Shutdown();
        }
        catch (WaitHandleCannotBeOpenedException)
        {
            _eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, _uniqueEventName);
        }

        new Task(() =>
        {
            while (_eventWaitHandle.WaitOne())
            {
                Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    if (!Current.MainWindow.Equals(null))
                    {
                        var mainWindow = Current.MainWindow;
                        if (mainWindow.WindowState == WindowState.Minimized || mainWindow.Visibility != Visibility.Visible)
                        {
                            mainWindow.Show();
                            mainWindow.WindowState = WindowState.Normal;
                        }

                        mainWindow.Activate();
                        mainWindow.Topmost = true;
                        mainWindow.Topmost = false;
                        mainWindow.Focus();
                    }
                }));
            }
        })
        .Start();
    }

    private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
    {
        StrongReferenceMessenger.Default.Reset();
    }

    private void CloseManager(App recipient, UpdateFinishedMessage message)
    {
        Application.Current.Shutdown();
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
