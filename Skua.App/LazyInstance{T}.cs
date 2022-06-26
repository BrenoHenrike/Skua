using System;
using Microsoft.Extensions.DependencyInjection;

namespace Skua.App;
public class LazyInstance<T> : Lazy<T> where T : class
{
    public LazyInstance(IServiceProvider serviceProvider)
        : base(() => serviceProvider.GetRequiredService<T>())
    {

    }
}
