using Skua.Core.Models.Items;

namespace Skua.Core.Interfaces;

public delegate void LogoutEventHandler();
public delegate void PlayerDeathEventHandler();
public delegate void MonsterKilledEventHandler(int mapId);
public delegate void QuestAcceptedEventHandler(int questId);
public delegate void QuestTurnInEventHandler(int questId);
public delegate void MapChangedEventHandler(string map);
public delegate void CellChangedEventHandler(string map, string cell, string pad);
public delegate void ReloginTriggeredEventHandler(bool wasKicked);
public delegate void ExtensionPacketEventHandler(dynamic packet);
public delegate void AFKEventHandler();
public delegate void TryBuyItemHandler(int shopId, int itemId, int shopItemId);
public delegate void CounterAttackHandler(bool faded);
public delegate void ItemDroppedHandler(ItemBase item, bool addedToInv, int quantityNow);
public delegate void ItemBoughtHandler(int CharItemID);
public delegate void ItemSoldHandler(int CharItemID, int QuantitySold, int CurrentQuantity, int Cost, bool IsAC);
public delegate void ItemAddedToBankHandler(ItemBase item, int quantityNow);
public delegate bool ScriptStoppingHandler(Exception? exception);
public delegate void RunToAreaHandler(string zone);

public interface IScriptEvent
{
    /// <summary>
    /// Occurs when the player log out of the game.
    /// </summary>
    event LogoutEventHandler Logout;
    /// <summary>
    /// Occurs when the player dies.
    /// </summary>
    event PlayerDeathEventHandler PlayerDeath;
    /// <summary>
    /// Occurs when the player kills a monster.
    /// </summary>
    event MonsterKilledEventHandler MonsterKilled;
    /// <summary>
    /// Occurs when a quest is accepted.
    /// </summary>
    event QuestAcceptedEventHandler QuestAccepted;
    /// <summary>
    /// Occurs when a quest is turned.
    /// </summary>
    event QuestTurnInEventHandler QuestTurnedIn;
    /// <summary>
    /// Occurs when the current map changes.
    /// </summary>
    /// <remarks>Note that the <see cref="MapChanged"/> is fired when you send the join command.<br></br>Before using any <see cref="IScriptMap"/> method in this event, be sure to add a delay.</remarks>
    event MapChangedEventHandler MapChanged;
    /// <summary>
    /// Occurs when the current cell changes.
    /// </summary>
    event CellChangedEventHandler CellChanged;
    /// <summary>
    /// Occurs when auto relogin has been triggered (but the relogin has not been carried out yet).
    /// </summary>
    event ReloginTriggeredEventHandler ReloginTriggered;
    /// <summary>
    /// Occurs when an extension packet is received.
    /// </summary>
    /// <remarks>The extension packet is a <see langword="dynamic"/> object deserialized from JSON.</remarks>
    event ExtensionPacketEventHandler ExtensionPacketReceived;
    /// <summary>
    /// Occurs when the player turns AFK.
    /// </summary>
    event AFKEventHandler PlayerAFK;
    /// <summary>
    /// Occurs when the player attempts to buy an item from a shop.
    /// </summary>
    event TryBuyItemHandler TryBuyItem;
    /// <summary>
    /// Occurs when a counter attack from an Ultra boss starts/fades.
    /// </summary>
    event CounterAttackHandler CounterAttack;
    /// <summary>
    /// Occurs when an item drops.
    /// </summary>
    event ItemDroppedHandler ItemDropped;
    /// <summary>
    /// Occurs when an item is sold.
    /// </summary>
    event ItemSoldHandler ItemSold;
    /// <summary>
    /// Occurs when an item is bought;
    /// </summary>
    event ItemBoughtHandler ItemBought;
    /// <summary>
    /// Occurs when an accepted item goes to bank.
    /// </summary>
    event ItemAddedToBankHandler ItemAddedToBank;
    /// <summary>
    /// Occurs when the script is finishing, you can place cleanup code here (like reset options and such).
    /// </summary>
    event ScriptStoppingHandler ScriptStopping;
    /// <summary>
    /// Occurs when a safe zone packet is received (Ledgermayne mechanic). </br>
    /// A is the Left zone, B is the Right zone, and "" (empty string) resets the zones.
    /// </summary>
    event RunToAreaHandler RunToArea;

    /// <summary>
    /// Clear all of the event handler subscribers.
    /// </summary>
    void ClearHandlers();
    //void OnLogout();
    //void OnCellChanged(string map, string cell, string pad);
    //void OnCounterAttack(bool faded);
    //void OnExtensionPacket(dynamic packet);
    //void OnItemDropped(ItemBase item, bool addedToInv = false, int quantityNow = 0);
    //void OnItemAddedToBank(ItemBase item, int quantityNow);
    //void OnMapChanged(string map);
    //void OnMonsterKilled(int mapId);
    //void OnPlayerAFK();
    //void OnPlayerDeath();
    //void OnQuestAccepted(int questId);
    //void OnQuestTurnIn(int questId);
    //void OnReloginTriggered(bool kicked);
    //void OnRunToArea(string zone);
    //Task<bool?> OnScriptStoppedAsync();
    //void OnTryBuyItem(int shopId, int itemId, int shopItemId);
}
