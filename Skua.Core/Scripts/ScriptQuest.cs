using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Skua.Core.Flash;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;
using Skua.Core.Utils;

namespace Skua.Core.Scripts;

public partial class ScriptQuest : ObservableRecipient, IScriptQuest
{
    public ScriptQuest(
        Lazy<IFlashUtil> flash,
        Lazy<IScriptWait> wait,
        Lazy<IScriptOption> options,
        Lazy<IScriptPlayer> player,
        Lazy<IScriptSend> send,
        Lazy<IScriptInventory> inventory,
        Lazy<IScriptTempInv> tempInv,
        Lazy<IScriptInventoryHelper> invHelper)
    {
        _lazyFlash = flash;
        _lazyWait = wait;
        _lazyOptions = options;
        _lazyPlayer = player;
        _lazySend = send;
        _lazyInventory = inventory;
        _lazyInvHelper = invHelper;
    }

    private readonly Lazy<IFlashUtil> _lazyFlash;
    private readonly Lazy<IScriptWait> _lazyWait;
    private readonly Lazy<IScriptOption> _lazyOptions;
    private readonly Lazy<IScriptPlayer> _lazyPlayer;
    private readonly Lazy<IScriptSend> _lazySend;
    private readonly Lazy<IScriptInventory> _lazyInventory;
    private readonly Lazy<IScriptInventoryHelper> _lazyInvHelper;
    private IFlashUtil Flash => _lazyFlash.Value;
    private IScriptWait Wait => _lazyWait.Value;
    private IScriptOption Options => _lazyOptions.Value;
    private IScriptPlayer Player => _lazyPlayer.Value;
    private IScriptSend Send => _lazySend.Value;
    private IScriptInventory Inventory => _lazyInventory.Value;
    private IScriptInventoryHelper InvHelper => _lazyInvHelper.Value;

    private Thread? _questThread;
    private CancellationTokenSource? _questsCTS;

    public int RegisterCompleteInterval { get; set; } = 2000;

    [ObjectBinding("world.questTree", Default = "new()")]
    private Dictionary<int, Quest> _quests = new();

    public List<Quest> Tree => Quests.Values.ToList() ?? new();
    public List<Quest> Active => Tree.FindAll(x => x.Active);
    public List<Quest> Completed => Tree.FindAll(x => x.Status == "c");
    public List<QuestData> Cached { get; set; } = new();
    public Dictionary<int, QuestData> CachedDictionary { get; set; } = new();
    private SynchronizedList<int> _registered = new();
    public IEnumerable<int> Registered => _registered.Items;

    public void Load(params int[] ids)
    {
        if (ids.Length < 30)
        {
            Flash.CallGameFunction("world.showQuests", ids.Select(id => id.ToString()).Join(','), "q");
            return;
        }

        foreach (int[] idchunk in ids.Chunk(30))
        {
            Flash.CallGameFunction("world.showQuests", idchunk.Select(id => id.ToString()).Join(','), "q");
            Thread.Sleep(Options.ActionDelay);
        }
    }

    public Quest? EnsureLoad(int id)
    {
        Wait.ForTrue(() => Tree.Contains(x => x.ID == id), () => Load(id), 20, 1000);
        return Tree.Find(q => q.ID == id)!;
    }

    public bool TryGetQuest(int id, out Quest? quest)
    {
        return (quest = Tree.Find(x => x.ID == id)) != null;
    }

    public bool Accept(int id)
    {
        if (Options.SafeTimings)
            Wait.ForActionCooldown(GameActions.AcceptQuest);
        Flash.CallGameFunction("world.acceptQuest", id);
        if (Options.SafeTimings)
            Wait.ForQuestAccept(id);
        return IsInProgress(id);
    }

    public void Accept(params int[] ids)
    {
        for (int i = 0; i < ids.Length; i++)
        {
            Accept(ids[i]);
            Thread.Sleep(Options.ActionDelay);
        }
    }

    public bool EnsureAccept(int id)
    {
        for (int i = 0; i < Options.QuestAcceptAndCompleteTries; i++)
        {
            Accept(id);
            if (IsInProgress(id))
                break;
            Thread.Sleep(Options.ActionDelay);
        }
        return IsInProgress(id);
    }

