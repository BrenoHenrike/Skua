using System;
using System.Windows.Controls;

namespace Skua.App.WPF.UserControls;
/// <summary>
/// Interaction logic for ToPickupDropsUserControl.xaml
/// </summary>
public partial class ToPickupDropsUserControl : UserControl
{
    public ToPickupDropsUserControl()
    {
        InitializeComponent();
        Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
    }

    private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
    {
        ((IDisposable)DataContext)?.Dispose();
        Dispatcher.ShutdownStarted -= Dispatcher_ShutdownStarted;
    }
}
