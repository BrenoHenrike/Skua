using System;
using System.Globalization;
using System.Windows.Data;

namespace Skua.WPF.Converters;
public class StringToIntConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        _ = int.TryParse(value.ToString(), out int result);
        return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value.ToString() ?? string.Empty;
    }
}
