using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Skua.Core.ViewModels;

namespace Skua.App.Views;
/// <summary>
/// Interaction logic for LogsView.xaml
/// </summary>
public partial class LogsView : UserControl
{
    public LogsView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<LogsViewModel>();
        Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
    }

    private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
    {
        foreach (LogTabItemViewModel vm in ((LogsViewModel)DataContext).LogTabs)
        {
            vm.Dispose();
        }
    }
}
