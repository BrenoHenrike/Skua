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
        IWindowService windowService = s.GetRequiredService<IWindowService>();

        //windowService.RegisterManagedWindow("About", s.GetRequiredService<AboutViewModel>());
        windowService.RegisterManagedWindow("Scripts", s.GetRequiredService<ScriptLoaderViewModel>());

        windowService.RegisterManagedWindow("Game", s.GetRequiredService<GameOptionsViewModel>());
        windowService.RegisterManagedWindow("Application", s.GetRequiredService<ApplicationOptionsViewModel>());
        windowService.RegisterManagedWindow("CoreBots", s.GetRequiredService<CoreBotsViewModel>());

        windowService.RegisterManagedWindow("Runtime", s.GetRequiredService<RuntimeHelpersViewModel>());
        windowService.RegisterManagedWindow("Fast Travel", s.GetRequiredService<FastTravelViewModel>());
        windowService.RegisterManagedWindow("Current Drops", s.GetRequiredService<CurrentDropsViewModel>());

        windowService.RegisterManagedWindow("Loader", s.GetRequiredService<LoaderViewModel>());
        windowService.RegisterManagedWindow("Grabber", s.GetRequiredService<GrabberViewModel>());
        windowService.RegisterManagedWindow("Stats", s.GetRequiredService<ScriptStatsViewModel>());
        windowService.RegisterManagedWindow("Console", s.GetRequiredService<ConsoleViewModel>());
        
        windowService.RegisterManagedWindow("Skills", s.GetRequiredService<AdvancedSkillsViewModel>());
        
        windowService.RegisterManagedWindow("Spammer", s.GetRequiredService<PacketSpammerViewModel>());
        windowService.RegisterManagedWindow("Logger", s.GetRequiredService<PacketLoggerViewModel>());
        windowService.RegisterManagedWindow("Interceptor", s.GetRequiredService<PacketInterceptorViewModel>());
        
        windowService.RegisterManagedWindow("Logs", s.GetRequiredService<LogsViewModel>());
        
        windowService.RegisterManagedWindow("Plugins", s.GetRequiredService<PluginsViewModel>());

        windowService.RegisterManagedWindow("Application Themes", s.GetRequiredService<ApplicationThemesViewModel>());

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

        return new MainMenuViewModel(menuItems, s.GetRequiredService<AutoViewModel>(), s.GetRequiredService<JumpViewModel>(), windowService);
    }
}
