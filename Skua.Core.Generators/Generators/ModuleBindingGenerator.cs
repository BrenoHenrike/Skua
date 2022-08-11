using System;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Skua.Core.Generators.Extensions;
using Skua.Core.Generators.Models;
using static Skua.Core.Generators.Diagnostics.DiagnosticDescriptors;
using static Skua.Core.Generators.Attributes;

namespace Skua.Core.Generators;

[Generator]
public class ModuleBindingGenerator : GenericFieldAttributeGenerator<ModuleBindingPropertyInfo>
{
    public ModuleBindingGenerator()
        : base(
            ModuleBindingFullName,
            ModuleBindingName,
            ModuleBindingSource,
            ModuleBindingPropertyInfo.Comparer.Default) { }

    protected override void GenerateProperties(StringBuilder source, ModuleBindingPropertyInfo info)
    {
        source.Append($"public {info.PropertyType} {info.PropertyName}{{get{{return this.{info.FieldName};}}");
        source.Append($"set{{Flash.Call($\"mod{{((bool)value ? \"Enable\" : \"Disable\")}}\", \"{info.ModuleName}\");");
        if (info.NotifyProp)
            source.Append($"SetProperty(ref this.{info.FieldName}, value);}}}}");
        else
            source.Append($"this.{info.FieldName} = value;}}}}");
    }

    protected override ModuleBindingPropertyInfo? TryGetInfo(IFieldSymbol fieldSymbol, out ImmutableArray<Diagnostic> diagnostics)
    {
        ImmutableArray<Diagnostic>.Builder builder = ImmutableArray.CreateBuilder<Diagnostic>();

        // Get the property type and name
        string typeNameWithNullabilityAnnotations = fieldSymbol.Type.GetFullyQualifiedNameWithNullabilityAnnotations();
        string fieldName = fieldSymbol.Name;
        string propertyName = Execute.GetGeneratedPropertyName(fieldSymbol);
        bool notifyProp = Execute.HasNotifyPropertyChanged(fieldSymbol.ContainingType);

        // Check for name collisions
        if (fieldName == propertyName)
        {
            builder.Add(
                ModuleBindingPropertyNameCollision,
                fieldSymbol,
                fieldSymbol.ContainingType,
                fieldSymbol.Name);

            diagnostics = builder.ToImmutable();
            return null;
        }
        string name = string.Empty;
        // Gather attributes info
        foreach (AttributeData attributeData in fieldSymbol.GetAttributes())
        {
            // Gather dependent property and command names
            if (Execute.TryGatherBindingName(fieldSymbol, attributeData, attributeFullName, builder, ModuleBindingPathNullorEmpty, out name))
            {
                break;
            }
        }

        diagnostics = builder.ToImmutable();

        return new ModuleBindingPropertyInfo(
            fieldName,
            propertyName,
            typeNameWithNullabilityAnnotations,
            name,
            notifyProp);
    }
}
