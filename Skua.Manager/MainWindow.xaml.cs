using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Messaging;
using Skua.Core.ViewModels.Manager;
using Skua.WPF;

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
    }

    private void ShowManager(MainWindow recipient, ShowMainWindowMessage message)
    {
        recipient.ShowWindow();
    }

    private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        ShowWindow();
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
