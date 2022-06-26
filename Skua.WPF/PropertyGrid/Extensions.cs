using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Markup.Primitives;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Skua.WPF;
internal static class Extensions
{
    private const string _hexaChars = "0123456789ABCDEF";

    public static string ToHexa(byte[] bytes)
    {
        if (bytes == null)
            return null;

        return ToHexa(bytes, 0, bytes.Length);
    }

    public static string ToHexa(byte[] bytes, int offset, int count)
    {
        if (bytes == null)
            return string.Empty;

        if (offset < 0)
            throw new ArgumentException(null, "offset");

        if (count < 0)
            throw new ArgumentException(null, "count");

        if (offset >= bytes.Length)
            return string.Empty;

        count = Math.Min(count, bytes.Length - offset);

        var sb = new StringBuilder(count * 2);
        for (int i = offset; i < (offset + count); i++)
        {
            sb.Append(_hexaChars[bytes[i] / 16]);
            sb.Append(_hexaChars[bytes[i] % 16]);
        }
        return sb.ToString();
    }

    public static object EnumToObject(Type enumType, object value)
    {
        if (enumType == null)
            throw new ArgumentNullException("enumType");

        if (!enumType.IsEnum)
            throw new ArgumentException(null, "enumType");

        if (value == null)
            throw new ArgumentNullException("value");

        Type underlyingType = Enum.GetUnderlyingType(enumType);
        if (underlyingType == typeof(long))
            return Enum.ToObject(enumType, ConversionService.ChangeType<long>(value));

        if (underlyingType == typeof(ulong))
            return Enum.ToObject(enumType, ConversionService.ChangeType<ulong>(value));

        if (underlyingType == typeof(int))
            return Enum.ToObject(enumType, ConversionService.ChangeType<int>(value));

        if ((underlyingType == typeof(uint)))
            return Enum.ToObject(enumType, ConversionService.ChangeType<uint>(value));

        if (underlyingType == typeof(short))
            return Enum.ToObject(enumType, ConversionService.ChangeType<short>(value));

        if (underlyingType == typeof(ushort))
            return Enum.ToObject(enumType, ConversionService.ChangeType<ushort>(value));

        if (underlyingType == typeof(byte))
            return Enum.ToObject(enumType, ConversionService.ChangeType<byte>(value));

        if (underlyingType == typeof(sbyte))
            return Enum.ToObject(enumType, ConversionService.ChangeType<sbyte>(value));

        throw new ArgumentException(null, "enumType");
    }

    public static string Format(object obj, string format, IFormatProvider formatProvider)
    {
        if (obj == null)
            return string.Empty;

        if (string.IsNullOrEmpty(format))
            return obj.ToString();

        if ((format.StartsWith("*")) ||
            (format.StartsWith("#")))
        {
            char sep1 = ' ';
            char sep2 = ':';
            if (format.Length > 1)
            {
                sep1 = format[1];
            }
            if (format.Length > 2)
            {
                sep2 = format[2];
            }

            var sb = new StringBuilder();
            foreach (PropertyInfo pi in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!pi.CanRead)
                    continue;

                if (pi.GetIndexParameters().Length > 0)
                    continue;

                object value;
                try
                {
                    value = pi.GetValue(obj, null);
                }
                catch
                {
                    continue;
                }
                if (sb.Length > 0)
                {
                    if (sep1 != ' ')
                    {
                        sb.Append(sep1);
                    }
                    sb.Append(' ');
                }

                if (format[0] == '#')
                {
                    sb.Append(DecamelizationService.Decamelize(pi.Name));
                }
                else
                {
                    sb.Append(pi.Name);
                }
                sb.Append(sep2);
                sb.Append(ConversionService.ChangeType(value, string.Format("{0}", value), formatProvider));
            }
            return sb.ToString();
        }

