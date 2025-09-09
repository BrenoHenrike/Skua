namespace Skua.Core.Interfaces;

public interface IActivator
{
    object CreateInstance(Type type, params object[] args);
}