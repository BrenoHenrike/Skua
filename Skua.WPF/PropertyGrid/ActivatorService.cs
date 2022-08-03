using System;
using Skua.Core.Interfaces;

namespace Skua.WPF;
public static class ActivatorService
{
    public static object CreateInstance(Type type, params object[] args)
    {
        if (type == null)
            throw new ArgumentNullException("type");

        object obj = PropertyGridServiceProvider.Current.GetService<IActivator>().CreateInstance(type, args);
        return obj;
    }

    public static T CreateInstance<T>(params object[] args)
    {
        return (T)CreateInstance(typeof(T), args);
    }

    public static object CreateInstance(Type type)
    {
        return CreateInstance(type, null);
    }

    public static T CreateInstance<T>()
    {
        return (T)CreateInstance(typeof(T), null);
    }
}
