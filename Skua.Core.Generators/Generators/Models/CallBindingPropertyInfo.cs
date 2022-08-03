using System;
using Skua.Core.Generators.Helpers;

namespace Skua.Core.Generators.Models;
/// <summary>
/// A model describing field and values of a specific Call Binding Attribute.
/// </summary>
/// <param name="FieldName">The name of the field of the current attribute.</param>
/// <param name="PropertyName">The name of the property generated for the current attribute.</param>
/// <param name="PropertyType">The type of the property with nullability annotation for the current attribute.</param>
/// <param name="Values">The values of the current Call Binding Attribute.</param>
public sealed record CallBindingPropertyInfo(
    string FieldName,
    string PropertyName,
    string PropertyType,
    bool NotifyProp,
    CallBindingValues Values)
{
    /// <summary>
    /// An <see cref="IEqualityComparer{T}"/> implementation for <see cref="CallBindingPropertyInfo"/>.
    /// </summary>
    public sealed class Comparer : Comparer<CallBindingPropertyInfo, Comparer>
    {
        /// <inheritdoc/>
        public override void AddToHashCode(ref HashCode hashCode, CallBindingPropertyInfo obj)
        {
            hashCode.Add(obj.FieldName);
            hashCode.Add(obj.PropertyName);
            hashCode.Add(obj.PropertyType);
            hashCode.Add(obj.Values.Path);
        }

        /// <inheritdoc/>
        public override bool AreEqual(CallBindingPropertyInfo x, CallBindingPropertyInfo y)
        {
            return
                x.FieldName == y.FieldName &&
                x.PropertyName == y.PropertyName &&
                x.PropertyType == y.PropertyType &&
                x.Values.Path == y.Values.Path;
        }
    }
}
