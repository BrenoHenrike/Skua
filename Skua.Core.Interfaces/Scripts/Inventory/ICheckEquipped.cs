using Skua.Core.Models.Items;

namespace Skua.Core.Interfaces;

public interface ICheckEquipped : ICanBank
{
    /// <summary>
    /// Checks if the item with specified <paramref name="name"/> is equipped.
    /// </summary>
    /// <param name="name">Name of the item.</param>
    /// <returns><see langword="true"/> if the item is equipped.</returns>
    bool IsEquipped(string name)
    {
        return TryGetItem(name, out InventoryItem? item) && item!.Equipped;
    }
    /// <summary>
    /// Checks if the item with specified <paramref name="id"/> is equipped.
    /// </summary>
    /// <param name="id">Name of the item.</param>
    /// <returns><see langword="true"/> if the item is equipped.</returns>
    bool IsEquipped(int id)
    {
        return TryGetItem(id, out InventoryItem? item) && item!.Equipped;
    }
}
