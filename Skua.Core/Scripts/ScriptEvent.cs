using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;

namespace Skua.Core.Scripts;

public class ScriptEvent : IScriptEvent, IDisposable
{
    public ScriptEvent()
    {
        _messenger = StrongReferenceMessenger.Default;
        _messenger.Register<ScriptEvent, ScriptStoppingRequestMessage, int>(this, (int)MessageChannels.ScriptStatus, OnScriptStopping);
        _messenger.Register<ScriptEvent, ScriptStartedMessage, int>(this, (int)MessageChannels.ScriptStatus, ScriptStarted);
        _messenger.Register<ScriptEvent, ScriptStoppedMessage, int>(this, (int)MessageChannels.ScriptStatus, ScriptStopped);
    }

    private void RegisterGameEvents()
    {
        _messenger.Register<ScriptEvent, LogoutMessage, int>(this, (int)MessageChannels.GameEvents, OnLogout);
        _messenger.Register<ScriptEvent, PlayerDeathMessage, int>(this, (int)MessageChannels.GameEvents, OnPlayerDeath);
        _messenger.Register<ScriptEvent, MonsterKilledMessage, int>(this, (int)MessageChannels.GameEvents, OnMonsterKilled);
        _messenger.Register<ScriptEvent, QuestAcceptedMessage, int>(this, (int)MessageChannels.GameEvents, OnQuestAccepted);
        _messenger.Register<ScriptEvent, QuestTurninMessage, int>(this, (int)MessageChannels.GameEvents, OnQuestTurnIn);
        _messenger.Register<ScriptEvent, MapChangedMessage, int>(this, (int)MessageChannels.GameEvents, OnMapChanged);
        _messenger.Register<ScriptEvent, CellChangedMessage, int>(this, (int)MessageChannels.GameEvents, OnCellChanged);
        _messenger.Register<ScriptEvent, ReloginTriggeredMessage, int>(this, (int)MessageChannels.GameEvents, OnReloginTriggered);
        _messenger.Register<ScriptEvent, ExtensionPacketMessage, int>(this, (int)MessageChannels.GameEvents, OnExtensionPacket);
        _messenger.Register<ScriptEvent, PacketMessage, int>(this, (int)MessageChannels.GameEvents, OnPacketReceived);
        _messenger.Register<ScriptEvent, PlayerAFKMessage, int>(this, (int)MessageChannels.GameEvents, OnPlayerAFK);
        _messenger.Register<ScriptEvent, TryBuyItemMessage, int>(this, (int)MessageChannels.GameEvents, OnTryBuyItem);
        _messenger.Register<ScriptEvent, CounterAttackMessage, int>(this, (int)MessageChannels.GameEvents, OnCounterAttack);
        _messenger.Register<ScriptEvent, ItemDroppedMessage, int>(this, (int)MessageChannels.GameEvents, OnItemDropped);
        _messenger.Register<ScriptEvent, ItemBoughtMessage, int>(this, (int)MessageChannels.GameEvents, OnItemBought);
        _messenger.Register<ScriptEvent, ItemSoldMessage, int>(this, (int)MessageChannels.GameEvents, OnItemSold);
        _messenger.Register<ScriptEvent, ItemAddedToBankMessage, int>(this, (int)MessageChannels.GameEvents, OnItemAddedToBank);
        _messenger.Register<ScriptEvent, RunToAreaMessage, int>(this, (int)MessageChannels.GameEvents, OnRunToArea);
    }

    private void UnregisterGameEvents()
    {
        _messenger.UnregisterAll(this, (int)MessageChannels.GameEvents);
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

    public event PacketEventHandler? PacketReceived;

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

    private void ScriptStopped(ScriptEvent recipient, ScriptStoppedMessage message)
    {
        recipient.UnregisterGameEvents();
        recipient.ClearHandlers();
    }

    private void ScriptStarted(ScriptEvent recipient, ScriptStartedMessage message)
    {
        recipient.RegisterGameEvents();
    }

    public void OnLogout(ScriptEvent recipient, LogoutMessage message)
    {
        recipient.Logout?.Invoke();
    }

    public void OnRunToArea(ScriptEvent recipient, RunToAreaMessage message)
    {
        recipient.RunToArea?.Invoke(message.Zone);
    }

    public void OnScriptStopping(ScriptEvent recipient, ScriptStoppingRequestMessage message)
    {
        message.Reply(Task.Run(() => recipient.ScriptStopping?.Invoke(message.Exception)));
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
        if (IScriptInterface.Instance.Monsters.MapMonsters.Any(m => m.MapID == message.MapID))
            IScriptInterface.Instance.Monsters.MapMonsters.First(m => m.MapID == message.MapID).Alive = false;
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

    private void OnPacketReceived(ScriptEvent recipient, PacketMessage message)
    {
        recipient.PacketReceived?.Invoke(message.Packet);
    }

    public void OnPlayerAFK(ScriptEvent recipient, PlayerAFKMessage message)
    {
        recipient.PlayerAFK?.Invoke();
    }

    public void OnTryBuyItem(ScriptEvent recipient, TryBuyItemMessage message)
    {
        recipient.TryBuyItem?.Invoke(message.ShopID, message.ItemID, message.ShopItemID);
    }

    private bool _disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Unregister from all messenger events
                _messenger.UnregisterAll(this);

                // Clear all event handlers
                ClearHandlers();

                // Clear additional handlers that weren't in ClearHandlers
                Logout = null;
                PacketReceived = null;
                ItemBought = null;
                ItemSold = null;
                ItemAddedToBank = null;
            }

            _disposed = true;
        }
    }

    ~ScriptEvent()
    {
        Dispose(false);
    }
}