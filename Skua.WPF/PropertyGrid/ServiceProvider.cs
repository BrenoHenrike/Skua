using Skua.Core.Interfaces;
using Skua.Core.Utils;
using System;
using System.Collections.Concurrent;

namespace Skua.WPF;

public class PropertyGridServiceProvider : IServiceProvider
{
    private readonly ConcurrentDictionary<Type, object> _services = new ConcurrentDictionary<Type, object>();
    private static readonly PropertyGridServiceProvider _current = new PropertyGridServiceProvider();

    public PropertyGridServiceProvider()
    {
        ResetDefaultServices();
    }

    protected virtual void ResetDefaultServices()
    {
        if (!_services.ContainsKey(typeof(IConverter)))
        {
            _services[typeof(IConverter)] = new BaseConverter();
        }

        if (!_services.ContainsKey(typeof(IDecamelizer)))
        {
            _services[typeof(IDecamelizer)] = new Decamelizer();
        }

        if (!_services.ContainsKey(typeof(ITypeResolver)))
        {
            _services[typeof(ITypeResolver)] = new BaseTypeResolver();
        }

        if (!_services.ContainsKey(typeof(IActivator)))
        {
            _services[typeof(IActivator)] = new BaseActivator();
        }
    }

    public static PropertyGridServiceProvider Current
    {
        get
        {
            return _current;
        }
    }

    public T GetService<T>()
    {
        return (T)GetService(typeof(T));
    }

    public virtual object SetService(Type serviceType, object service)
    {
        if (serviceType == null)
            throw new ArgumentNullException("serviceType");

        // service can be null, it will reset to the default one

        object previous;
        _services.TryGetValue(serviceType, out previous);
        _services[serviceType] = service;
        ResetDefaultServices();
        return previous;
    }

    public virtual object GetService(Type serviceType)
    {
        if (serviceType == null)
            throw new ArgumentNullException("serviceType");

        object value;
        _services.TryGetValue(serviceType, out value);
        return value;
    }
}