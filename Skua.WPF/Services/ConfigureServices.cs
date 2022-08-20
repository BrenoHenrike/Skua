using Microsoft.Extensions.DependencyInjection;
using Skua.Core.Interfaces;
using Skua.WPF.Flash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        services.AddSingleton<ThemeUserSettingsService>();

        return services;
    }
}
