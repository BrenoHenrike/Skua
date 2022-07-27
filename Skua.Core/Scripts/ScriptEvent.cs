using Microsoft.Toolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;

namespace Skua.Core.Scripts;
public class ScriptEvent : IScriptEvent
{
    public ScriptEvent(IMessenger messenger)
    {
        _messenger = messenger;
        _messenger.Register<ScriptEvent, LogoutMessage>(this, OnLogout);
        _messenger.Register<ScriptEvent, PlayerDeathMessage>(this, OnPlayerDeath);
        _messenger.Register<ScriptEvent, MonsterKilledMessage>(this, OnMonsterKilled);
        _messenger.Register<ScriptEvent, QuestAcceptedMessage>(this, OnQuestAccepted);
        _messenger.Register<ScriptEvent, QuestTurninMessage>(this, OnQuestTurnIn);
        _messenger.Register<ScriptEvent, MapChangedMessage>(this, OnMapChanged);
        _messenger.Register<ScriptEvent, CellChangedMessage>(this, OnCellChanged);
        _messenger.Register<ScriptEvent, ReloginTriggeredMessage>(this, OnReloginTriggered);
        _messenger.Register<ScriptEvent, ExtensionPacketMessage>(this, OnExtensionPacket);
        _messenger.Register<ScriptEvent, PlayerAFKMessage>(this, OnPlayerAFK);
        _messenger.Register<ScriptEvent, TryBuyItemMessage>(this, OnTryBuyItem);
        _messenger.Register<ScriptEvent, CounterAttackMessage>(this, OnCounterAttack);
        _messenger.Register<ScriptEvent, ItemDroppedMessage>(this, OnItemDropped);
        _messenger.Register<ScriptEvent, ItemBoughtMessage>(this, OnItemBought);
        _messenger.Register<ScriptEvent, ItemSoldMessage>(this, OnItemSold);
        _messenger.Register<ScriptEvent, ItemAddedToBankMessage>(this, OnItemAddedToBank);
        _messenger.Register<ScriptEvent, RunToAreaMessage>(this, OnRunToArea);
        _messenger.Register<ScriptEvent, ScriptStoppingMessage>(this, OnScriptStopping);
    }

    private readonly IMessenger _messenger;

    public event LogoutEventHandler? Logout;
    public event PlayerDeathEventHandler? PlayerDeath;
    public event MonsterKilledEventHandler? MonsterKilled;
    public event QuestAcceptedEventHandler? QuestAccepted;
    public event QuestTurnInEventHandler? QuestTurnedIn;
    public event MapChangedEventHandler? MapChanged;
    public event CellChangedEventHandler? CellChanged;
    public event ReloginTriggeredEventHandler? ReloginTriggered;
    public event ExtensionPacketEventHandler? ExtensionPacketReceived;
    public event AFKEventHandler? PlayerAFK;
    public event TryBuyItemHandler? TryBuyItem;
    public event CounterAttackHandler? CounterAttack;
    public event ItemDroppedHandler? ItemDropped;
    public event ItemSoldHandler? ItemSold;
    public event ItemBoughtHandler? ItemBought;
    public event ItemAddedToBankHandler? ItemAddedToBank;
    public event ScriptStoppingHandler? ScriptStopping;
    public event RunToAreaHandler? RunToArea;

    public void ClearHandlers()
    {
        PlayerDeath = null;
        MonsterKilled = null;
        QuestAccepted = null;
        QuestTurnedIn = null;
        MapChanged = null;
        CellChanged = null;
        ReloginTriggered = null;
        ExtensionPacketReceived = null;
        PlayerAFK = null;
        TryBuyItem = null;
        CounterAttack = null;
        ItemDropped = null;
        ScriptStopping = null;
        RunToArea = null;
    }

    public void OnLogout(ScriptEvent recipient, LogoutMessage message)
    {
        recipient.Logout?.Invoke();
    }

    public void OnRunToArea(ScriptEvent recipient, RunToAreaMessage message)
    {
        recipient.RunToArea?.Invoke(message.Zone);
    }

    public void OnScriptStopping(ScriptEvent recipient, ScriptStoppingMessage message)
    {
        message.Reply(Task.Run(() => recipient.ScriptStopping?.Invoke()));
    }

    public void OnItemDropped(ScriptEvent recipient, ItemDroppedMessage message)
    {
        recipient.ItemDropped?.Invoke(message.Item, message.AddedToInv, message.QuantityNow);
    }

    private void OnItemSold(ScriptEvent recipient, ItemSoldMessage message)
    {
        recipient.ItemSold?.Invoke(message.CharItemID, message.QuantitySold, message.CurrentQuantity, message.Cost, message.IsAC);
    }

    private void OnItemBought(ScriptEvent recipient, ItemBoughtMessage message)
    {
        recipient.ItemBought?.Invoke(message.CharItemID);
    }

    public void OnItemAddedToBank(ScriptEvent recipient, ItemAddedToBankMessage message)
    {
        recipient.ItemAddedToBank?.Invoke(message.Item, message.QuantityNow);
    }

    public void OnCounterAttack(ScriptEvent recipient, CounterAttackMessage message)
    {
        recipient.CounterAttack?.Invoke(message.Faded);
    }

    public void OnPlayerDeath(ScriptEvent recipient, PlayerDeathMessage message)
    {
        recipient.PlayerDeath?.Invoke();
    }

    public void OnMonsterKilled(ScriptEvent recipient, MonsterKilledMessage message)
    {
        recipient.MonsterKilled?.Invoke(message.MapID);
    }

    public void OnQuestAccepted(ScriptEvent recipient, QuestAcceptedMessage message)
    {
        recipient.QuestAccepted?.Invoke(message.QuestID);
    }

    public void OnQuestTurnIn(ScriptEvent recipient, QuestTurninMessage message)
    {
        recipient.QuestTurnedIn?.Invoke(message.QuestID);
    }

    public void OnMapChanged(ScriptEvent recipient, MapChangedMessage message)
    {
        recipient.MapChanged?.Invoke(message.Map);
    }

    public void OnCellChanged(ScriptEvent recipient, CellChangedMessage message)
    {
        recipient.CellChanged?.Invoke(message.Map, message.Cell, message.Pad);
    }

    public void OnReloginTriggered(ScriptEvent recipient, ReloginTriggeredMessage message)
    {
        recipient.ReloginTriggered?.Invoke(message.WasKicked);
    }

    public void OnExtensionPacket(ScriptEvent recipient, ExtensionPacketMessage message)
    {
        recipient.ExtensionPacketReceived?.Invoke(message.Packet);
    }

    public void OnPlayerAFK(ScriptEvent recipient, PlayerAFKMessage message)
    {
        recipient.PlayerAFK?.Invoke();
    }

    public void OnTryBuyItem(ScriptEvent recipient, TryBuyItemMessage message)
    {
        recipient.TryBuyItem?.Invoke(message.ShopID, message.ItemID, message.ShopItemID);
    }
}