        if (format.StartsWith("Item[", StringComparison.CurrentCultureIgnoreCase))
        {
            string enumExpression;
            int exprPos = format.IndexOf(']', 5);
            if (exprPos < 0)
            {
                enumExpression = string.Empty;
            }
            else
            {
                enumExpression = format.Substring(5, exprPos - 5).Trim();
                // enumExpression is a lambda like expression with index as the variable
                // ex: {0: Item[index < 10]} will enum all objects with index < 10
                // errrhh... so far, since lambda cannot be parsed at runtime, we do nothing...
            }

            var enumerable = obj as IEnumerable;
            if (enumerable != null)
            {
                format = format.Substring(6 + enumExpression.Length);
                string expression;
                string separator;
                if (format.Length == 0)
                {
                    expression = null;
                    separator = ",";
                }
                else
                {
                    int pos = format.IndexOf(',');
                    if (pos <= 0)
                    {
                        separator = ",";
                        // skip '.'
                        expression = format.Substring(1);
                    }
                    else
                    {
                        separator = format.Substring(pos + 1);
                        expression = format.Substring(1, pos - 1);
                    }
                }
                return ConcatenateCollection(enumerable, expression, separator, formatProvider);
            }
        }
        else if (format.IndexOf(',') >= 0)
        {
            var sb = new StringBuilder();
            foreach (string propName in format.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                PropertyInfo pi = obj.GetType().GetProperty(propName, BindingFlags.Instance | BindingFlags.Public);
                if ((pi == null) || (!pi.CanRead))
                    continue;

                if (pi.GetIndexParameters().Length > 0)
                    continue;

                object value;
                try
                {
                    value = pi.GetValue(obj, null);
                }
                catch
                {
                    continue;
                }
                if (sb.Length > 0)
                {
                    sb.Append(' ');
                }
                sb.Append(pi.Name);
                sb.Append(':');
                sb.AppendFormat(formatProvider, "{0}", value);
            }
            return sb.ToString();
        }

