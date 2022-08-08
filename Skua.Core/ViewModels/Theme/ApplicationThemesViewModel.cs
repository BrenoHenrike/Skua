using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;

namespace Skua.Core.ViewModels;
public class ApplicationThemesViewModel : BotControlViewModelBase
{
    public ApplicationThemesViewModel(IThemeService themeService, ThemeSettingsViewModel themeSettings, ColorSchemeEditorViewModel colorSchemeEditor)
        : base("Application Themes")
    {
        ThemeService = themeService;
        ThemeSettings = themeSettings;
        ColorSchemeEditor = colorSchemeEditor;
        SetCurrentThemeCommand = new RelayCommand<object>(t => ThemeService.SetCurrentTheme(t));
        RemoveThemeCommand = new RelayCommand<object>(t => ThemeService.RemoveTheme(t));
    }

    public IThemeService ThemeService { get; }
    public ThemeSettingsViewModel ThemeSettings { get; }
    public ColorSchemeEditorViewModel ColorSchemeEditor { get; }
    public IRelayCommand SetCurrentThemeCommand { get; }
    public IRelayCommand RemoveThemeCommand { get; }
}
