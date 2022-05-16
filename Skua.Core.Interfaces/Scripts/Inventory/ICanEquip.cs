using Skua.Core.Models.Items;

namespace Skua.Core.Interfaces;
public interface ICanEquip : ICheckEquipped
{
    /// <summary>
    /// Equips the item with specified <paramref name="id"/>. This will do nothing if the item is not in the player's inventory.
    /// </summary>
    /// <param name="id">ID of the item to equip.</param>
    void EquipItem(int id);
    /// <summary>
    /// Equips the item with specified <paramref name="name"/>. This will do nothing if the item is not in the player's inventory.
    /// </summary>
    /// <param name="name">Name of the item to equip.</param>
    void EquipItem(string name)
    {
        if (TryGetItem(name, out InventoryItem? item))
            EquipItem(item!.ID);
    }
    /// <summary>
    /// Equips the items with specified <paramref name="names"/>. This will do nothing if the item is not in the player's inventory.
    /// </summary>
    /// <param name="names">Names of the items to equip.</param>
    void EquipItems(params string[] names)
    {
        for (int i = 0; i < names.Length; i++)
        {
            if (TryGetItem(names[i], out InventoryItem? item))
                EquipItem(item!.ID);
        }
    }
    /// <summary>
    /// Equips the item with specified <paramref name="ids"/>. This will do nothing if the item is not in the player's inventory.
    /// </summary>
    /// <param name="ids">IDs of the items to equip.</param>
    void EquipItems(params int[] ids)
    {
        for (int i = 0; i < ids.Length; i++)
        {
            EquipItem(ids[i]);
        }
    }
}
