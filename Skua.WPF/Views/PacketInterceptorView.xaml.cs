using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.ViewModels;
using System.Windows.Controls;

namespace Skua.WPF.Views;

/// <summary>
/// Interaction logic for PacketInterceptorView.xaml
/// </summary>
public partial class PacketInterceptorView : UserControl
{
    public PacketInterceptorView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<PacketInterceptorViewModel>();
    }
}