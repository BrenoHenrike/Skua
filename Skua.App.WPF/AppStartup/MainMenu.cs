using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.ViewModels;
using System;
using System.Collections.Generic;

namespace Skua.App.WPF.AppStartup;
internal class MainMenu
{
    internal static MainMenuViewModel CreateViewModel(IServiceProvider s)
    {
        ManagedWindows.Register(s);

        List<MainMenuItemViewModel> menuItems = new()
        {
            new("Scripts"),
            new("Options", new List<MainMenuItemViewModel>()
            {
                new("Game"),
                new("Application"),
                new("CoreBots"),
                new("Application Themes")
            }),
            new("Helpers", new List<MainMenuItemViewModel>()
            {
                new("Runtime"),
                new("Fast Travel"),
                new("Current Drops")
            }),
            new("Tools", new List<MainMenuItemViewModel>()
            {
                new("Loader"),
                new("Grabber"),
                new("Stats"),
                new("Console"),
            }),
            new("Skills"),
            new("Packets", new List<MainMenuItemViewModel>()
            {
                new("Spammer"),
                new("Logger"),
                new("Interceptor")
            }),
            new("Bank", new RelayCommand(Ioc.Default.GetRequiredService<IScriptBank>().Open)),
            new("Logs")
        };

        return new MainMenuViewModel(menuItems, s.GetRequiredService<AutoViewModel>(), s.GetRequiredService<JumpViewModel>(), s.GetRequiredService<IWindowService>());
    }
}
