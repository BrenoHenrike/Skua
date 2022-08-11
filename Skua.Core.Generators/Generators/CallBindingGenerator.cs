using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Skua.Core.Generators.Extensions;
using Skua.Core.Generators.Models;
using static Skua.Core.Generators.Diagnostics.DiagnosticDescriptors;
using static Skua.Core.Generators.Attributes;

namespace Skua.Core.Generators;

[Generator]
public class CallBindingGenerator : GenericFieldAttributeGenerator<CallBindingPropertyInfo>
{
    public CallBindingGenerator()
        : base(
            CallBindingFullName,
            CallBindingName,
            CallBindingSource,
            CallBindingPropertyInfo.Comparer.Default) { }
    protected override void GenerateProperties(StringBuilder source, CallBindingPropertyInfo info)
    {
        string defaultValue = info.Values.Default is not null ? info.Values.Default : Default.Get(info.PropertyType);
        source.Append($"public {info.PropertyType} {info.PropertyName}{{get{{");
        if(info.Values.Get)
        {
            source.Append("try{");
            source.Append($"{info.PropertyType}{(info.IsNullable ? string.Empty : "?")} returnValue = Flash.Call<{info.PropertyType}>(\"{info.Values.Path}\");");
            source.Append($"return returnValue ?? {defaultValue};");
            source.Append($"}}catch{{return {defaultValue};}}");
        }
        else
        {
            source.Append($"return this.{info.FieldName};");
        }
        source.Append("}");
        if(info.Values.HasSetter)
        {
            source.Append("set{");
            if(info.Values.Set)
            {
                if (info.Values.UseValue)
                    source.Append($"Flash.Call(\"{info.Values.Path}\", value);");
                else
                    source.Append($"Flash.Call(\"{info.Values.Path}\");");
                if (info.NotifyProp)
                    source.Append($"SetProperty(ref this.{info.FieldName}, value);");
                else
                    source.Append($"this.{info.FieldName} = value;");
            }
            else
            {
                if (info.NotifyProp)
                    source.Append($"SetProperty(ref this.{info.FieldName}, value);");
                else
                    source.Append($"this.{info.FieldName} = value;");
            }
            source.Append("}");
        }
        source.Append("}");
    }

    protected override CallBindingPropertyInfo? TryGetInfo(IFieldSymbol fieldSymbol, out ImmutableArray<Diagnostic> diagnostics)
    {
        ImmutableArray<Diagnostic>.Builder builder = ImmutableArray.CreateBuilder<Diagnostic>();

        // Get the property type and name
        string typeNameWithNullabilityAnnotations = fieldSymbol.Type.GetFullyQualifiedNameWithNullabilityAnnotations();
        bool isNullable = fieldSymbol.Type.NullableAnnotation == NullableAnnotation.Annotated;
        string fieldName = fieldSymbol.Name;
        string propertyName = Execute.GetGeneratedPropertyName(fieldSymbol);
        bool notifyProp = Execute.HasNotifyPropertyChanged(fieldSymbol.ContainingType);

        // Check for name collisions
        if (fieldName == propertyName)
        {
            builder.Add(
                CallBindingPropertyNameCollision,
                fieldSymbol,
                fieldSymbol.ContainingType,
                fieldSymbol.Name);

            diagnostics = builder.ToImmutable();
            return null;
        }
        bool set = true, get = true, useValue = true, hasSetter = false;
        string path = string.Empty;
        string? defaultValue = null;
        // Gather attributes info
        foreach (AttributeData attributeData in fieldSymbol.GetAttributes())
        {
            // Gather dependent property and command names
            if (Execute.TryGatherBindingName(fieldSymbol, attributeData, attributeFullName, builder, CallBindingPathNullorEmpty, out path))
            {
                set = attributeData.GetNamedArgument("Set", true);
                get = attributeData.GetNamedArgument("Get", true);
                useValue = attributeData.GetNamedArgument("UseValue", true);
                hasSetter = attributeData.GetNamedArgument("HasSetter", false);
                defaultValue = attributeData.GetNamedArgument<string?>("Default", null);
                break;
            }
        }

        diagnostics = builder.ToImmutable();

        return new CallBindingPropertyInfo(
            fieldName,
            propertyName,
            typeNameWithNullabilityAnnotations,
            isNullable,
            notifyProp,
            new CallBindingValues(path, defaultValue, useValue, get, set, hasSetter));
    }
}
