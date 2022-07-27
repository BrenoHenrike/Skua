using System;
using System.Windows.Controls;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Skua.Core.ViewModels;

namespace Skua.App.WPF.Views;
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
