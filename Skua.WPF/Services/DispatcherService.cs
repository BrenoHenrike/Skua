using Skua.Core.Interfaces;
using System;
using System.Windows;

namespace Skua.WPF.Services;
public class DispatcherService : IDispatcherService
{
    public void Invoke(Action action)
    {
        Application.Current?.Dispatcher.Invoke(action);
    }
}
