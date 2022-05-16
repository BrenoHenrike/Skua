
using Skua.Core.Utils;
using Skua.Core.PostSharp;
using System.Diagnostics;
using Skua.Core.Models.Quests;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Items;

namespace Skua.Core.Scripts;

public class ScriptQuest : ScriptableObject, IScriptQuest
{
    private Thread? QuestThread;
    private CancellationTokenSource? QuestsCTS;
    private readonly List<int> _add = new();
    private readonly List<int> _rem = new();
    public int RegisterCompleteInterval { get; set; } = 2000;
    [ObjectBinding("world.questTree")]
    private Dictionary<int, Quest> _quests { get; } = new();
    public List<Quest> Tree => _quests.Values.ToList();
    public List<Quest> Active => Tree.FindAll(x => x.Active);
    public List<Quest> Completed => Tree.FindAll(x => x.Status == "c");
    public Dictionary<int, Quest> Cached { get; set; } = new();
    public List<int> Registered { get; } = new();

    public void Load(params int[] ids)
    {
        Bot.Flash.CallGameFunction("world.showQuests", ids.Cast<string>().ToArray().Join(','), "q");
    }

    public Quest EnsureLoad(int id)
    {
        Bot.Wait.ForTrue(() => Tree.Contains(x => x.ID == id), () => Load(id), 20);
        return Tree.Find(q => q.ID == id)!;
    }

    public bool TryGetQuest(int id, out Quest? quest)
    {
        return (quest = Tree.Find(x => x.ID == id)) is not null;
    }

    public bool Accept(int id)
    {
        if (Bot.Options.SafeTimings)
            Bot.Wait.ForActionCooldown(GameActions.AcceptQuest);
        Bot.Flash.CallGameFunction("world.acceptQuest", id);
        if (Bot.Options.SafeTimings)
            Bot.Wait.ForQuestAccept(id);
        return IsInProgress(id);
    }

    public void Accept(params int[] ids)
    {
        for(int i = 0; i < ids.Length; i++)
        {
            Accept(ids[i]);
            Bot.Sleep(Bot.Options.ActionDelay);
        }
    }

    public bool EnsureAccept(int id)
    {
        for (int i = 0; i < Bot.Options.QuestAcceptAndCompleteTries; i++)
        {
            Accept(id);
            if (IsInProgress(id))
                break;
            Bot.Sleep(Bot.Options.ActionDelay);
        }
        return IsInProgress(id);
    }

    public void EnsureAccept(params int[] ids)
    {
        for (int i = 0; i < ids.Length; i++)
        {
            EnsureAccept(ids[i]);
            Bot.Sleep(Bot.Options.ActionDelay);
        }
    }

    public bool Complete(int id, int itemId = -1, bool special = false)
    {
        if (Bot.Options.SafeTimings)
            Bot.Wait.ForActionCooldown(GameActions.TryQuestComplete);
        if (Bot.Options.ExitCombatBeforeQuest && Bot.Player.InCombat)
            Bot.Map.Jump(Bot.Player.Cell, Bot.Player.Pad);
        Bot.Flash.CallGameFunction("world.tryQuestComplete", id, itemId, special);
        if (Bot.Options.SafeTimings)
            Bot.Wait.ForQuestComplete(id);
        return !IsInProgress(id);
    }

    public void Complete(params int[] ids)
    {
        for(int i = 0; i < ids.Length; i++)
        {
            Complete(ids[i]);
            Bot.Sleep(Bot.Options.ActionDelay);
        }
    }

    public bool EnsureComplete(int id, int itemId = -1, bool special = false)
    {
        if (Bot.Options.ExitCombatBeforeQuest)
            Bot.Combat.Exit();
        _EnsureComplete(id, itemId, special);
        return !IsInProgress(id);
    }

    private void _EnsureComplete(int id, int itemId = -1, bool special = false)
    {
        for (int i = 0; i < Bot.Options.QuestAcceptAndCompleteTries; i++)
        {
            Complete(id, itemId, special);
            if (IsInProgress(id))
                break;
            Bot.Sleep(Bot.Options.ActionDelay);
        }
    }

    public void EnsureComplete(params int[] ids)
    {
        for(int i = 0; i < ids.Length; i++)
        {
            EnsureComplete(ids[i]);
            Bot.Sleep(Bot.Options.ActionDelay);
        }
    }

    [MethodCallBinding("world.isQuestInProgress", GameFunction = true)]
    public bool IsInProgress(int id) => false;

    public bool UpdateQuest(int id)
    {
        Quest? quest = EnsureLoad(id);
        if(quest is null)
            return false;
        Bot.Send.ClientPacket("{\"t\":\"xt\",\"b\":{\"r\":-1,\"o\":{\"cmd\":\"updateQuest\",\"iValue\":" + quest.Value + ",\"iIndex\":" + quest.Slot + "}}}", "json");
        return true;
    }

