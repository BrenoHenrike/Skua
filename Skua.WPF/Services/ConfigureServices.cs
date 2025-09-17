using Microsoft.Extensions.DependencyInjection;
using Skua.Core.Interfaces;
using Skua.Core.Services;
using Skua.WPF.Flash;

namespace Skua.WPF.Services;

public static class ConfigureServices
{
    public static IServiceCollection AddWindowsServices(this IServiceCollection services)
    {
        services.AddSingleton<IFlashUtil, FlashUtil>();

        services.AddSingleton<IDispatcherService, DispatcherService>();
        services.AddSingleton<IClipboardService, ClipboardService>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IWindowService, WindowService>();
        services.AddSingleton<IFileDialogService, FileDialogService>();
        services.AddSingleton<IHotKeyService, HotKeyService>();
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<ISoundService, SoundService>();
        services.AddSingleton<ThemeUserSettingsService>();
        services.AddSingleton<BackgroundThemeService>();

        return services;
    }
}