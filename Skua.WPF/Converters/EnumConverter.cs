using System;
using System.Globalization;
using System.Windows.Data;

namespace Skua.WPF.Converters;
public class EnumConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Enum enumValue = default!;
        if (parameter is Type type)
        {
            enumValue = (Enum)Enum.Parse(type, value.ToString()!);
        }
        return enumValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int returnValue = 0;
        if (parameter is Type type)
        {
            returnValue = (int)Enum.Parse(type, value.ToString()!);
        }
        return returnValue;
    }
}
