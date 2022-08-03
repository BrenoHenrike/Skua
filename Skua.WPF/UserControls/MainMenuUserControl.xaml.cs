using System.Windows.Controls;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Skua.Core.ViewModels;

namespace Skua.WPF.UserControls;
/// <summary>
/// Interaction logic for MainMenuUserControl.xaml
/// </summary>
public partial class MainMenuUserControl : UserControl
{
    public MainMenuUserControl()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<MainMenuViewModel>();
    }
}
