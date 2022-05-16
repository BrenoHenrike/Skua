
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

namespace Skua.Core.Scripts;

public class ScriptEvent : ScriptableObject, IScriptEvent
{
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

    public void OnRunToArea(string zone)
    {
        Task.Run(() => RunToArea?.Invoke(zone));
    }

    public async Task<bool?> OnScriptStoppedAsync()
    {
        return await Task.Run(() => ScriptStopping?.Invoke());
    }

    public void OnItemDropped(ItemBase item, bool addedToInv = false, int quantityNow = 0)
    {
        Task.Run(() => ItemDropped?.Invoke(item, addedToInv, quantityNow));
    }

    public void OnCounterAttack(bool faded)
    {
        Task.Run(() => CounterAttack?.Invoke(faded));
    }

    public void OnPlayerDeath()
    {
        Task.Run(() => PlayerDeath?.Invoke());
    }

    public void OnMonsterKilled()
    {
        Task.Run(() => MonsterKilled?.Invoke());
    }

    public void OnQuestAccepted(int questId)
    {
        Task.Run(() => QuestAccepted?.Invoke(questId));
    }

    public void OnQuestTurnIn(int questId)
    {
        Task.Run(() => QuestTurnedIn?.Invoke(questId));
    }

    public void OnMapChanged(string map)
    {
        Task.Run(() => MapChanged?.Invoke(map));
    }

    public void OnCellChanged(string map, string cell, string pad)
    {
        Task.Run(() => CellChanged?.Invoke(map, cell, pad));
    }

    public void OnReloginTriggered(bool kicked)
    {
        Task.Run(() => ReloginTriggered?.Invoke(kicked));
    }

    public void OnExtensionPacket(dynamic packet)
    {
        Task.Run(() => { ExtensionPacketReceived?.Invoke(packet); });
    }

    public void OnPlayerAFK()
    {
        Task.Run(() => PlayerAFK?.Invoke());
    }

    public void OnTryBuyItem(int shopId, int itemId, int shopItemId)
    {
        Task.Run(() => TryBuyItem?.Invoke(shopId, itemId, shopItemId));
    }
}
