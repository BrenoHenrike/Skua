using System;
using System.Collections.Generic;
using Skua.Core.Generators.Helpers;

namespace Skua.Core.Generators.Models;
public sealed record MethodCallBindingPropertyInfo(
    string MethodName,
    string NewMethodName,
    string ReturnType,
    string MethodParams,
    string[] MethodParamNames,
    string MethodBody,
    MethodCallBindingValues Values)
{
    /// <summary>
    /// An <see cref="IEqualityComparer{T}"/> implementation for <see cref="MethodCallBindingPropertyInfo"/>.
    /// </summary>
    public sealed class Comparer : Comparer<MethodCallBindingPropertyInfo, Comparer>
    {
        /// <inheritdoc/>
        public override void AddToHashCode(ref HashCode hashCode, MethodCallBindingPropertyInfo obj)
        {
            hashCode.Add(obj.MethodName);
            hashCode.Add(obj.NewMethodName);
            hashCode.Add(obj.ReturnType);
            hashCode.Add(obj.MethodParams);
            hashCode.Add(obj.MethodBody);
            hashCode.Add(obj.Values.Path);
        }

        /// <inheritdoc/>
        public override bool AreEqual(MethodCallBindingPropertyInfo x, MethodCallBindingPropertyInfo y)
        {
            return
                x.MethodName == y.MethodName &&
                x.NewMethodName == y.NewMethodName &&
                x.ReturnType == y.ReturnType &&
                x.MethodParams == y.MethodParams &&
                x.MethodBody == y.MethodBody &&
                x.Values.Path == y.Values.Path;
        }
    }
}
