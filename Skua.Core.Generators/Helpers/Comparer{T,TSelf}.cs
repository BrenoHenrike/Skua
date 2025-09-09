using System;
using System.Collections.Generic;

namespace Skua.Core.Generators.Helpers;

/// <summary>
/// A base <see cref="IEqualityComparer{T}"/> implementation for <typeparamref name="T"/> instances.
/// </summary>
/// <typeparam name="T">The type of items to compare.</typeparam>
/// <typeparam name="TSelf">The concrete comparer type.</typeparam>
public abstract class Comparer<T, TSelf> : IEqualityComparer<T>
    where TSelf : Comparer<T, TSelf>, new()
{
    /// <summary>
    /// The singleton <typeparamref name="TSelf"/> instance.
    /// </summary>
    public static TSelf Default { get; } = new();

    /// <inheritdoc/>
    public bool Equals(T? x, T? y)
    {
        if (x is null && y is null)
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        if (ReferenceEquals(x, y))
        {
            return true;
        }

        return AreEqual(x, y);
    }

    /// <inheritdoc/>
    public int GetHashCode(T obj)
    {
        HashCode hashCode = default;

        AddToHashCode(ref hashCode, obj);

        return hashCode.ToHashCode();
    }

    /// <summary>
    /// Adds the current instance to an incremental <see cref="HashCode"/> value.
    /// </summary>
    /// <param name="hashCode">The target <see cref="HashCode"/> value.</param>
    /// <param name="obj">The <typeparamref name="T"/> instance being inspected.</param>
    public abstract void AddToHashCode(ref HashCode hashCode, T obj);

    /// <summary>
    /// Compares two <typeparamref name="T"/> instances for equality.
    /// </summary>
    /// <param name="x">The first <typeparamref name="T"/> instance to compare.</param>
    /// <param name="y">The second <typeparamref name="T"/> instance to compare.</param>
    /// <returns>Whether or not <paramref name="x"/> and <paramref name="y"/> are equal.</returns>
    public abstract bool AreEqual(T x, T y);
}