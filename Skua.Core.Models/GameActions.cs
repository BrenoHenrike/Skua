using Newtonsoft.Json;

namespace Skua.Core.Models;

/// <summary>
/// An enumeration of actions that the game requires to be cooled down before use.
/// </summary>
public enum GameActions
{
    /// <summary>
    /// Loading a shop.
    /// </summary>
    LoadShop,
    /// <summary>
    /// Loading an enhancement shop.
    /// </summary>
    LoadEnhShop,
    /// <summary>
    /// Loading a hair shop.
    /// </summary>
    LoadHairShop,
    /// <summary>
    /// Equipping an item.
    /// </summary>
    EquipItem,
    /// <summary>
    /// Unequipping an ite.
    /// </summary>
    UnequipItem,
    /// <summary>
    /// Buying an item.
    /// </summary>
    BuyItem,
    /// <summary>
    /// Selling an item.
    /// </summary>
    SellItem,
    /// <summary>
    /// Getting a map item (i.e. via the getMapItem packet).
    /// </summary>
    GetMapItem,
    /// <summary>
    /// Sending a quest completion packet.
    /// </summary>
    TryQuestComplete,
    /// <summary>
    /// Accepting a quest.
    /// </summary>
    AcceptQuest,
    /// <summary>
    /// I don't know...
    /// </summary>
    DoIA,
    /// <summary>
    /// Resting.
    /// </summary>
    Rest,
    /// <summary>
    /// I don't know...
    /// </summary>
    Who,
    /// <summary>
    /// Joining another map.
    /// </summary>
    Transfer
}

public struct GameActionLock
{
    [JsonProperty("cd")]
    public long? CD { get; }
    [JsonProperty("ts")]
    public long? TS { get; }
}
