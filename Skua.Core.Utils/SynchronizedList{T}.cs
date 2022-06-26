namespace Skua.Core.Utils;
/// <summary>
/// A list that allows synchronous operations.
/// </summary>
/// <typeparam name="T">Type contained in the list.</typeparam>
public class SynchronizedList<T>
{
    private readonly List<T> list = new();
    private readonly object sync = new();

    /// <summary>
    /// The items contained in this list.
    /// </summary>
    public List<T> Items => list;

    /// <summary>
    /// Adds an object to the end of the <see cref="List{T}"/>.
    /// </summary>
    /// <param name="value">The object to be added to the end of the <see cref="List{T}"/>. The value can be null for reference types.</param>
    public void Add(T value)
    {
        lock (sync)
            list.Add(value);
    }

    /// <summary>
    /// Adds the elements of the specified collection to the end of the <see cref="List{T}"/>
    /// </summary>
    /// <param name="values">The collection whose elements should be added to the end of the <see cref="List{T}"/>. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
    /// <exception cref="ArgumentNullException" />
    public void AddRange(IEnumerable<T> values)
    {
        lock (sync)
            list.AddRange(values);
    }

    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="List{T}"/>
    /// </summary>
    /// <param name="item">The object to remove from the <see cref="List{T}"/>. The value can be null for reference types.</param>
    /// <returns></returns>
    public bool Remove(T item)
    {
        lock (sync)
            return list.Remove(item);
    }

    /// <summary>
    /// Removes all the elements that match the conditions defined by the specified predicate.
    /// </summary>
    /// <param name="predicate">The <see cref="Predicate{T}"/> delegate that defines the conditions of the elements to remove.</param>
    /// <returns>The number of elements removed from the <see cref="List{T}"/>.</returns>
    public int Remove(Predicate<T> predicate)
    {
        lock (sync)
            return list.RemoveAll(predicate);
    }

    /// <summary>
    /// Searches for an element that matches the conditions defined by the specified predicate, and returns the first occurrence within the entire <see cref="List{T}"/>.
    /// </summary>
    /// <param name="predicate">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
    /// <returns>The first element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type T.</returns>
    public T? Find(Predicate<T> predicate)
    {
        return list.Find(predicate);
    }

    /// <summary>
    /// Removes all elements of the <see cref="List{T}"/>.
    /// </summary>
    public void Clear()
    {
        lock (sync)
            list.Clear();
    }

    /// <summary>
    /// Performs the specified action on each element of the <see cref="List{T}"/>.
    /// </summary>
    /// <param name="action">The <see cref="Action{T}"/> delegate to perform on each element of the <see cref="List{T}"/>.</param>
    public void ForEach(Action<T> action)
    {
        lock (sync)
            list.ForEach(action);
    }
}
