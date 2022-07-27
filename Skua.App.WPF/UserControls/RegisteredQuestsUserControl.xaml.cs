using System;
using System.Windows.Controls;

namespace Skua.App.WPF.UserControls;
/// <summary>
/// Interaction logic for RegisteredQuestsView.xaml
/// </summary>
public partial class RegisteredQuestsUserControl : UserControl
{
    public RegisteredQuestsUserControl()
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
