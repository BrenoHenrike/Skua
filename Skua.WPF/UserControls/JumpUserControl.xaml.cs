using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.ViewModels;
using System.Windows.Controls;

namespace Skua.WPF.UserControls;

/// <summary>
/// Interaction logic for JumpUserControl.xaml
/// </summary>
public partial class JumpUserControl : UserControl
{
    public JumpUserControl()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<JumpViewModel>();
    }
}