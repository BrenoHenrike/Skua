using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.Interfaces;
using Skua.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace Skua.WPF.Services;
public class WindowService : IWindowService, IDisposable
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

    public void ShowWindow<TViewModel>(int width, int height)
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
        
        // Clean up when window is closed
        void windowClosedHandler(object? sender, EventArgs e)
        {
            if (sender is Window w)
            {
                w.Closed -= windowClosedHandler;
                _managedWindows.Remove(key);
                
                // Clean up DataContext
                if (w.DataContext is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                w.DataContext = null;
            }
        }
        
        window.Closed += windowClosedHandler;
        _managedWindows.Add(key, window);
    }

    private bool _disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Close and dispose all managed windows
                foreach (var kvp in _managedWindows.ToList())
                {
                    try
                    {
                        kvp.Value.Close();
                        if (kvp.Value.DataContext is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                    }
                    catch { }
                }
                _managedWindows.Clear();
            }

            _disposed = true;
        }
    }

    ~WindowService()
    {
        Dispose(false);
    }
}
