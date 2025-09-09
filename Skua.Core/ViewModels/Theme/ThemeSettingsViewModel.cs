using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;

namespace Skua.Core.ViewModels;

public partial class ThemeSettingsViewModel : ObservableObject
{
    public ThemeSettingsViewModel(IThemeService themeService)
    {
        ThemeService = themeService;
    }

    [ObservableProperty]
    private string _themeName = string.Empty;

    public IThemeService ThemeService { get; }

    [RelayCommand]
    private void SaveTheme()
    {
        ThemeService.SaveTheme(ThemeName);
    }
}