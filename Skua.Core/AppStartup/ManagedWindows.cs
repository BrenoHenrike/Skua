using Microsoft.Extensions.DependencyInjection;
using Skua.Core.Interfaces;
using Skua.Core.ViewModels;
using Skua.Core.ViewModels.Manager;

namespace Skua.Core.AppStartup;

public class ManagedWindows
{
    public static void Register(IServiceProvider s)
    {
        IWindowService windowService = s.GetRequiredService<IWindowService>();

        //windowService.RegisterManagedWindow("About", s.GetRequiredService<AboutViewModel>());
        windowService.RegisterManagedWindow("Scripts", s.GetRequiredService<ScriptLoaderViewModel>());
        windowService.RegisterManagedWindow("Script Repo", s.GetRequiredService<ScriptRepoViewModel>());

        windowService.RegisterManagedWindow("Game", s.GetRequiredService<GameOptionsViewModel>());
        windowService.RegisterManagedWindow("Application", s.GetRequiredService<ApplicationOptionsViewModel>());
        windowService.RegisterManagedWindow("CoreBots", s.GetRequiredService<CoreBotsViewModel>());
        windowService.RegisterManagedWindow("Application Themes", s.GetRequiredService<ApplicationThemesViewModel>());
        windowService.RegisterManagedWindow("HotKeys", s.GetRequiredService<HotKeysViewModel>());

        windowService.RegisterManagedWindow("Runtime", s.GetRequiredService<RuntimeHelpersViewModel>());
        windowService.RegisterManagedWindow("Notify Drop", s.GetRequiredService<NotifyDropViewModel>());
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
    }

    public static void RegisterForManager(IServiceProvider s)
    {
        IWindowService windowService = s.GetRequiredService<IWindowService>();

        windowService.RegisterManagedWindow("Script Repo Manager", s.GetRequiredService<ScriptRepoManagerViewModel>());
        windowService.RegisterManagedWindow("Application Themes", s.GetRequiredService<ApplicationThemesViewModel>());
    }
}