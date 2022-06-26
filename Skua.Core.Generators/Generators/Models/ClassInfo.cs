using System;
using System.Collections.Generic;
using Skua.Core.Generators.Helpers;

namespace Skua.Core.Generators.Models;
/// <summary>
/// A model describing class information for a specific type.
/// </summary>
/// <param name="Name">The name of the current type.</param>
/// <param name="Namespace">The namespace the current type is.</param>
/// <param name="InheritanceFormattedNames">The interface the current type has, formatted by fully qualified names and separated by commas.</param>
internal sealed record ClassInfo(
    string Name,
    string Namespace,
    string? InheritanceFormattedNames)
{
    /// <summary>
    /// An <see cref="IEqualityComparer{T}"/> implementation for <see cref="ClassInfo"/>.
    /// </summary>
    public sealed class Comparer : Comparer<ClassInfo, Comparer>
    {
        /// <inheritdoc/>
        public override void AddToHashCode(ref HashCode hashCode, ClassInfo obj)
        {
            hashCode.Add(obj.Name);
            hashCode.Add(obj.Namespace);
            hashCode.Add(obj.InheritanceFormattedNames);
        }

        /// <inheritdoc/>
        public override bool AreEqual(ClassInfo x, ClassInfo y)
        {
            return
                x.Name == y.Name &&
                x.Namespace == y.Namespace &&
                x.InheritanceFormattedNames == y.InheritanceFormattedNames;
        }
    }
}
