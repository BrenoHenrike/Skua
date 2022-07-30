using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace Skua.App.WPF.Services;
public partial class ThemeService : ObservableObject, IThemeService
{
    public ThemeService(ThemeUserSettingsService themeSettings)
    {
        _themeSettings = themeSettings;
        ITheme theme = _paletteHelper.GetTheme();

        _primaryColor = theme.PrimaryMid.Color;
        //_secondaryColor = theme.SecondaryMid.Color;
        _primaryForegroundColor = theme.PrimaryMid.GetForegroundColor();
        //_secondaryForegroundColor = theme.SecondaryMid.GetForegroundColor();

        SelectedColor = _primaryColor;
        IsDarkTheme = theme.GetBaseTheme() == BaseTheme.Dark;

        if (theme is Theme internalTheme)
        {
            _isColorAdjusted = internalTheme.ColorAdjustment is not null;

            var colorAdjustment = internalTheme.ColorAdjustment ?? new ColorAdjustment();
            _desiredContrastRatio = colorAdjustment.DesiredContrastRatio;
            _contrastValue = colorAdjustment.Contrast;
            _colorSelectionValue = colorAdjustment.Colors;
        }

        if (_paletteHelper.GetThemeManager() is { } themeManager)
        {
            themeManager.ThemeChanged += (_, e) =>
            {
                IsDarkTheme = e.NewTheme?.GetBaseTheme() == BaseTheme.Dark;

                _primaryColor = e.NewTheme?.PrimaryMid.Color;
                //_secondaryColor = e.NewTheme?.SecondaryMid.Color;
                _primaryForegroundColor = e.NewTheme?.PrimaryMid.GetForegroundColor();
                //_secondaryForegroundColor = e.NewTheme?.SecondaryMid.GetForegroundColor();
            };
        }
    }

    public void SaveTheme(string name)
    {
        _themeSettings.SaveTheme(name);
        OnPropertyChanged(nameof(UserThemes));
    }

    public void RemoveTheme(object? themeItem)
    {
        _themeSettings.RemoveTheme(themeItem);
        OnPropertyChanged(nameof(UserThemes));
    }

    public void SetCurrentTheme(object? themeItem)
    {
        _themeSettings.SetTheme(themeItem);

        var theme = _paletteHelper.GetTheme();

        _primaryColor = theme.PrimaryMid.Color;
        //_secondaryColor = theme.SecondaryMid.Color;
        _primaryForegroundColor = theme.PrimaryMid.GetForegroundColor();
        //_secondaryForegroundColor = theme.SecondaryMid.GetForegroundColor();

        ActiveScheme = ColorScheme.Primary;
        SelectedColor = theme.PrimaryMid.Color;
    }

    public List<object> Presets => _themeSettings.DefaultThemes.Cast<object>().ToList();
    public List<object> UserThemes => _themeSettings.UserThemes.Cast<object>().ToList();

    private readonly PaletteHelper _paletteHelper = new();
    private readonly ThemeUserSettingsService _themeSettings;
    private Color? _primaryColor;
    //private Color? _secondaryColor;
    private Color? _primaryForegroundColor;
    //private Color? _secondaryForegroundColor;

    [ObservableProperty]
    private ColorScheme _activeScheme;

    private object? _selectedColor;
    public object? SelectedColor
    {
        get => _selectedColor;
        set
        {
            if (_selectedColor != value)
            {
                _selectedColor = value;
                OnPropertyChanged();

                // if we are triggering a change internally its a hue change and the colors will match
                // so we don't want to trigger a custom color change.
                var currentSchemeColor = ActiveScheme switch
                {
                    ColorScheme.Primary => _primaryColor,
                    //ColorScheme.Secondary => _secondaryColor,
                    ColorScheme.PrimaryForeground => _primaryForegroundColor,
                    //ColorScheme.SecondaryForeground => _secondaryForegroundColor,
                    _ => throw new NotSupportedException($"{ActiveScheme} is not a handled ColorScheme.")
                };

                if ((Color)_selectedColor! != currentSchemeColor && value is Color color)
                    ChangeCustomColor(color);
            }
        }
    }
    public IEnumerable<object> ColorSelectionValues => Enum.GetValues(typeof(ColorSelection)).Cast<object>();

    private object _colorSelectionValue = ColorSelection.All;
    public object ColorSelectionValue
    {
        get => _colorSelectionValue;
        set
        {
            if (SetProperty(ref _colorSelectionValue, value))
            {
                ModifyTheme((theme, val) =>
                {
                    if (theme is Theme internalTheme && internalTheme.ColorAdjustment != null)
                        internalTheme.ColorAdjustment.Colors = (ColorSelection)val;
                }, value);
            }
        }
    }

    public IEnumerable<object> ContrastValues => Enum.GetValues(typeof(Contrast)).Cast<object>();

