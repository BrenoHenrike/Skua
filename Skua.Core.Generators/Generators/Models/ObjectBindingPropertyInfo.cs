using Skua.Core.Generators.Helpers;
using System;
using System.Collections.Generic;

namespace Skua.Core.Generators.Models;
public sealed record ObjectBindingPropertyInfo(
    string FieldName,
    string PropertyName,
    string PropertyType,
    bool IsNullable,
    bool NotifyProp,
    ObjectBindingValues Values)
{
    /// <summary>
    /// An <see cref="IEqualityComparer{T}"/> implementation for <see cref="ObjectBindingPropertyInfo"/>.
    /// </summary>
    public sealed class Comparer : Comparer<ObjectBindingPropertyInfo, Comparer>
    {
        /// <inheritdoc/>
        public override void AddToHashCode(ref HashCode hashCode, ObjectBindingPropertyInfo obj)
        {
            hashCode.Add(obj.FieldName);
            hashCode.Add(obj.PropertyName);
            hashCode.Add(obj.PropertyType);
            hashCode.Add(obj.Values.Paths[0]);
        }

        /// <inheritdoc/>
        public override bool AreEqual(ObjectBindingPropertyInfo x, ObjectBindingPropertyInfo y)
        {
            return
                x.FieldName == y.FieldName &&
                x.PropertyName == y.PropertyName &&
                x.PropertyType == y.PropertyType &&
                x.Values.Paths[0] == y.Values.Paths[0];
        }
    }
}