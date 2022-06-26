using System;

namespace Skua.WPF;
public static class ConversionService
{
    public static bool TryChangeType<T>(object input, IFormatProvider provider, out T value)
    {
        object v;
        bool b = ServiceProvider.Current.GetService<IConverter>().TryChangeType(input, typeof(T), provider, out v);
        if (!b)
        {
            if (v == null)
            {
                if (typeof(T).IsValueType)
                {
                    value = (T)Activator.CreateInstance(typeof(T));
                }
                else
                {
                    value = default(T);
                }
            }
            else
            {
                value = (T)v;
            }
            return false;
        }
        value = (T)v;
        return b;
    }

    public static bool TryChangeType<T>(object input, out T value)
    {
        return TryChangeType(input, null, out value);
    }

    public static bool TryChangeType(object input, Type conversionType, out object value)
    {
        return TryChangeType(input, conversionType, null, out value);
    }

    public static bool TryChangeType(object input, Type conversionType, IFormatProvider provider, out object value)
    {
        return ServiceProvider.Current.GetService<IConverter>().TryChangeType(input, conversionType, provider, out value);
    }

    public static object ChangeType(object input, Type conversionType)
    {
        return ChangeType(input, conversionType, null, null);
    }

    public static object ChangeType(object input, Type conversionType, object defaultValue)
    {
        return ChangeType(input, conversionType, defaultValue, null);
    }

    public static object ChangeType(object input, Type conversionType, object defaultValue, IFormatProvider provider)
    {
        if (conversionType == null)
            throw new ArgumentNullException("conversionType");

        if (defaultValue == null && conversionType.IsValueType)
        {
            defaultValue = Activator.CreateInstance(conversionType);
        }

        object value;
        if (TryChangeType(input, conversionType, provider, out value))
            return value;

        return defaultValue;
    }

    public static T ChangeType<T>(object input)
    {
        return ChangeType(input, default(T));
    }

    public static T ChangeType<T>(object input, T defaultValue)
    {
        return ChangeType(input, defaultValue, null);
    }

    public static T ChangeType<T>(object input, T defaultValue, IFormatProvider provider)
    {
        T value;
        if (TryChangeType(input, provider, out value))
            return value;

        return defaultValue;
    }
}
