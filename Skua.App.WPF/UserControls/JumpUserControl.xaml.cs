using System.Windows;
using System.Windows.Controls;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Skua.Core.ViewModels;

namespace Skua.App.WPF.UserControls;
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
