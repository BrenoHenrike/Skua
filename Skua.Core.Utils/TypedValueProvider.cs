namespace Skua.Core.Utils;

public interface ITypedValueProvider
{
    object? Provide(Type type);
}

[Serializable]
public class DefaultTypedValueProvider : ITypedValueProvider
{
    public object? Provide(Type type)
    {
        try
        {
            return type == null || type == typeof(void) ? null : Activator.CreateInstance(type);
        }
        catch
        {
            return type.GetDefaultValue();
        }
    }
}

[Serializable]
public class EmptyListProvider<T> : ITypedValueProvider
{
    public object Provide(Type type)
    {
        return new List<T>();
    }
}
