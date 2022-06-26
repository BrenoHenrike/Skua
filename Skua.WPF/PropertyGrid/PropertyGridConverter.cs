using System;
using System.Globalization;
using System.Windows.Data;

namespace Skua.WPF;
public class PropertyGridConverter : IValueConverter
{
    private static Type GetParameterAsType(object parameter)
    {
        if (parameter == null)
            return null;

        string typeName = string.Format("{0}", parameter);
        if (string.IsNullOrWhiteSpace(typeName))
            return null;

        return TypeResolutionService.ResolveType(typeName);
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Type parameterType = GetParameterAsType(parameter);
        if (parameterType != null)
        {
            value = ConversionService.ChangeType(value, parameterType, null, culture);
        }

        object convertedValue = targetType == null ? value : ConversionService.ChangeType(value, targetType, null, culture);
        return convertedValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        object convertedValue = targetType == null ? value : ConversionService.ChangeType(value, targetType, null, culture);
        return convertedValue;
    }
}
