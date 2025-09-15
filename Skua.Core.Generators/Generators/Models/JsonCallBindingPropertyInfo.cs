using Skua.Core.Generators.Helpers;
using System;

namespace Skua.Core.Generators.Models;

/// <summary>
/// A model describing field and values of a specific JsonCallBinding Attribute.
/// </summary>
/// <param name="FieldName">The name of the field of the current attribute.</param>
/// <param name="PropertyName">The name of the property generated for the current attribute.</param>
/// <param name="PropertyType">The type of the property with nullability annotation for the current attribute.</param>
/// <param name="IsNullable">Whether the property type is nullable.</param>
/// <param name="NotifyProp">Whether to notify property changes.</param>
/// <param name="Values">The values of the current JsonCallBinding Attribute.</param>
public sealed record JsonCallBindingPropertyInfo(
    string FieldName,
    string PropertyName,
    string PropertyType,
    bool IsNullable,
    bool NotifyProp,
    JsonCallBindingValues Values)
{
    /// <summary>
    /// An <see cref="IEqualityComparer{T}"/> implementation for <see cref="JsonCallBindingPropertyInfo"/>.
    /// </summary>
    public sealed class Comparer : Comparer<JsonCallBindingPropertyInfo, Comparer>
    {
        /// <inheritdoc/>
        public override void AddToHashCode(ref HashCode hashCode, JsonCallBindingPropertyInfo obj)
        {
            hashCode.Add(obj.FieldName);
            hashCode.Add(obj.PropertyName);
            hashCode.Add(obj.PropertyType);
            hashCode.Add(obj.Values.FunctionName);
        }

        /// <inheritdoc/>
        public override bool AreEqual(JsonCallBindingPropertyInfo x, JsonCallBindingPropertyInfo y)
        {
            return
                x.FieldName == y.FieldName &&
                x.PropertyName == y.PropertyName &&
                x.PropertyType == y.PropertyType &&
                x.Values.FunctionName == y.Values.FunctionName;
        }
    }
}