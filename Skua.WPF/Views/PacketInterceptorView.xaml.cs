using System;
using System.Windows.Controls;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Skua.Core.ViewModels;

namespace Skua.WPF.Views;
/// <summary>
/// Interaction logic for PacketInterceptorView.xaml
/// </summary>
public partial class PacketInterceptorView : UserControl
{
    public PacketInterceptorView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<PacketInterceptorViewModel>()!;
        Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
    }

    private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
    {
        ((PacketInterceptorViewModel)DataContext)?.Dispose();
        Dispatcher.ShutdownStarted -= Dispatcher_ShutdownStarted;
    }
}
