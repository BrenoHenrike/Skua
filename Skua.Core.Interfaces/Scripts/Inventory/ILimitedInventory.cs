namespace Skua.Core.Interfaces;
public interface ILimitedInventory
{
    /// <summary>
    /// The total number of slots in this inventory.
    /// </summary>
    int Slots { get; }
    /// <summary>
    /// The number of slots that are currently in use in this inventory.
    /// </summary>
    int UsedSlots { get; }
    /// <summary>
    /// The number of free slots the player has in this inventory.
    /// </summary>
    int FreeSlots => Slots - UsedSlots;
}
