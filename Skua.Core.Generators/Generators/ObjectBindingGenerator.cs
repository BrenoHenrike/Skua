using System;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Skua.Core.Generators.Extensions;
using Skua.Core.Generators.Models;
using static Skua.Core.Generators.Diagnostics.DiagnosticDescriptors;

namespace Skua.Core.Generators;

[Generator]
public partial class ObjectBindingGenerator : GenericFieldAttributeGenerator<ObjectBindingPropertyInfo>
{
    public ObjectBindingGenerator()
        : base(
            Attributes.ObjectBindingFullName,
            Attributes.ObjectBindingName,
            Attributes.ObjectBindingSource,
            ObjectBindingPropertyInfo.Comparer.Default) { }

    protected override ObjectBindingPropertyInfo? TryGetInfo(IFieldSymbol fieldSymbol, out ImmutableArray<Diagnostic> diagnostics)
    {
        ImmutableArray<Diagnostic>.Builder builder = ImmutableArray.CreateBuilder<Diagnostic>();

        // Get the property type and name
        string typeNameWithNullabilityAnnotations = fieldSymbol.Type.GetFullyQualifiedNameWithNullabilityAnnotations();
        bool isNullable = fieldSymbol.Type.NullableAnnotation == NullableAnnotation.Annotated;
        string fieldName = fieldSymbol.Name;
        string propertyName = Execute.GetGeneratedPropertyName(fieldSymbol);
        bool notifyProp = Execute.HasNotifyPropertyChanged(fieldSymbol);

        // Check for name collisions
        if (fieldName == propertyName)
        {
            builder.Add(
                ObjectBindingPropertyNameCollision,
                fieldSymbol,
                fieldSymbol.ContainingType,
                fieldSymbol.Name);

            diagnostics = builder.ToImmutable();
            return null;
        }
        string[] paths = Array.Empty<string>();
        string? select = null, requireNotNull = null, defaultValue = null;
        bool set = true, get = true, isStatic = true, hasSetter = false;
        // Gather attributes info
        foreach (AttributeData attributeData in fieldSymbol.GetAttributes())
        {
            // Gather dependent property and command names
            if (Execute.TryGatherBindingNames(fieldSymbol, attributeData, attributeFullName, builder, CallBindingPathNullorEmpty, out paths))
            {
                set = attributeData.GetNamedArgument("Set", true);
                get = attributeData.GetNamedArgument("Get", true);
                isStatic = attributeData.GetNamedArgument("IsStatic", false);
                select = attributeData.GetNamedArgument<string?>("Select", null);
                requireNotNull = attributeData.GetNamedArgument<string?>("RequireNotNull", null);
                defaultValue = attributeData.GetNamedArgument<string?>("Default", null);
                hasSetter = attributeData.GetNamedArgument("HasSetter", false);
                break;
            }
        }

        diagnostics = builder.ToImmutable();

        return new ObjectBindingPropertyInfo(
            fieldName,
            propertyName,
            typeNameWithNullabilityAnnotations,
            isNullable,
            notifyProp,
            new ObjectBindingValues(paths, get, set, select, requireNotNull, defaultValue, isStatic, hasSetter));
    }

    protected override void GenerateProperties(StringBuilder source, ObjectBindingPropertyInfo info)
    {
        string defaultValue = info.Values.Default is not null ? info.Values.Default : Default.Get(info.PropertyType);
        source.Append($"public {info.PropertyType} {info.PropertyName}{{get{{");
        if (info.Values.Get)
        {
            if (info.Values.RequireNotNull is not null)
                source.Append($"if (Flash.IsNull(\"{info.Values.RequireNotNull}\")) return {defaultValue};");
            
            source.Append("try{");
            if (info.Values.Select is not null)
                source.Append($"{info.PropertyType}{(info.IsNullable ? string.Empty : "?")} returnValue = Newtonsoft.Json.JsonConvert.DeserializeObject<{info.PropertyType}>(Flash.Call(\"selectArrayObjects\", \"{info.Values.Paths[0]}\", \"{info.Values.Select}\"));");
            else
                source.Append($"{info.PropertyType}{(info.IsNullable ? string.Empty : "?")} returnValue = Flash.{(info.Values.IsStatic ? "GetGameObjectStatic" : "GetGameObject")}<{info.PropertyType}>(\"{info.Values.Paths[0]}\");");

            source.Append($"return returnValue ?? {defaultValue};");
            source.Append($"}}catch{{return {defaultValue};}}");
        }
        else
        {
            source.Append($"return this.{info.FieldName};");
        }
        source.Append("}");
        if (info.Values.HasSetter)
        {
            source.Append("set{");
            if (info.Values.Set)
                CreateSetGameObject(source, info.Values.Paths);
            if (info.NotifyProp)
                source.Append($"SetProperty(ref this.{info.FieldName}, value);");
            else
                source.Append($"this.{info.FieldName} = value;");
            source.Append("}");
        }
        source.Append("}");
    }

    private void CreateSetGameObject(StringBuilder builder, string[] paths)
    {
        foreach (string path in paths)
        {
            builder.Append($"Flash.SetGameObject(\"{path}\", value);");
        }
    }
}
