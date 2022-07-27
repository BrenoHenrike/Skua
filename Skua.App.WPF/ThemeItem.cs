using MaterialDesignThemes.Wpf;
using System;
using System.Globalization;
using System.Text;
using System.Windows.Media;

namespace Skua.App.WPF;
public class ThemeItem
{
    public string Name { get; set; } = string.Empty;
    public IBaseTheme BaseTheme { get; set; } = Theme.Dark;
    public Color PrimaryColor { get; set; }
    public Color SecondaryColor { get; set; }
    public Color PrimaryForegroundColor { get; set; }
    public Color SecondaryForegroundColor { get; set; }
    public bool UseColorAdjustment { get; set; }
    public ColorAdjustment? ColorAdjustment { get; set; }

    public static ThemeItem FromString(string? themeString)
    {
        if (string.IsNullOrWhiteSpace(themeString))
            return new() { Name = "Null" };

        ThemeItem themeItem = new();
        string[] values = themeString.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (values.Length == 0)
        {
            themeItem.Name = "Error";
            return themeItem;
        }
        themeItem.Name = values[0];
        themeItem.BaseTheme = values[1] == "dark" ? Theme.Dark : Theme.Light;
        themeItem.PrimaryColor = (Color)ColorConverter.ConvertFromString(values[2]);
        themeItem.SecondaryColor = (Color)ColorConverter.ConvertFromString(values[3]);
        themeItem.PrimaryForegroundColor = (Color)ColorConverter.ConvertFromString(values[4]);
        themeItem.SecondaryForegroundColor = (Color)ColorConverter.ConvertFromString(values[5]);
        if (values.Length <= 6)
        {
            themeItem.UseColorAdjustment = false;
            return themeItem;
        }
        themeItem.UseColorAdjustment = bool.TryParse(values[6], out bool useColorAdjustment) && useColorAdjustment;
        if (!float.TryParse(values[7], NumberStyles.Any, CultureInfo.InvariantCulture, out float contrastRatio))
            contrastRatio = 1f;
        if (!Enum.TryParse(typeof(Contrast), values[8], true, out object? contrast))
            contrast = Contrast.Medium;
        if (!Enum.TryParse(typeof(ColorSelection), values[9], true, out object? colorSelection))
            colorSelection = ColorSelection.All;

        ColorAdjustment colorAdjustment = new()
        {
            DesiredContrastRatio = contrastRatio,
            Contrast = (Contrast?)contrast ?? Contrast.Medium,
            Colors = (ColorSelection?)colorSelection ?? ColorSelection.All
        };
        themeItem.ColorAdjustment = colorAdjustment;
        return themeItem;
    }

    public override string ToString()
    {
        return Name;
    }

    public string ConvertToString()
    {
        StringBuilder bob = new();
        bob.Append($"{Name},");
        bob.Append($"{(BaseTheme.GetType() == typeof(MaterialDesignDarkTheme) ? "Dark" : "Light")},");
        bob.Append($"{ToHex(PrimaryColor)},");
        bob.Append($"{ToHex(SecondaryColor)},");
        bob.Append($"{ToHex(PrimaryForegroundColor)},");
        bob.Append($"{ToHex(SecondaryForegroundColor)},");
        if(UseColorAdjustment && ColorAdjustment is not null)
        {
            bob.Append($"{UseColorAdjustment},");
            bob.Append($"{ColorAdjustment.DesiredContrastRatio},");
            bob.Append($"{ColorAdjustment.Contrast},");
            bob.Append($"{ColorAdjustment.Colors},");
        }
        return bob.ToString();
    }

    private string ToHex(Color color)
    {
        string lowerHexString(int i) => i.ToString("X2").ToLower();
        string hex = lowerHexString(color.A)
                     + lowerHexString(color.R)
                     + lowerHexString(color.G)
                     + lowerHexString(color.B);
        return $"#{hex}";
    }

    public override bool Equals(object? obj)
    {
        return obj is ThemeItem item && item.Name == Name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name);
    }
}
