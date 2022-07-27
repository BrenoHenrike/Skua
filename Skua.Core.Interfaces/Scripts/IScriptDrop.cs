using System.ComponentModel;
using Skua.Core.Models.Items;
using Skua.Core.Utils;

namespace Skua.Core.Interfaces;
public interface IScriptDrop : INotifyPropertyChanged
{
    /// <summary>
    /// Get a list of drops available in the drop stack (even if they aren't in the screen).
    /// </summary>
    IEnumerable<ItemBase> CurrentDropInfos { get; }
    /// <summary>
    /// Get a list of item names available on the drop stack (even if they aren't in the screen).
    /// </summary>
    IEnumerable<string> CurrentDrops { get; }
    /// <summary>
    /// Whether the drop grabber is enabled.
    /// </summary>
    bool Enabled { get; }
    /// <summary>
    /// The interval, in milliseconds, at which to pickup the drops (the base interval is 1000ms, any value assigned to this property will be added to that interval).
    /// </summary>
    int Interval { get; set; }
    /// <summary>
    /// Whether to reject drops not included in <see cref="ToPickup"/>.
    /// </summary>
    bool RejectElse { get; set; }
    /// <summary>
    /// The list of item names to pickup every <see cref="Interval"/>.
    /// </summary>
    IEnumerable<string> ToPickup { get; }
    /// <summary>
    /// The list of item IDs to pickup every <see cref="Interval"/>
    /// </summary>
    IEnumerable<int> ToPickupIDs { get; }
    /// <summary>
    /// Adds the specified items in the <see cref="ToPickup"/> list.
    /// </summary>
    /// <param name="names">Name of the items to be added in <see cref="ToPickup"/> list.</param>
    void Add(params string[] names);
    /// <summary>
    /// Adds the specified items in the <see cref="ToPickupIDs"/> list.
    /// </summary>
    /// <param name="ids">IDs of the items to be added in <see cref="ToPickupIDs"/> list.</param>
    void Add(params int[] ids);
    /// <summary>
    /// Removes the specified items from <see cref="ToPickup"/>
    /// </summary>
    /// <param name="names">Names of the items to be removed from <see cref="ToPickup"/></param>
    void Remove(params string[] names);
    /// <summary>
    /// Removes the specified items from <see cref="ToPickupIDs"/>
    /// </summary>
    /// <param name="ids">Names of the items to be removed from <see cref="ToPickupIDs"/></param>
    void Remove(params int[] ids);
    /// <summary>
    /// Starts the drop grabber.
    /// </summary>
    void Start();
    /// <summary>
    /// Stops the drop grabber.
    /// </summary>
    void Stop();
    /// <summary>
    /// Stops the drop grabber.
    /// </summary>
    ValueTask StopAsync();
    /// <summary>
    /// Removes all items from the pickup list.
    /// </summary>
    void Clear();
    /// <summary>
    /// Checks if a drop with the specified <paramref name="name"/> exists.
    /// </summary>
    /// <param name="name">Name of the item.</param>
    /// <returns><see langword="true"/> if a drop with the specified <paramref name="name"/> exists.</returns>
    bool Exists(string name)
    {
        return (name == "*" && CurrentDrops.Any()) || CurrentDrops.Contains(name, StringComparer.OrdinalIgnoreCase);
    }
    /// <summary>
    /// Checks if a drop with the specified <paramref name="id"/> exists.
    /// </summary>
    /// <param name="id">ID of the item.</param>
    /// <returns><see langword="true"/> if a drop with the specified <paramref name="id"/> exists.</returns>
    bool Exists(int id)
    {
        return CurrentDropInfos.Contains(x => x.ID == id);
    }
    /// <summary>
    /// Pickup the item with specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">Name of the item to pickup.</param>
    /// <remarks>If the item doesn't exist in the drop stack, this method will do nothing.</remarks>
    void Pickup(string name);
    /// <summary>
    /// Pickup the item with specified <paramref name="id"/>.
    /// </summary>
    /// <param name="id">Name of the item to pickup.</param>
    /// <remarks>If the item doesn't exist in the drop stack, this method will do nothing.</remarks>
    void Pickup(int id);
    /// <summary>
    /// Pickup all the items with the names specified in <paramref name="names"/>
    /// </summary>
    /// <param name="names">Names of the items to pickup.</param>
    void Pickup(params string[] names);
    /// <summary>
    /// Pickup all the items with the IDs specified in <paramref name="ids"/>
    /// </summary>
    /// <param name="names">IDs of the items to pickup.</param>
    void Pickup(params int[] ids);
    /// <summary>
    /// Pickup all the items which are AC-tagged.
    /// </summary>
    void PickupACItems()
    {
        Pickup(CurrentDropInfos.Where(d => d.Coins).Select(d => d.Name).ToArray());
    }
    /// <summary>
    /// Pickup all available drops (even if they aren't in the screen).
    /// </summary>
    /// <param name="skipWait">Whether <see cref="IScriptOption.SafeTimings"/> is ignored.</param>
    void PickupAll(bool skipWait = false);
    /// <summary>
    /// Picks up the items with specified <paramref name="names"/>, without waiting for the items to be picked up. This method disregards the <see cref="IScriptOption.SafeTimings"/> option.
    /// </summary>
    /// <param name="names">Names of the items to pickup.</param>
    void PickupFast(params string[] names)
    {
        for (int i = 0; i < names.Length; i++)
            Pickup(names[i]);
    }
    /// <summary>
    /// Picks up the items with specified <paramref name="ids"/>, without waiting for the items to be picked up. This method disregards the <see cref="IScriptOption.SafeTimings"/> option.
    /// </summary>
    /// <param name="ids">IDs of the items to pickup</param>
    void PickupFast(params int[] ids)
    {
        for (int i = 0; i < ids.Length; i++)
            Pickup(ids[i]);
    }
    /// <summary>
    /// Rejects all available drops.
    /// </summary>
    /// <param name="skipWait">Whether <see cref="IScriptOption.SafeTimings"/> is ignored.</param>
    void RejectAll(bool skipWait = false);
    /// <summary>
    /// Rejects all drops except those in the specified list of <paramref name="names"/>.
    /// </summary>
    /// <param name="names">Names of the items to not reject.</param>
    void RejectExcept(params string[] names);
    /// <summary>
    /// Rejects all drops except those in the specified list of <paramref name="ids"/>.
    /// </summary>
    /// <param name="ids">IDs of the items to not reject.</param>
    void RejectExcept(params int[] ids);
}
