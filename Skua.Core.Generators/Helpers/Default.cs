using System;
using System.Collections.Generic;

namespace Skua.Core.Generators;

internal class Default
{
    public static readonly string DefaultString = "default";

    public static string Get(string? typeName)
    {
        if (typeName == null)
            return DefaultString;

        Type? type = Type.GetType(typeName);
        if (type == null)
            return DefaultString;

        if (type.IsArray)
            return $"Array.Empty<{type.FullName}>()";

        if (type == typeof(string))
            return string.Empty;

        if (typeof(IEnumerable<>).IsAssignableFrom(type))
            return "new()";

        return DefaultString;
    }
}