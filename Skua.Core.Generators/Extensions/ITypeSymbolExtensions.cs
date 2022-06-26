using Microsoft.CodeAnalysis;

namespace Skua.Core.Generators.Extensions;
internal static class ITypeSymbolExtensions
{
    /// <summary>
    /// Checks whether or not a given <see cref="ITypeSymbol"/> has or inherits a specified attribute.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="name">The name of the attribute to look for.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> has an attribute with the specified type name.</returns>
    public static bool HasOrInheritsAttributeWithFullyQualifiedName(this ITypeSymbol typeSymbol, string name)
    {
        for (ITypeSymbol? currentType = typeSymbol; currentType is not null; currentType = currentType.BaseType)
        {
            if (currentType.HasAttributeWithFullyQualifiedName(name))
            {
                return true;
            }
        }

        return false;
    }
}