    public void EnsureAccept(params int[] ids)
    {
        for (int i = 0; i < ids.Length; i++)
        {
            EnsureAccept(ids[i]);
            Thread.Sleep(Options.ActionDelay);
        }
    }

    public bool Complete(int id, int itemId = -1, bool special = false)
    {
        if (Options.SafeTimings)
            Wait.ForActionCooldown(GameActions.TryQuestComplete);
        Flash.CallGameFunction("world.tryQuestComplete", id, itemId, special);
        if (Options.SafeTimings)
            Wait.ForQuestComplete(id);
        return !IsInProgress(id);
    }

    public void Complete(params int[] ids)
    {
        for (int i = 0; i < ids.Length; i++)
        {
            Complete(ids[i]);
            Thread.Sleep(Options.ActionDelay);
        }
    }

    public bool EnsureComplete(int id, int itemId = -1, bool special = false)
    {
        _EnsureComplete(id, itemId, special);
        return !IsInProgress(id);
    }

    private void _EnsureComplete(int id, int itemId = -1, bool special = false)
    {
        if (id == 0)
            return;
        for (int i = 0; i < Options.QuestAcceptAndCompleteTries; i++)
        {
            Complete(id, itemId, special);
            if (!IsInProgress(id))
                break;
            Thread.Sleep(Options.ActionDelay);
        }
    }

    public void EnsureComplete(params int[] ids)
    {
        for (int i = 0; i < ids.Length; i++)
        {
            EnsureComplete(ids[i]);
            Thread.Sleep(Options.ActionDelay);
        }
    }

    [MethodCallBinding("world.isQuestInProgress", GameFunction = true)]
    private bool _isInProgress(int id) => false;

    public bool UpdateQuest(int id)
    {
        Quest? quest = EnsureLoad(id);
        if (quest is null)
            return false;
        Send.ClientPacket("{\"t\":\"xt\",\"b\":{\"r\":-1,\"o\":{\"cmd\":\"updateQuest\",\"iValue\":" + quest.Value + ",\"iIndex\":" + quest.Slot + "}}}", "json");
        return true;
    }

    public void UpdateQuest(int value, int slot)
    {
        Send.ClientPacket("{\"t\":\"xt\",\"b\":{\"r\":-1,\"o\":{\"cmd\":\"updateQuest\",\"iValue\":" + value + ",\"iIndex\":" + slot + "}}}", "json");
    }

    public bool CanComplete(int id)
    {
        return Completed.Contains(q => q.ID == id);
    }

