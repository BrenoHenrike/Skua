using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.ViewModels;
using System.Windows.Controls;

namespace Skua.WPF.Views;

/// <summary>
/// Interaction logic for LogsView.xaml
/// </summary>
public partial class LogsView : UserControl
{
    public LogsView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<LogsViewModel>();
    }
}