        int pos2 = format.IndexOf(':');
        if (pos2 > 0)
        {
            object inner = DataBindingEvaluator.Eval(obj, format.Substring(0, pos2), false);
            if (inner == null)
                return string.Empty;

            return string.Format(formatProvider, "{0:" + format.Substring(pos2 + 1) + "}", inner);
        }
        return DataBindingEvaluator.Eval(obj, format, formatProvider, null, false);
    }

    public static string ConcatenateCollection(IEnumerable collection, string expression, string separator)
    {
        return ConcatenateCollection(collection, expression, separator, null);
    }

    public static string ConcatenateCollection(IEnumerable collection, string expression, string separator, IFormatProvider formatProvider)
    {
        if (collection == null)
            return null;

        var sb = new StringBuilder();
        int i = 0;
        foreach (object o in collection)
        {
            if (i > 0)
            {
                sb.Append(separator);
            }
            else
            {
                i++;
            }

            if (o != null)
            {
                //object e = ConvertUtilities.Evaluate(o, expression, typeof(string), null, formatProvider);
                object e = DataBindingEvaluator.Eval(o, expression, formatProvider, null, false);
                if (e != null)
                {
                    sb.Append(e);
                }
            }
        }
        return sb.ToString();
    }

    public static Type GetElementType(Type collectionType)
    {
        if (collectionType == null)
            throw new ArgumentNullException("collectionType");

        foreach (Type iface in collectionType.GetInterfaces())
        {
            if (!iface.IsGenericType)
                continue;

            if (iface.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                return iface.GetGenericArguments()[1];

            if (iface.GetGenericTypeDefinition() == typeof(IList<>))
                return iface.GetGenericArguments()[0];

            if (iface.GetGenericTypeDefinition() == typeof(ICollection<>))
                return iface.GetGenericArguments()[0];

            if (iface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return iface.GetGenericArguments()[0];
        }
        return typeof(object);
    }

    public static int GetEnumMaxPower(Type enumType)
    {
        if (enumType == null)
            throw new ArgumentNullException("enumType");

        if (!enumType.IsEnum)
            throw new ArgumentException(null, "enumType");

        Type utype = Enum.GetUnderlyingType(enumType);
        return GetEnumUnderlyingTypeMaxPower(utype);
    }

    public static int GetEnumUnderlyingTypeMaxPower(Type underlyingType)
    {
        if (underlyingType == null)
            throw new ArgumentNullException("underlyingType");

        if (underlyingType == typeof(long) || underlyingType == typeof(ulong))
            return 64;

        if (underlyingType == typeof(int) || underlyingType == typeof(uint))
            return 32;

        if (underlyingType == typeof(short) || underlyingType == typeof(ushort))
            return 16;

        if (underlyingType == typeof(byte) || underlyingType == typeof(sbyte))
            return 8;

        throw new ArgumentException(null, "underlyingType");
    }

    public static ulong EnumToUInt64(object value)
    {
        if (value == null)
            throw new ArgumentNullException("value");

        TypeCode typeCode = Convert.GetTypeCode(value);
        switch (typeCode)
        {
            case TypeCode.SByte:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
                return (ulong)Convert.ToInt64(value, CultureInfo.InvariantCulture);

            case TypeCode.Byte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
                return Convert.ToUInt64(value, CultureInfo.InvariantCulture);

            //case TypeCode.String:
            default:
                return ConversionService.ChangeType<ulong>(value);
        }
    }

    public static bool IsFlagsEnum(Type type)
    {
        if (type == null)
            throw new ArgumentNullException("type");

        if (!type.IsEnum)
            return false;

        return type.IsDefined(typeof(FlagsAttribute), true);
    }

    public static List<T> SplitToList<T>(this string thisString, params char[] separators)
    {
        var list = new List<T>();
        if (thisString != null)
        {
            foreach (string s in thisString.Split(separators))
            {
                T item = ConversionService.ChangeType<T>(s);
                list.Add(item);
            }
        }
        return list;
    }

    public static string Nullify(this string thisString)
    {
        return Nullify(thisString, true);
    }

    public static string Nullify(this string thisString, bool trim)
    {
        if (string.IsNullOrWhiteSpace(thisString))
            return null;

        return trim ? thisString.Trim() : thisString;
    }

    public static bool EqualsIgnoreCase(this string thisString, string text)
    {
        return EqualsIgnoreCase(thisString, text, false);
    }

    public static bool EqualsIgnoreCase(this string thisString, string text, bool trim)
    {
        if (trim)
        {
            thisString = Nullify(thisString, true);
            text = Nullify(text, true);
        }

        if (thisString == null)
            return text == null;

        if (text == null)
            return false;

        if (thisString.Length != text.Length)
            return false;

        return string.Compare(thisString, text, StringComparison.OrdinalIgnoreCase) == 0;
    }

    public static IEnumerable<DependencyObject> EnumerateVisualChildren(this DependencyObject obj)
    {
        return obj.EnumerateVisualChildren(true);
    }

    public static IEnumerable<DependencyObject> EnumerateVisualChildren(this DependencyObject obj, bool recursive)
    {
        return obj.EnumerateVisualChildren(recursive, true);
    }

    public static IEnumerable<DependencyObject> EnumerateVisualChildren(this DependencyObject obj, bool recursive, bool sameLevelFirst)
    {
        if (obj == null)
            yield break;

        if (sameLevelFirst)
        {
            int count = VisualTreeHelper.GetChildrenCount(obj);
            var list = new List<DependencyObject>(count);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child == null)
                    continue;

                yield return child;
                if (recursive)
                {
                    list.Add(child);
                }
            }

            foreach (var child in list)
            {
                foreach (DependencyObject grandChild in child.EnumerateVisualChildren(recursive, true))
                {
                    yield return grandChild;
                }
            }
        }
        else
        {
            int count = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child == null)
                    continue;

                yield return child;
                if (recursive)
                {
                    foreach (var dp in child.EnumerateVisualChildren(true, false))
                    {
                        yield return dp;
                    }
                }
            }
        }
    }

    public static T FindVisualChild<T>(this DependencyObject obj, Func<T, bool> where) where T : FrameworkElement
    {
        if (where == null)
            throw new ArgumentNullException("where");

        foreach (T item in obj.EnumerateVisualChildren(true, true).OfType<T>())
        {
            if (where(item))
                return item;
        }
        return null;
    }

    public static T FindVisualChild<T>(this DependencyObject obj, string name) where T : FrameworkElement
    {
        foreach (T item in obj.EnumerateVisualChildren(true, true).OfType<T>())
        {
            if (name == null)
                return item;

            if (item.Name == name)
                return item;
        }
        return null;
    }

    public static IEnumerable<DependencyProperty> EnumerateMarkupDependencyProperties(object element)
    {
        if (element != null)
        {
            MarkupObject markupObject = MarkupWriter.GetMarkupObjectFor(element);
            if (markupObject != null)
            {
                foreach (MarkupProperty mp in markupObject.Properties)
                {
                    if (mp.DependencyProperty != null)
                        yield return mp.DependencyProperty;
                }
            }
        }
    }

    public static IEnumerable<DependencyProperty> EnumerateMarkupAttachedProperties(object element)
    {
        if (element != null)
        {
            MarkupObject markupObject = MarkupWriter.GetMarkupObjectFor(element);
            if (markupObject != null)
            {
                foreach (MarkupProperty mp in markupObject.Properties)
                {
                    if (mp.IsAttached)
                        yield return mp.DependencyProperty;
                }
            }
        }
    }

    public static T GetVisualSelfOrParent<T>(this DependencyObject source) where T : DependencyObject
    {
        if (source == null)
            return default(T);

        if (source is T)
            return (T)source;

        if (!(source is Visual) && !(source is Visual3D))
            return default(T);

        return VisualTreeHelper.GetParent(source).GetVisualSelfOrParent<T>();
    }
    public static T FindFocusableVisualChild<T>(this DependencyObject obj, string name) where T : FrameworkElement
    {
        foreach (T item in obj.EnumerateVisualChildren(true, true).OfType<T>())
        {
            if (item.Focusable && (item.Name == name || name == null))
                return item;
        }
        return null;
    }

    public static IEnumerable<T> GetChildren<T>(this DependencyObject obj)
    {
        if (obj == null)
            yield break;

        foreach (object item in LogicalTreeHelper.GetChildren(obj))
        {
            if (item == null)
                continue;

            if (item is T)
                yield return (T)item;

            var dep = item as DependencyObject;
            if (dep != null)
            {
                foreach (T child in dep.GetChildren<T>())
                {
                    yield return child;
                }
            }
        }
    }

    public static T GetSelfOrParent<T>(this FrameworkElement source) where T : FrameworkElement
    {
        while (true)
        {
            if (source == null)
                return default(T);

            if (source is T)
                return (T)source;

            source = source.Parent as FrameworkElement;
        }
    }

    public static string GetAllMessages(this Exception exception)
    {
        return GetAllMessages(exception, Environment.NewLine);
    }

    public static string GetAllMessages(this Exception exception, string separator)
    {
        if (exception == null)
            return null;

        var sb = new StringBuilder();
        AppendMessages(sb, exception, separator);
        return sb.ToString().Replace("..", ".");
    }

    private static void AppendMessages(StringBuilder sb, Exception e, string separator)
    {
        if (e == null)
            return;

        // this one is not interesting...
        if (!(e is TargetInvocationException))
        {
            if (sb.Length > 0)
            {
                sb.Append(separator);
            }
            sb.Append(e.Message);
        }
        AppendMessages(sb, e.InnerException, separator);
    }

    public static T GetAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute
    {
        if (provider == null)
            return null;

        object[] o = provider.GetCustomAttributes(typeof(T), true);
        if (o == null || o.Length == 0)
            return null;

        return (T)o[0];
    }

    public static T GetAttribute<T>(this MemberDescriptor descriptor) where T : Attribute
    {
        if (descriptor == null)
            return null;

        return GetAttribute<T>(descriptor.Attributes);
    }

    public static T GetAttribute<T>(this AttributeCollection attributes) where T : Attribute
    {
        if (attributes == null)
            return null;

        foreach (var att in attributes)
        {
            if (typeof(T).IsAssignableFrom(att.GetType()))
                return (T)att;
        }
        return null;
    }

    public static IEnumerable<T> GetAttributes<T>(this MemberInfo element) where T : Attribute
    {
        return (IEnumerable<T>)Attribute.GetCustomAttributes(element, typeof(T));
    }

    public static bool IsNullable(this Type type)
    {
        if (type == null)
            return false;

        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }
}
