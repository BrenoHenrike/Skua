using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Skua.Core.Interfaces;
using Skua.Core.ViewModels;
using Skua.Core.ViewModels.Manager;

namespace Skua.Core.AppStartup;
internal class SkuaManager
{
    internal static ManagerMainViewModel CreateViewModel(IServiceProvider s)
    {
        List<TabItemViewModel> tabs = new()
        {
            new("Launcher", s.GetRequiredService<LauncherViewModel>()),
            new("Updates", s.GetRequiredService<ClientUpdatesViewModel>()),
            new("Options", s.GetRequiredService<ManagerOptionsViewModel>()),
            new("Themes", s.GetRequiredService<ApplicationThemesViewModel>()),
            new("Goals", s.GetRequiredService<GoalsViewModel>()),
        };
        return new(tabs, s.GetRequiredService<IDialogService>(), s.GetRequiredService<ISettingsService>());
    }

    internal static ManagerOptionsViewModel CreateOptionsViewModel(IServiceProvider s)
    {
        List<DisplayOptionItemViewModelBase> options = new()
        {
            CreateSettingOptionItem<bool>("Use Manager theme on Skua", "Whether to use, when launching from the Launcher tab, the same theme as the Manager in any launched App", "SyncThemes"),
            CreateSettingOptionItem<bool>("Auto Update Scripts", "Whether to auto update scripts when launching the Manager, needs \"Check for Scripts updates\" to be true", "AutoUpdateScripts"),
            CreateSettingOptionItem<bool>("Check for Client Updates", "Whether to check for client updates when launching the Manager", "CheckClientUpdates"),
            CreateSettingOptionItem<bool>("Check for Client Prereleases", "Whether to check for pre-releases when checking updates", "CheckClientPrereleases"),
            CreateSettingOptionItem<bool>("Check for Script Updates", "Whether to check for scripts updates when launching the Manager", "CheckScriptUpdates"),
            CreateSettingOptionItem<bool>("Delete .zip after Download", "Whether to delete the .zip folder after downloading and extracting the new version", "DeleteZipFileAfter")
        };

        return new(options, s.GetRequiredService<ISettingsService>(), s.GetRequiredService<IFileDialogService>());

        static RelayCommand<T> CreateSettingCommand<T>(string key)
        {
            return new RelayCommand<T>(b => Ioc.Default.GetRequiredService<ISettingsService>().Set(key, b));
        }
        static CommandOptionItemViewModel<T> CreateSettingOptionItem<T>(string content, string description, string key)
        {
            return new(content, description, key, CreateSettingCommand<T>(key), Ioc.Default.GetRequiredService<ISettingsService>().Get<T>(key));
        }
    }
}
