using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Messaging;
using Skua.Core.ViewModels.Manager;
using Skua.WPF;
using System.Windows;
using System.Windows.Shell;

namespace Skua.Manager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : CustomWindow
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<ManagerMainViewModel>();
        StrongReferenceMessenger.Default.Register<MainWindow, ShowMainWindowMessage>(this, ShowManager);
        TitleText = "Skua Manager";
    }

    private void ShowManager(MainWindow recipient, ShowMainWindowMessage message)
    {
        recipient.ShowWindow();
    }

    private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        ShowWindow();
    }

    private void ExitWindow_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var launcher = Ioc.Default.GetRequiredService<LauncherViewModel>();
        launcher.KillAllSkuaProcesses();
        Application.Current.Shutdown();
        WindowChrome.SetWindowChrome(this, null);
    }

    private void LaunchNewSkuaBotClient_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var launcher = Ioc.Default.GetRequiredService<LauncherViewModel>();
        launcher.LaunchSkua().ConfigureAwait(false);
    }

    private void ShowWindow()
    {
        if (IsVisible)
        {
            Hide();
            return;
        }

        Show();
    }
}