    public void UpdateQuest(int value, int slot)
    {
        Bot.Send.ClientPacket("{\"t\":\"xt\",\"b\":{\"r\":-1,\"o\":{\"cmd\":\"updateQuest\",\"iValue\":" + value + ",\"iIndex\":" + slot + "}}}", "json");
    }

    public bool CanComplete(int id)
    {
        return Completed.Contains(q => q.ID == id);
    }

    public bool CanCompleteFullCheck(int id)
    {
        if (CanComplete(id))
            return true;

        Quest? quest = Tree.FirstOrDefault(q => q.ID == id);
        if (quest is null)
            return false;
        List<ItemBase> requirements = new();
        requirements.AddRange(quest.Requirements);
        requirements.AddRange(quest.AcceptRequirements);
        if (requirements.Count == 0)
            return true;
        foreach (ItemBase item in requirements)
        {
            if (Bot.Inventory.Contains(item.Name, item.Quantity))
                continue;
            return false;
        }
        return true;
    }

    public bool IsDailyComplete(int id)
    {
        Quest? quest = EnsureLoad(id);
        if (quest is null)
            return false;
        return Bot.Flash.CallGameFunction<int>("world.getAchievement", quest.Field, quest.Index) != 0;
    }

    public bool IsUnlocked(int id)
    {
        Quest? quest = EnsureLoad(id);
        if (quest is null)
            return false;
        return quest.Slot < 0 || Bot.Flash.CallGameFunction<int>("world.getQuestValue", quest.Slot) >= quest.Value - 1;
    }

    public bool HasBeenCompleted(int id)
    {
        Quest? quest = EnsureLoad(id);
        if (quest is null)
            return false;
        return quest.Slot < 0 || Bot.Flash.CallGameFunction<int>("world.getQuestValue", quest.Slot) >= quest.Value;
    }

    public bool IsAvailable(int id)
    {
        Quest? quest = EnsureLoad(id);
        return quest is not null
               && !IsDailyComplete(quest.ID)
               && IsUnlocked(quest.ID)
               && (!quest.Upgrade || Bot.Player.Upgrade)
               && Bot.Player.Level >= quest.Level
               && (quest.RequiredClassID <= 0 || Bot.Flash.CallGameFunction<int>("world.myAvatar.getCPByID", quest.RequiredClassID) >= quest.RequiredClassPoints)
               && (quest.RequiredFactionId <= 1 || Bot.Flash.CallGameFunction<int>("world.myAvatar.getRep", quest.RequiredFactionId) >= quest.RequiredFactionRep)
               && quest.AcceptRequirements.All(r => Bot.Inventory.Contains(r.Name, r.Quantity));
    }

    public void RegisterQuests(params int[] ids)
    {
        lock (_add)
            _add.AddRange(ids);
        if (!QuestThread?.IsAlive ?? true)
        {
            QuestThread = new(() =>
            {
                QuestsCTS = new();
                Poll(QuestsCTS.Token);
                QuestsCTS.Dispose();
                QuestsCTS = null;
            })
            {
                Name = "Quest Thread"
            };
            QuestThread.Start();
        }
    }

    public void UnregisterQuests(params int[] ids)
    {
        lock (_rem)
            _rem.AddRange(ids);
    }

    public void ClearRegisteredQuests()
    {
        lock (_rem)
            _rem.AddRange(Registered);
        if (QuestThread?.IsAlive ?? false)
        {
            QuestsCTS?.Cancel();
            Bot.Wait.ForTrue(() => QuestsCTS is null, 20);
        }
    }

    private void Poll(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (_add.Count > 0)
            {
                Registered.AddRange(_add.Except(Registered));
                lock (_add)
                    _add.Clear();
            }
            if (_rem.Count > 0)
            {
                Registered.RemoveAll(_rem.Contains);
                lock (_rem)
                    _rem.Clear();
            }
            if (Bot.Player.Playing)
            {
                _CompleteQuest(Registered);
            }
            if (!token.IsCancellationRequested)
                Thread.Sleep(RegisterCompleteInterval);
        }
    }

    private void _CompleteQuest(List<int> registered)
    {
        for (int i = 0; i < registered.Count; i++)
        {
            if (!CanComplete(registered[i]))
                continue;
            if (Bot.Options.SafeTimings)
                Bot.Wait.ForActionCooldown(GameActions.TryQuestComplete);
            Bot.Flash.CallGameFunction("world.tryQuestComplete", registered[i], -1, false);
            if (Bot.Options.SafeTimings)
                Bot.Wait.ForQuestComplete(registered[i]);
            Accept(registered[i]);
            Bot.Sleep(Bot.Options.ActionDelay);
        }
    }
}
