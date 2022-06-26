using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Skua.Core.Interfaces.Services;
using Skua.Core.ViewModels;
using Skua.WPF;

namespace Skua.App.Services;
public class WindowService : IWindowService
{
    //private readonly Dictionary<Type, Type> _windowMaps = new()
    //{
    //    { typeof(BotWindowViewModel), typeof(BotWindow) }
    //};

    //private readonly Dictionary<Type, Type> _viewModelMaps = new()
    //{
    //    {typeof() }
    //};
    public void ShowWindow<TViewModel>()
        where TViewModel : class
    {
        BotWindow window = new();
        window.DataContext = Ioc.Default.GetService<TViewModel>();
        window.Show();
    }
}
