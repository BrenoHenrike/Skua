using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.ViewModels;
using System.Windows.Controls;

namespace Skua.WPF.UserControls;

/// <summary>
/// Interaction logic for MainMenuUserControl.xaml
/// </summary>
public partial class MainMenuUserControl : UserControl
{
    public MainMenuUserControl()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<MainMenuViewModel>();
    }
}