using System;

namespace Skua.Core.Interfaces;
public interface ITypeResolver
{
    Type ResolveType(string fullName, bool throwOnError);
}