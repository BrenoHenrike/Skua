namespace Skua.Core.Utils;

public static class ReflectUtils
{
    public static object? GetDefaultValue(this Type type)
    {
        return type != typeof(void) && type != null && type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}