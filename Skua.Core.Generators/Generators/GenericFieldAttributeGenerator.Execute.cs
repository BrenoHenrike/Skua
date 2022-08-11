using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Skua.Core.Generators.Extensions;

namespace Skua.Core.Generators;
public partial class GenericFieldAttributeGenerator<TInfo>
{
    public static class Execute
    {
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

        public static bool HasNotifyPropertyChanged(ITypeSymbol typeSymbol)
        {
            INamedTypeSymbol? baseType = typeSymbol.BaseType;

            while (baseType != null)
            {
                if (baseType.HasFullyQualifiedName("CommunityToolkit.Mvvm.ComponentModel.ObservableObject")
                    || baseType.HasFullyQualifiedName("CommunityToolkit.Mvvm.ComponentModel.ObservableRecipient")
                    || baseType.HasFullyQualifiedName("CommunityToolkit.Mvvm.ComponentModel.ObservableValidator"))
                {
                    return true;
                }

                baseType = baseType.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Tries to gather a name/path from the given attribute.
        /// </summary>
        /// <param name="fieldSymbol">The input <see cref="IFieldSymbol"/> instance to process.</param>
        /// <param name="attributeData">The <see cref="AttributeData"/> instance for <paramref name="fieldSymbol"/>.</param>
        /// <param name="diagnostics">The current collection of gathered diagnostics.</param>
        /// <param name="descriptor">The descriptor to use if no binding is found.</param>
        /// <param name="bindingName">The name/path found in the attribute constructor.</param>
        /// <returns>Whether <paramref name="attributeData"/> has a defined name/path.</returns>
        public static bool TryGatherBindingName(IFieldSymbol fieldSymbol, AttributeData attributeData, string attributeFullName, ImmutableArray<Diagnostic>.Builder diagnostics, DiagnosticDescriptor descriptor, out string bindingName)
        {
            if (attributeData.AttributeClass?.HasFullyQualifiedName(attributeFullName) == true)
            {
                foreach (string? name in attributeData.GetConstructorArguments<string>())
                {
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        bindingName = name!.Replace("\"", "\\\"");
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
        /// Tries to gather names/paths from the given attribute.
        /// </summary>
        /// <param name="fieldSymbol">The input <see cref="IFieldSymbol"/> instance to process.</param>
        /// <param name="attributeData">The <see cref="AttributeData"/> instance for <paramref name="fieldSymbol"/>.</param>
        /// <param name="diagnostics">The current collection of gathered diagnostics.</param>
        /// <param name="descriptor">The descriptor to use if no binding is found.</param>
        /// <param name="bindingNames">The names/paths found in the attribute constructor.</param>
        /// <returns>Whether <paramref name="attributeData"/> has multiple defined name/paths.</returns>
        public static bool TryGatherBindingNames(IFieldSymbol fieldSymbol, AttributeData attributeData, string attributeFullName, ImmutableArray<Diagnostic>.Builder diagnostics, DiagnosticDescriptor descriptor, out string[] bindingNames)
        {
            List<string> names = new();
            if (attributeData.AttributeClass?.HasFullyQualifiedName(attributeFullName) == true)
            {
                foreach (string? name in attributeData.GetConstructorArguments<string>())
                {
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        names.Add(name!.Replace("\"", "\\\""));
                        continue;
                    }
                    else
                    {
                        diagnostics.Add(
                            descriptor,
                            fieldSymbol,
                            fieldSymbol.ContainingType,
                            fieldSymbol.Name);
                        bindingNames = Array.Empty<string>();
                        return false;
                    }
                }
                bindingNames = names.ToArray();
                return true;
            }
            bindingNames = Array.Empty<string>();
            return false;
        }
    }
}
