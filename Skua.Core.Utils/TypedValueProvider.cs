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
            object? value = type == null || type == typeof(void) ? null : Activator.CreateInstance(type);
            return value;
        }
        catch
        {
            return type.GetDefaultValue();
        }
    }
}
