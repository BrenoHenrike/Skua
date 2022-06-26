using System;
using System.Windows.Controls;
using System.Windows.Input;
using Skua.Core.Utils;

namespace Skua.App.UserControls;
/// <summary>
/// Interaction logic for OptionUserControl.xaml
/// </summary>
public partial class OptionUserControl : UserControl
{
    public OptionUserControl()
    {
        InitializeComponent();
        Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
    }

    private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
    {
        ((IDisposable)DataContext).Dispose();
    }
}
