using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skua.Core.Interfaces;
public interface IScriptInventoryHelper
{
    /// <summary>
    /// Check the bank, player, temporary and house inventory for the item
    /// </summary>
    /// <param name="name">Name of the item.</param>
    /// <param name="quantity">Desired quantity.</param>
    /// <param name="moveToInventory">Whether or not send the item to Inventory.</param>
    /// <returns>Whether the item exists in the desired quantity in the bank, player, temporary and house inventory.</returns>
    public bool Check(string name, int quantity = 1, bool moveToInventory = true);
    /// <summary>
    /// Check the bank, player, temporary and house inventory for the item
    /// </summary>
    /// <param name="id">ID of the item.</param>
    /// <param name="quantity">Desired quantity</param>
    /// <param name="moveToInventory">Whether or not send the item to Inventory.</param>
    /// <returns>Whether the item exists in the desired quantity in the bank, player, temporary and house inventory.</returns>
    public bool Check(int id, int quantity = 1, bool moveToInventory = true);
    /// <summary>
    /// Check if the bank/inventory has all listed items.
    /// </summary>
    /// <param name="itemNames">Names of the items to be checked.</param>
    /// <param name="quantity">Desired quantity.</param>
    /// <param name="moveToInventory">Whether or not send the item to Inventory.</param>
    /// <returns>Returns whether all the items exist in the bank or player inventory.</returns>
    public bool HasAll(IEnumerable<string> itemNames, int quantity = 1, bool moveToInventory = true);
    /// <summary>
    /// Check if the bank/inventory has at least 1 of all listed items.
    /// </summary>
    /// <param name="itemNames">Array of names of the items to be check</param>
    /// <param name="quantity">Desired quantity.</param>
    /// <param name="moveToInventory">Whether or not send the item to Inventory</param>
    /// <returns>Returns whether atleast 1 of the items exist in the bank or player inventory.</returns>
    public bool HasAny(IEnumerable<string> itemNames, int quantity = 1, bool moveToInventory = true);
    /// <summary>
    /// Check if the bank/inventory has all listed items.
    /// </summary>
    /// <param name="itemIds">Names of the items to be checked.</param>
    /// <param name="quantity">Desired quantity.</param>
    /// <param name="moveToInventory">Whether or not send the item to Inventory.</param>
    /// <returns>Returns whether all the items exist in the bank or player inventory.</returns>
    public bool HasAll(IEnumerable<int> itemIds, int quantity = 1, bool moveToInventory = true);
    /// <summary>
    /// Check if the bank/inventory has at least 1 of all listed items.
    /// </summary>
    /// <param name="itemIds">Array of names of the items to be check</param>
    /// <param name="quantity">Desired quantity.</param>
    /// <param name="moveToInventory">Whether or not send the item to Inventory</param>
    /// <returns>Returns whether atleast 1 of the items exist in the bank or player inventory.</returns>
    public bool HasAny(IEnumerable<int> itemIds, int quantity = 1, bool moveToInventory = true);
}
