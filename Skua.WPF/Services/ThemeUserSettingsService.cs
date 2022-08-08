using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using CommunityToolkit.Mvvm.ComponentModel;
using Skua.Core.Interfaces;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Skua.WPF.Services;
public class ThemeUserSettingsService : ObservableObject
{
    public ThemeUserSettingsService(ISettingsService settings)
    {
        _settings = settings;
        _paletteHelper = new();
        DefaultThemes = GetThemes(_settings.Get<StringCollection>("DefaultThemes")).ToArray();
        UserThemes = GetThemes(_settings.Get<StringCollection>("UserThemes"));
        ThemeItem current = ThemeItem.FromString(_settings.Get<string>("CurrentTheme"));
        CurrentTheme = current.Name is "Null" or "Error" ? DefaultThemes[0] : current;
        SetTheme(CurrentTheme);
    }

    private readonly PaletteHelper _paletteHelper;
    private readonly ISettingsService _settings;

    public ThemeItem[] DefaultThemes { get; }
    public List<ThemeItem> UserThemes { get; set; }
    public ThemeItem CurrentTheme { get; set; }

    public List<ThemeItem> GetThemes(StringCollection? themeStrings)
    {
        if (themeStrings is null)
            return new();
        List<ThemeItem> themes = new();
        foreach(string? themeString in themeStrings)
            themes.Add(ThemeItem.FromString(themeString));

        return themes;
    }

    public void SetTheme(object? theme)
    {
        if (theme is not ThemeItem themeItem)
            return;

        SetTheme(themeItem);
        SaveCurrentTheme();
    }

    public void SetTheme(ThemeItem themeItem)
    {
        ITheme theme = _paletteHelper.GetTheme();

        theme.SetPrimaryColor(themeItem.PrimaryColor);
        theme.SetSecondaryColor(themeItem.SecondaryColor);

        theme.PrimaryLight = new ColorPair(theme.PrimaryLight.Color, themeItem.PrimaryForegroundColor);
        theme.PrimaryMid = new ColorPair(theme.PrimaryMid.Color, themeItem.PrimaryForegroundColor);
        theme.PrimaryDark = new ColorPair(theme.PrimaryDark.Color, themeItem.PrimaryForegroundColor);

        theme.SecondaryLight = new ColorPair(theme.SecondaryLight.Color, themeItem.SecondaryForegroundColor);
        theme.SecondaryMid = new ColorPair(theme.SecondaryMid.Color, themeItem.SecondaryForegroundColor);
        theme.SecondaryDark = new ColorPair(theme.SecondaryDark.Color, themeItem.SecondaryForegroundColor);

        if(theme is Theme internalTheme)
            internalTheme.ColorAdjustment = themeItem.UseColorAdjustment ? themeItem.ColorAdjustment : null;

        theme.SetBaseTheme(themeItem.BaseTheme);
        _paletteHelper.SetTheme(theme);
        CurrentTheme = themeItem;
    }

    public void SaveTheme(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;
        if (_paletteHelper.GetTheme() is not Theme theme)
            return;

        ThemeItem themeItem = new()
        {
            Name = name,
            BaseTheme = theme.GetBaseTheme().GetBaseTheme(),
            PrimaryColor = theme.PrimaryMid.Color,
            SecondaryColor = theme.SecondaryMid.Color,
            PrimaryForegroundColor = theme.PrimaryMid.GetForegroundColor(),
            SecondaryForegroundColor = theme.SecondaryMid.GetForegroundColor(),
            UseColorAdjustment = theme.ColorAdjustment is not null,
            ColorAdjustment = theme.ColorAdjustment
        };

        SaveTheme(themeItem);
    }

    public void SaveTheme(ThemeItem themeItem)
    {
        CurrentTheme = themeItem;
        SaveCurrentTheme();

        if (!UserThemes.Contains(themeItem))
            UserThemes.Add(themeItem);
        else
        {
            int index = UserThemes.IndexOf(themeItem);
            UserThemes[index] = themeItem;
        }

        SaveUserTheme();
        OnPropertyChanged(nameof(UserThemes));
        OnPropertyChanged(nameof(CurrentTheme));
    }

    public void RemoveTheme(object? theme)
    {
        if (theme is not ThemeItem themeItem)
            return;

        RemoveTheme(themeItem);
    }

    public void RemoveTheme(ThemeItem themeItem)
    {
        if (CurrentTheme == themeItem)
        {
            CurrentTheme = DefaultThemes[0];
            SetTheme(CurrentTheme);
            SaveCurrentTheme();
            OnPropertyChanged(nameof(CurrentTheme));
        }
        UserThemes.Remove(themeItem);
        SaveUserTheme();
        OnPropertyChanged(nameof(UserThemes));
    }

    private void SaveUserTheme()
    {
        StringCollection userThemes = new();
        userThemes.AddRange(UserThemes.Select(t => t.ConvertToString()).ToArray());
        _settings.Set("UserThemes", userThemes);
    }

    private void SaveCurrentTheme()
    {
        _settings.Set("CurrentTheme", CurrentTheme.ConvertToString());
    }
}
