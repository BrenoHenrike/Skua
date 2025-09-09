using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Skua.Core.Utils;

/// <summary>
/// Implementation of a dynamic data collection based on generic <see cref="ObservableCollection{T}"/>,
/// implementing <see cref="INotifyCollectionChanged"/> to notify listeners
/// when items get added, removed or the whole list is refreshed.
/// </summary>
public class RangedObservableCollection<T> : ObservableCollection<T>
{
    private const string CountName = "Count";
    private const string IndexerName = "Item[]";

    /// <summary>
    /// Initializes a new instance of RangedObservableCollection that is empty and has default initial capacity.
    /// </summary>
    public RangedObservableCollection()
        : base() { }

    /// <summary>
    /// Initializes a new instance of the RangedObservableCollection class that contains
    /// elements copied from the specified collection and has sufficient capacity
    /// to accommodate the number of elements copied.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new list.</param>
    /// <remarks>
    /// The elements are copied onto the RangedObservableCollection in the
    /// same order they are read by the enumerator of the collection.
    /// </remarks>
    /// <exception cref="ArgumentNullException"> collection is a null reference </exception>
    public RangedObservableCollection(IEnumerable<T> collection)
        : base(collection) { }

    /// <summary>
    /// Adds the elements of the specified collection to the end of the RangedObservableCollection.
    /// </summary>
    /// <param name="collection">The collection whose elements are added to the list.</param>
    /// <remarks>
    /// The elements are copied onto the RangedObservableCollection in the
    /// same order they are read by the enumerator of the collection.
    /// </remarks>
    /// <exception cref="ArgumentNullException"> <paramref name="collection"/> is a null reference </exception>
    public void AddRange(IEnumerable<T> collection)
    {
        if (collection is null || !collection.Any())
            return;

        CheckReentrancy();

        int startIndex = Count;
        List<T> changedItems = new(collection);

        foreach (T i in changedItems)
            Items.Add(i);

        OnCountPropertyChanged();
        OnIndexerPropertyChanged();
        OnCollectionReset();
    }

    /// <summary>
    /// Clears the current RangedObservableCollection and replaces the elements with the elements of specified collection.
    /// </summary>
    /// <param name="collection">The collection whose elements are added to the list.</param>
    /// <remarks>
    /// The elements are copied onto the RangedObservableCollection in the
    /// same order they are read by the enumerator of the collection.
    /// </remarks>
    /// <exception cref="ArgumentNullException"> <paramref name="collection"/> is a null reference </exception>
    public void ReplaceRange(IEnumerable<T> collection)
    {
        if (collection is null || !collection.Any())
            return;

        CheckReentrancy();

        Items.Clear();
        foreach (T i in collection)
            Items.Add(i);

        OnCountPropertyChanged();
        OnIndexerPropertyChanged();
        OnCollectionReset();
    }

    /// <summary>
    /// Removes the first occurence of each item in the specified collection.
    /// </summary>
    /// <param name="collection">The collection whose elements are removed from list.</param>
    /// <remarks>
    /// The elements are copied onto the RangedObservableCollection in the
    /// same order they are read by the enumerator of the collection.
    /// </remarks>
    /// <exception cref="ArgumentNullException"> <paramref name="collection"/> is a null reference </exception>
    public void RemoveRange(IEnumerable<T> collection)
    {
        if (collection is null || !collection.Any())
            return;

        CheckReentrancy();

        List<T> changedItems = new(collection);
        for (int i = 0; i < changedItems.Count; i++)
        {
            if (!Items.Remove(changedItems[i]))
            {
                changedItems.RemoveAt(i);
                i--;
            }
        }

        if (changedItems.Count > 0)
        {
            OnCountPropertyChanged();
            OnIndexerPropertyChanged();
            OnCollectionReset();
        }
    }

    /// <summary>
    /// Clears the current RangedObservableCollection and replaces the elements with the specified element.
    /// </summary>
    /// <param name="item">The element which is added to the list.</param>
    public void Replace(T item)
    {
        if (item is null)
            return;

        CheckReentrancy();

        Items.Clear();
        Items.Add(item);

        OnCountPropertyChanged();
        OnIndexerPropertyChanged();
        OnCollectionReset();
    }

    private void OnCountPropertyChanged()
    {
        OnPropertyChanged(EventArgsCache.CountPropertyChanged);
    }

    private void OnIndexerPropertyChanged()
    {
        OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);
    }

    private void OnCollectionReset()
    {
        OnCollectionChanged(EventArgsCache.ResetCollectionChanged);
    }

    private static class EventArgsCache
    {
        internal static readonly PropertyChangedEventArgs CountPropertyChanged = new(CountName);
        internal static readonly PropertyChangedEventArgs IndexerPropertyChanged = new(IndexerName);
        internal static readonly NotifyCollectionChangedEventArgs ResetCollectionChanged = new(NotifyCollectionChangedAction.Reset);
    }
}