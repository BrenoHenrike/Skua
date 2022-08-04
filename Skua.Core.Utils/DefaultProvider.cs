namespace Skua.Core.Utils;
public class DefaultProvider
{
    public static object? GetDefault<T>(Type type)
    {
        if (type == null)
            return default(T);

        if (type.IsArray)
            return Array.Empty<T>();

        if (type == typeof(string))
            return string.Empty;

        if (typeof(IEnumerable<>).IsAssignableFrom(type))
            return Enumerable.Empty<T>();

        return default(T);
    }
}
