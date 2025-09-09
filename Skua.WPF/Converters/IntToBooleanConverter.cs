using System;
using System.Globalization;
using System.Windows.Data;

namespace Skua.WPF.Converters;

public class IntToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int intValue = (int)value;
        return intValue != 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? 1 : 0;
    }
}