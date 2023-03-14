using Skua.Core.Models;

namespace Skua.Core.Interfaces;

public delegate void ThemeChangedEventHandler(object? theme);
public delegate void SchemeChangedEventHandler(ColorScheme scheme, object? color);
public interface IThemeService
{
    event ThemeChangedEventHandler ThemeChanged;
    event SchemeChangedEventHandler SchemeChanged;
    List<object> Presets { get; }
    List<object> UserThemes { get; }
    IEnumerable<object> ColorSelectionValues { get; }
    object ColorSelectionValue { get; set; }
    IEnumerable<object> ContrastValues { get; }
    object ContrastValue { get; set; }
    float DesiredContrastRatio { get; set; }
    bool IsColorAdjusted { get; set; }
    bool IsDarkTheme { get; set; }
    object? SelectedColor { get; set; }
    ColorScheme ActiveScheme { get; set; }
    void ApplyBaseTheme(bool isDark);
    void ChangeCustomColor(object? obj);
    void ChangeScheme(ColorScheme scheme);
    void SaveTheme(string name);
    void SetCurrentTheme(object? theme);
    void RemoveTheme(object? theme);
}
