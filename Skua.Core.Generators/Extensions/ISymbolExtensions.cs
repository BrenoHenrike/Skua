using Microsoft.CodeAnalysis;
using Skua.Core.Generators.Models;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Skua.Core.Generators.Extensions;

internal static class ISymbolExtensions
{
    /// <summary>
    /// Creates a <see cref="ClassInfo"/> from the symbol instance.
    /// </summary>
    /// <param name="symbol">The input <see cref="IFieldSymbol"/> instance.</param>
    /// <returns>A <see cref="ClassInfo"/> reference with the input field symbol information.</returns>
    public static ClassInfo GetClassInfo(this ISymbol symbol)
    {
        INamedTypeSymbol classSymbol = symbol.ContainingType;
        string? baseClass = classSymbol.BaseType?.ToDisplayString();
        if (baseClass is not null and "object")
            baseClass = null;
        List<string> interfaces = classSymbol.Interfaces.Select(i => i.ToDisplayString()).ToList();
        string inheritances = $"{baseClass ?? ""}{(baseClass is not null && interfaces.Count > 0 ? ", " : "")}{(interfaces.Count > 0 ? string.Join(", ", interfaces) : "")}";

        return new ClassInfo(
            classSymbol.Name,
            classSymbol.ContainingNamespace.ToDisplayString(),
            inheritances);
    }

    /// <summary>
    /// Checks whether or not a given type symbol has a specified full name.
    /// </summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance to check.</param>
    /// <param name="name">The full name to check.</param>
    /// <returns>Whether <paramref name="symbol"/> has a full name equals to <paramref name="name"/>.</returns>
    public static bool HasFullyQualifiedName(this ISymbol symbol, string name)
    {
        return symbol.ToDisplayString() == name;
    }

    /// <summary>
    /// Checks whether or not a given symbol has an attribute with the specified full name.
    /// </summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance to check.</param>
    /// <param name="name">The attribute name to look for.</param>
    /// <returns>Whether or not <paramref name="symbol"/> has an attribute with the specified name.</returns>
    public static bool HasAttributeWithFullyQualifiedName(this ISymbol symbol, string name)
    {
        ImmutableArray<AttributeData> attributes = symbol.GetAttributes();

        foreach (AttributeData attribute in attributes)
        {
            if (attribute.AttributeClass?.HasFullyQualifiedName(name) == true)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Gets the fully qualified name for a given symbol, including nullability annotations
    /// </summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance.</param>
    /// <returns>The fully qualified name for <paramref name="symbol"/>.</returns>
    public static string GetFullyQualifiedNameWithNullabilityAnnotations(this ISymbol symbol)
    {
        return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier));
    }
}