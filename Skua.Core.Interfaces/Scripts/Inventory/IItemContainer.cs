using Skua.Core.Models.Items;

namespace Skua.Core.Interfaces;
public interface IItemContainer<T> where T : ItemBase
{
    /// <summary>
    /// A list of items in this inventory.
    /// </summary>
    List<T> Items { get; }
}