    public bool CanCompleteFullCheck(int id)
    {
        if (CanComplete(id))
            return true;

        _quests.TryGetValue(id, out Quest? quest);
        if (quest is null)
            return false;
        List<ItemBase> requirements = new();
        requirements.AddRange(quest.Requirements);
        requirements.AddRange(quest.AcceptRequirements);
        if (requirements.Count == 0)
            return true;
        foreach (ItemBase item in requirements)
        {
            if (InvHelper.Check(item.ID, item.Quantity, false))
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
        return IsDailyComplete(quest);
    }

    public bool IsDailyComplete(Quest quest)
    {
        return Flash.CallGameFunction<int>("world.getAchievement", quest.Field, quest.Index) > 0;
    }

    public bool IsUnlocked(int id)
    {
        Quest? quest = EnsureLoad(id);
        if (quest is null)
            return false;
        return IsUnlocked(quest);
    }

    public bool IsUnlocked(Quest quest)
    {
        return quest.Slot < 0 || Flash.CallGameFunction<int>("world.getQuestValue", quest.Slot) >= quest.Value - 1;
    }

    public bool HasBeenCompleted(int id)
    {
        Quest? quest = EnsureLoad(id);
        if (quest is null)
            return false;
        return HasBeenCompleted(quest);
    }

    public bool HasBeenCompleted(Quest quest)
    {
        return quest.Slot < 0 || Flash.CallGameFunction<int>("world.getQuestValue", quest.Slot) >= quest.Value;
    }

    public bool IsAvailable(int id)
    {
        Quest? quest = EnsureLoad(id);
        return quest is not null
               && !IsDailyComplete(quest)
               && IsUnlocked(quest)
               && (!quest.Upgrade || Player.IsMember)
               && Player.Level >= quest.Level
               && (quest.RequiredClassID <= 0 || Flash.CallGameFunction<int>("world.myAvatar.getCPByID", quest.RequiredClassID) >= quest.RequiredClassPoints)
               && (quest.RequiredFactionId <= 1 || Flash.CallGameFunction<int>("world.myAvatar.getRep", quest.RequiredFactionId) >= quest.RequiredFactionRep)
               && quest.AcceptRequirements.All(r => Inventory.Contains(r.Name, r.Quantity));
    }

    public void RegisterQuests(params int[] ids)
    {
        if (ids.Length == 0)
            return;
        _registered.AddRange(ids.Except(_registered.Items));
        OnPropertyChanged(nameof(Registered));
        Broadcast(null, Registered, nameof(Registered));
        if (!_questThread?.IsAlive ?? true)
        {
            _questThread = new(async () =>
            {
                _questsCTS = new();
                try
                {
                    await _Poll(_questsCTS.Token);
                }
                catch { }
                _questsCTS?.Dispose();
                _questsCTS = null;
            })
            {
                Name = "Quest Thread"
            };
            _questThread.Start();
        }
    }

    public void UnregisterQuests(params int[] ids)
    {
        _registered.Remove(ids.Contains);
        OnPropertyChanged(nameof(Registered));
        Broadcast(null, Registered, nameof(Registered));
    }

    public void UnregisterAllQuests()
    {
        _registered.Clear();
        OnPropertyChanged(nameof(Registered));
        Broadcast(null, Registered, nameof(Registered));
        if (_questThread?.IsAlive ?? false)
        {
            _questsCTS?.Cancel();
            Wait.ForTrue(() => _questsCTS is null, 20);
        }
    }

    private async Task _Poll(CancellationToken token)
    {
        _lastComplete = Environment.TickCount;
        while (!token.IsCancellationRequested)
        {
            if (Player.Playing)
                await _CompleteQuest(_registered.Items, token).ConfigureAwait(false);
            await Task.Delay(RegisterCompleteInterval, token);
        }
    }

    private int _lastComplete;

    private async Task _CompleteQuest(IEnumerable<int> registered, CancellationToken token)
    {
        foreach (int quest in registered)
        {
            if (!IsInProgress(quest))
                Accept(quest);
            await Task.Delay(Options.ActionDelay, token);
            if (!CanComplete(quest))
            {
                if (Environment.TickCount - _lastComplete > 10000 && CanCompleteFullCheck(quest))
                {
                    EnsureAccept(quest);
                    _lastComplete = Environment.TickCount;
                }
                continue;
            }
            if (Options.SafeTimings)
                Wait.ForActionCooldown(GameActions.TryQuestComplete);
            Flash.CallGameFunction("world.tryQuestComplete", quest, -1, false);
            if (Options.SafeTimings)
                Wait.ForQuestComplete(quest);
            EnsureAccept(quest);
            _lastComplete = Environment.TickCount;
            await Task.Delay(Options.ActionDelay, token);
        }
    }

    public void LoadCachedQuests()
    {
        if (Cached.Count > 0)
            return;

        var skuaQuestFile = File.ReadAllText(ClientFileSources.SkuaQuestsFile);
        Cached = JsonConvert.DeserializeObject<List<QuestData>>(skuaQuestFile) ?? new();
        CachedDictionary = Cached.ToDictionary(x => x.ID, x => x);
    }

    public List<QuestData> GetCachedQuests(int start, int count)
    {
        if (Cached.Count == 0)
            LoadCachedQuests();

        return Cached.Skip(start).Take(count).ToList();
    }

    public List<QuestData> GetCachedQuests(params int[] ids)
    {
        if (Cached.Count == 0)
            LoadCachedQuests();

        List<QuestData> quests = new();
        foreach (int id in ids)
        {
            if (CachedDictionary.TryGetValue(id, out QuestData? value))
                quests.Add(value);
        }
        return quests;
    }
}