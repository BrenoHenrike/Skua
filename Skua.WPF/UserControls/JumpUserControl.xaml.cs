using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.ViewModels;

namespace Skua.WPF.UserControls;
/// <summary>
/// Interaction logic for JumpUserControl.xaml
/// </summary>
public partial class JumpUserControl : UserControl
{
    public JumpUserControl()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<JumpViewModel>();
    }
}
