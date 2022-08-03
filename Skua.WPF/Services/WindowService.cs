using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Skua.Core.Interfaces;
using Skua.Core.ViewModels;
using System;
using System.Collections.Generic;

namespace Skua.WPF.Services;
public class WindowService : IWindowService
{
    private Dictionary<string, HostWindow> _managedWindows = new();
    public WindowService(IServiceProvider services)
    {
        _services = services;
    }

    private readonly IServiceProvider _services;

    public void ShowWindow<TViewModel>()
        where TViewModel : class
    {
        if(typeof(TViewModel) == typeof(BotWindowViewModel))
        {
            BotWindow botWindow = new();
            botWindow.DataContext = _services.GetService<TViewModel>();
            botWindow.Show();
            return;
        }

        HostWindow hostWindow = new();
        hostWindow.DataContext = _services.GetService<TViewModel>();
        hostWindow.Show();
    }

    public void ShowWindow<TViewModel>(TViewModel viewModel)
        where TViewModel : class
    {
        HostWindow window = new();
        window.DataContext = viewModel;
        window.Show();
    }

    public void ShowManagedWindow(string key)
    {
        if (!_managedWindows.ContainsKey(key))
            return;

        if (_managedWindows[key].IsVisible)
        {
            _managedWindows[key].Activate();
            return;
        }

        _managedWindows[key].Show();
    }

    public void RegisterManagedWindow<TViewModel>(string key, TViewModel viewModel) where TViewModel : class, IManagedWindow
    {
        if (_managedWindows.ContainsKey(key))
            return;
        HostWindow window = new();
        window.DataContext = viewModel;
        window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        _managedWindows.Add(key, window);
    }
}
