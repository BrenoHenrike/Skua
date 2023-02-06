using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.Interfaces;
using Skua.Core.ViewModels;
using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

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

    public void ShowHostWindow<TViewModel>(int width, int height)
        where TViewModel : class
    {
        HostWindow window = new()
        {
            DataContext = _services.GetService<TViewModel>(),
            Height = height,
            Width = width,
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen
        };
        window.Show();
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

        var window = _managedWindows[key];
        if (window.IsVisible)
        {
            window.Activate();
            return;
        }

        window.Show();
        if(window.DataContext is ObservableRecipient recipient)
            recipient.IsActive = true;
    }

    public void RegisterManagedWindow<TViewModel>(string key, TViewModel viewModel) where TViewModel : class, IManagedWindow
    {
        if (_managedWindows.ContainsKey(key))
            return;

        HostWindow window = new()
        {
            DataContext = viewModel,
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen
        };
        _managedWindows.Add(key, window);
    }
}
