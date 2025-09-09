﻿using Skua.Core.Interfaces;
using System;

namespace Skua.WPF;

public class BaseTypeResolver : ITypeResolver
{
    public virtual Type ResolveType(string fullName, bool throwOnError)
    {
        if (fullName == null)
            throw new ArgumentNullException(nameof(fullName));

        Type type = Type.GetType(fullName, throwOnError)!;
        return type;
    }
}