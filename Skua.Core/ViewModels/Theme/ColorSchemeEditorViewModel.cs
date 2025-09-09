using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Models;

namespace Skua.Core.ViewModels;

public class ColorSchemeEditorViewModel : BotControlViewModelBase
{
    public ColorSchemeEditorViewModel(IThemeService themeService, ThemeSettingsViewModel themeSettings)
        : base("Color Scheme Editor")
    {
        ThemeService = themeService;
        ThemeSettings = themeSettings;

        ChangeCustomHueCommand = new RelayCommand<object>(ThemeService.ChangeCustomColor);
        ChangeToPrimaryCommand = new RelayCommand(() => ThemeService.ChangeScheme(ColorScheme.Primary));
        ChangeToSecondaryCommand = new RelayCommand(() => ThemeService.ChangeScheme(ColorScheme.Secondary));
        ChangeToPrimaryForegroundCommand = new RelayCommand(() => ThemeService.ChangeScheme(ColorScheme.PrimaryForeground));
        ChangeToSecondaryForegroundCommand = new RelayCommand(() => ThemeService.ChangeScheme(ColorScheme.SecondaryForeground));
    }

    public IThemeService ThemeService { get; }
    public ThemeSettingsViewModel ThemeSettings { get; }
    public IRelayCommand ChangeCustomHueCommand { get; }
    public IRelayCommand ChangeToPrimaryCommand { get; }
    public IRelayCommand ChangeToSecondaryCommand { get; }
    public IRelayCommand ChangeToPrimaryForegroundCommand { get; }
    public IRelayCommand ChangeToSecondaryForegroundCommand { get; }
}