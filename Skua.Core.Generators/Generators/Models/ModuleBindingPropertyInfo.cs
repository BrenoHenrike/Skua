using System;
using System.Collections.Generic;
using Skua.Core.Generators.Helpers;

namespace Skua.Core.Generators.Models;
public sealed record ModuleBindingPropertyInfo(
    string FieldName,
    string PropertyName,
    string PropertyType,
    string ModuleName,
    bool NotifyProp)
{
    /// <summary>
    /// An <see cref="IEqualityComparer{T}"/> implementation for <see cref="ModuleBindingPropertyInfo"/>.
    /// </summary>
    public sealed class Comparer : Comparer<ModuleBindingPropertyInfo, Comparer>
    {
        /// <inheritdoc/>
        public override void AddToHashCode(ref HashCode hashCode, ModuleBindingPropertyInfo obj)
        {
            hashCode.Add(obj.FieldName);
            hashCode.Add(obj.PropertyName);
            hashCode.Add(obj.PropertyType);
            hashCode.Add(obj.ModuleName);
        }

        /// <inheritdoc/>
        public override bool AreEqual(ModuleBindingPropertyInfo x, ModuleBindingPropertyInfo y)
        {
            return
                x.FieldName == y.FieldName &&
                x.PropertyName == y.PropertyName &&
                x.PropertyType == y.PropertyType &&
                x.ModuleName == y.ModuleName;
        }
    }
}
