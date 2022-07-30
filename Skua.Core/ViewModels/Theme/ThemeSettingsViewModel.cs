using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Skua.Core.Interfaces;

namespace Skua.Core.ViewModels;
public partial class ThemeSettingsViewModel : ObservableObject
{
    public ThemeSettingsViewModel(IThemeService themeService)
    {
        ThemeService = themeService;
        SaveThemeCommand = new RelayCommand(SaveTheme);
    }

    private void SaveTheme()
    {
        ThemeService.SaveTheme(ThemeName);
    }

    [ObservableProperty]
    private string _themeName = string.Empty;
    public IThemeService ThemeService { get; }

    public IRelayCommand SaveThemeCommand { get; }
}
