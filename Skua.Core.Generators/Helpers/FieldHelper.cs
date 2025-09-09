using Microsoft.CodeAnalysis;
using Skua.Core.Generators.Extensions;
using Skua.Core.Generators.Models;
using System;
using System.Collections.Immutable;
using System.Globalization;

namespace Skua.Core.Generators.Helpers;

internal static class FieldHelper
{
    /// <summary>
    /// Tries to gather the
    /// </summary>
    /// <param name="fieldSymbol"></param>
    /// <param name="attributeData"></param>
    /// <param name="diagnostics"></param>
    /// <param name="descriptor"></param>
    /// <param name="bindingName"></param>
    /// <returns></returns>
    public static bool TryGatherBindingName(IFieldSymbol fieldSymbol, AttributeData attributeData, ImmutableArray<Diagnostic>.Builder diagnostics, DiagnosticDescriptor descriptor, out string bindingName)
    {
        if (attributeData.AttributeClass?.HasFullyQualifiedName("Skua.Core.Flash.CallBindingAttribute") == true)
        {
            foreach (string? name in attributeData.GetConstructorArguments<string>())
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    bindingName = name!;
                    return true;
                }
                else
                {
                    diagnostics.Add(
                        descriptor,
                        fieldSymbol,
                        fieldSymbol.ContainingType,
                        fieldSymbol.Name);
                    break;
                }
            }
        }
        bindingName = string.Empty;
        return false;
    }

    /// <summary>
    /// Get the generated property name for an input field.
    /// </summary>
    /// <param name="fieldSymbol">The input <see cref="IFieldSymbol"/> instance to process.</param>
    /// <returns>The generated property name for <paramref name="fieldSymbol"/>.</returns>
    public static string GetGeneratedPropertyName(IFieldSymbol fieldSymbol)
    {
        string propertyName = fieldSymbol.Name;

        if (propertyName.StartsWith("m_"))
        {
            propertyName = propertyName.Substring(2);
        }
        else if (propertyName.StartsWith("_"))
        {
            propertyName = propertyName.TrimStart('_');
        }

        return $"{char.ToUpper(propertyName[0], CultureInfo.InvariantCulture)}{propertyName.Substring(1)}";
    }

    public static string GenerateClass(ClassInfo classInfo)
    {
        return
$@"// <This file was auto-generated>

namespace {classInfo.Namespace};

public partial class {classInfo.Name}{(string.IsNullOrWhiteSpace(classInfo.InheritanceFormattedNames) ? "" : $" : {classInfo.InheritanceFormattedNames}")}
{{";
    }
}