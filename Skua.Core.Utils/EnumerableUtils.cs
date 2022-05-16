namespace Skua.Core.Utils;

public delegate void Consumer<T>(T arg);

public static class EnumerableUtils
{
    public static void ForEach<T>(this IEnumerable<T> enumerable, Consumer<T> func)
    {
        foreach (T item in enumerable)
            func(item);
    }

    public static bool Contains<T>(this IEnumerable<T> enumerable, Predicate<T> pred)
    {
        foreach (T item in enumerable)
            if (pred(item))
                return true;
        return false;
    }

    public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB)
    {
        (list[indexB], list[indexA]) = (list[indexA], list[indexB]);
        return list;
    }

    public static IEnumerable<Tuple<T, T>> PairUp<T>(this IEnumerable<T> list)
    {
        using IEnumerator<T> iterator = list.GetEnumerator();
        while (iterator.MoveNext())
        {
            var first = iterator.Current;
            var second = iterator.MoveNext() ? iterator.Current : default;
            yield return Tuple.Create(first, second);
        }
    }
}
