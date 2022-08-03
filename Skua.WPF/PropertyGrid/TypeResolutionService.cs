using System;
using Skua.Core.Interfaces;

namespace Skua.WPF;
public static class TypeResolutionService
{
    public static Type ResolveType(string fullName)
    {
        return ResolveType(fullName, false);
    }

    public static Type ResolveType(string fullName, bool throwOnError)
    {
        return PropertyGridServiceProvider.Current.GetService<ITypeResolver>().ResolveType(fullName, throwOnError);
    }
}
