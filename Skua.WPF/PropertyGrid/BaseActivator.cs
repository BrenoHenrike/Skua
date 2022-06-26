using System;
using Skua.Core.Interfaces;

namespace Skua.WPF;
public class BaseActivator : IActivator
{
    public virtual object CreateInstance(Type type, params object[] args)
    {
        if (type == null)
            throw new ArgumentNullException("type");

        if (type == typeof(DynamicObject))
            return new DynamicObject();

        if (type == typeof(PropertyGridProperty))
            return new PropertyGridProperty((PropertyGridDataProvider)args[0]);

        return Activator.CreateInstance(type, args);
    }
}