    private object _contrastValue = Contrast.Medium;
    public object ContrastValue
    {
        get => _contrastValue;
        set
        {
            if (SetProperty(ref _contrastValue, value))
            {
                ModifyTheme((theme, val) =>
                {
                    if (theme is Theme internalTheme && internalTheme.ColorAdjustment != null)
                        internalTheme.ColorAdjustment.Contrast = (Contrast)val;
                }, value);
            }
        }
    }

    private float _desiredContrastRatio = 4.5f;
    public float DesiredContrastRatio
    {
        get => _desiredContrastRatio;
        set
        {
            if (SetProperty(ref _desiredContrastRatio, value))
            {
                ModifyTheme((theme, val) =>
                {
                    if (theme is Theme internalTheme && internalTheme.ColorAdjustment != null)
                        internalTheme.ColorAdjustment.DesiredContrastRatio = val;
                }, value);
            }
        }
    }

    private bool _isColorAdjusted;
    public bool IsColorAdjusted
    {
        get => _isColorAdjusted;
        set
        {
            if (SetProperty(ref _isColorAdjusted, value))
            {
                ModifyTheme((theme, val, ts) =>
                {
                    if (theme is Theme internalTheme)
                    {
                        internalTheme.ColorAdjustment = val
                            ? new ColorAdjustment
                            {
                                DesiredContrastRatio = ts.DesiredContrastRatio,
                                Contrast = (Contrast)ts.ContrastValue,
                                Colors = (ColorSelection)ts.ColorSelectionValue
                            }
                            : null;
                    }
                }, value);
            }
        }
    }

    private bool _isDarkTheme;
    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            if (SetProperty(ref _isDarkTheme, value))
                ModifyTheme((theme, val)=> theme.SetBaseTheme(val ? Theme.Dark : Theme.Light), value);
        }
    }

    public void ApplyBaseTheme(bool isDark)
    {
        ITheme theme = _paletteHelper.GetTheme();
        IBaseTheme baseTheme = isDark ? Theme.Dark : Theme.Light;
        theme.SetBaseTheme(baseTheme);
        _paletteHelper.SetTheme(theme);
    }

    public void ChangeCustomColor(object? obj)
    {
        if (obj is not Color color)
            return;

        ITheme theme = _paletteHelper.GetTheme();

        switch (ActiveScheme)
        {
            case ColorScheme.Primary:
                theme.SetPrimaryColor(color);
                _primaryColor = color;
                break;
            //case ColorScheme.Secondary:
            //    theme.SetSecondaryColor(color);
            //    _secondaryColor = color;
            //    break;
            case ColorScheme.PrimaryForeground:
                SetPrimaryForegroundToSingleColor(theme, color);
                _primaryForegroundColor = color;
                return;
            //case ColorScheme.SecondaryForeground:
            //    SetSecondaryForegroundToSingleColor(theme, color);
            //    _secondaryForegroundColor = color;
            //    return;
        }

        _paletteHelper.SetTheme(theme);
    }

    public void ChangeScheme(ColorScheme scheme)
    {
        ActiveScheme = scheme;
        switch (ActiveScheme)
        {
            case ColorScheme.Primary:
                SelectedColor = _primaryColor;
                break;
            //case ColorScheme.Secondary:
            //    SelectedColor = _secondaryColor;
            //    break;
            case ColorScheme.PrimaryForeground:
                SelectedColor = _primaryForegroundColor;
                break;
            //case ColorScheme.SecondaryForeground:
            //    SelectedColor = _secondaryForegroundColor;
            //    break;
        }
    }

    private void SetPrimaryForegroundToSingleColor(ITheme theme, object? color)
    {
        if (color is not Color hue)
            return;

        theme.PrimaryLight = new ColorPair(theme.PrimaryLight.Color, hue);
        theme.PrimaryMid = new ColorPair(theme.PrimaryMid.Color, hue);
        theme.PrimaryDark = new ColorPair(theme.PrimaryDark.Color, hue);

        _paletteHelper.SetTheme(theme);
    }

    private void SetSecondaryForegroundToSingleColor(ITheme theme, object? color)
    {
        if (color is not Color hue)
            return;

        theme.SecondaryLight = new ColorPair(theme.SecondaryLight.Color, hue);
        theme.SecondaryMid = new ColorPair(theme.SecondaryMid.Color, hue);
        theme.SecondaryDark = new ColorPair(theme.SecondaryDark.Color, hue);

        _paletteHelper.SetTheme(theme);
    }

    private void ModifyTheme<TValue>(Action<ITheme, TValue> modificationAction, TValue value)
    {
        ITheme theme = _paletteHelper.GetTheme();

        modificationAction?.Invoke(theme, value);

        _paletteHelper.SetTheme(theme);
    }

    private void ModifyTheme<TValue>(Action<ITheme, TValue, IThemeService> modificationAction, TValue value)
    {
        ITheme theme = _paletteHelper.GetTheme();

        modificationAction?.Invoke(theme, value, this);

        _paletteHelper.SetTheme(theme);
    }
}
