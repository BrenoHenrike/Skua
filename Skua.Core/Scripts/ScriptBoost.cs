using CommunityToolkit.Mvvm.ComponentModel;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

namespace Skua.Core.Scripts;
public partial class ScriptBoost : ObservableObject, IScriptBoost, IAsyncDisposable
{
    public ScriptBoost(
        Lazy<IFlashUtil> flash,
        Lazy<IScriptSend> send,
        Lazy<IScriptMap> map,
        Lazy<IScriptInventory> inventory,
        Lazy<IScriptBank> bank,
        Lazy<IScriptPlayer> player,
        Lazy<IScriptWait> wait)
    {
        _lazyInventory = inventory;
        _lazyBank = bank;
        _lazyPlayer = player;
        _lazySend = send;
        _lazyWait = wait;
        _lazyMap = map;
        _lazyFlash = flash;
        _timerBoosts = new PeriodicTimer(TimeSpan.FromSeconds(30));
    }

    private readonly Lazy<IScriptInventory> _lazyInventory;
    private readonly Lazy<IScriptBank> _lazyBank;
    private readonly Lazy<IScriptPlayer> _lazyPlayer;
    private readonly Lazy<IScriptSend> _lazySend;
    private readonly Lazy<IScriptWait> _lazyWait;
    private readonly Lazy<IScriptMap> _lazyMap;
    private readonly Lazy<IFlashUtil> _lazyFlash;
    private IScriptInventory Inventory => _lazyInventory.Value;
    private IScriptBank Bank => _lazyBank.Value;
    private IScriptSend Send => _lazySend.Value;
    private IScriptWait Wait => _lazyWait.Value;
    private IScriptMap Map => _lazyMap.Value;
    private IFlashUtil Flash => _lazyFlash.Value;

    private readonly PeriodicTimer _timerBoosts;
    private Task? _taskBoosts;
    private CancellationTokenSource? _ctsBoosts;

    public bool Enabled => _taskBoosts is not null;
    /// <inheritdoc cref="IScriptBoost.UseClassBoost"/>
    [ObservableProperty]
    private bool _useClassBoost = false;
    /// <inheritdoc cref="IScriptBoost.ClassBoostID"/>
    [ObservableProperty]
    private int _classBoostID;
    /// <inheritdoc cref="IScriptBoost.UseExperienceBoost"/>
    [ObservableProperty]
    private bool _useExperienceBoost = false;
    /// <inheritdoc cref="IScriptBoost.ExperienceBoostID"/>
    [ObservableProperty]
    private int _experienceBoostID;
    /// <inheritdoc cref="IScriptBoost.UseGoldBoost"/>
    [ObservableProperty]
    private bool _useGoldBoost = false;
    /// <inheritdoc cref="IScriptBoost.GoldBoostID"/>
    [ObservableProperty]
    private int _goldBoostID;
    /// <inheritdoc cref="IScriptBoost.UseReputationBoost"/>
    [ObservableProperty]
    private bool _useReputationBoost = false;
    /// <inheritdoc cref="IScriptBoost.ReputationBoostID"/>
    [ObservableProperty]
    private int _reputationBoostID;

    public bool IsBoostActive(BoostType boost)
    {
        return Flash.GetGameObject($"world.myAvatar.objData.{_boostMap[boost]}", 0) > 0;
    }

    public void UseBoost(int id)
    {
        Send.Packet($"%xt%zm%serverUseItem%{Map.RoomID}%+%{id}%");
    }

    public int GetBoostID(BoostType boostType, bool searchBank = true)
    {
        return boostType switch
        {
            BoostType.Gold => SearchBoost("gold", searchBank),
            BoostType.Class => SearchBoost("class", searchBank),
            BoostType.Reputation => SearchBoost("rep", searchBank),
            BoostType.Experience => SearchBoost("xp", searchBank),
            _ => 0,
        };
    }

    private int SearchBoost(string name, bool searchBank = false)
    {
        if (!_lazyPlayer.Value.LoggedIn)
            return 0;
        int id = (Inventory.Items?
                   .Where(i => i.Category == ItemCategory.ServerUse)
                   .Where(i => i.Name.Contains(name))
                   .FirstOrDefault())?.ID ?? 0;
        if (id == 0 && searchBank)
        {
            if(!Bank.Loaded)
                Bank.Load();
            id = (Bank.Items?
                   .Where(i => i.Category == ItemCategory.ServerUse)
                   .Where(i => i.Name.Contains(name))
                   .FirstOrDefault())?.ID ?? 0;
            Bank.EnsureToInventory(id, false);
        }
        return id;
    }

    public void Start()
    {
        if (_taskBoosts is not null)
            return;

        _ctsBoosts = new();
        _taskBoosts = HandleBoosts(_timerBoosts, _ctsBoosts.Token);
        OnPropertyChanged(nameof(Enabled));
    }

    public void Stop()
    {
        if (_taskBoosts is null)
            return;

        _ctsBoosts?.Cancel();
        Wait.ForTrue(() => _taskBoosts?.IsCompleted == true, null, 20);
        _ctsBoosts?.Dispose();
        _taskBoosts = null;
        OnPropertyChanged(nameof(Enabled));
    }

    public async ValueTask StopAsync()
    {
        if (_taskBoosts is null)
            return;

        _ctsBoosts?.Cancel();
        await Wait.ForTrueAsync(() => _taskBoosts?.IsCompleted == true, 20);
        _ctsBoosts?.Dispose();
        _ctsBoosts = null;
        _taskBoosts = null;
        OnPropertyChanged(nameof(Enabled));
    }

    private async Task HandleBoosts(PeriodicTimer timer, CancellationToken token)
    {
        try
        {
            await PollBoosts(token);

            while (await timer.WaitForNextTickAsync(token))
                await PollBoosts(token);
        }
        catch { }
    }

    private async Task PollBoosts(CancellationToken token)
    {
        await _UseBoost(UseGoldBoost, GoldBoostID, BoostType.Gold, token);

        await _UseBoost(UseClassBoost, ClassBoostID, BoostType.Class, token);

        await _UseBoost(UseExperienceBoost, ExperienceBoostID, BoostType.Experience, token);

        await _UseBoost(UseReputationBoost, ReputationBoostID, BoostType.Reputation, token);
    }

    private async ValueTask _UseBoost(bool useBoost, int id, BoostType boostType, CancellationToken token)
    {
        if (!useBoost || id == 0 || IsBoostActive(boostType))
            return;

        UseBoost(id);
        await Task.Delay(1000, token);
    }

    public async ValueTask DisposeAsync()
    {
        if(_taskBoosts is not null)
        {
            _ctsBoosts?.Cancel();
            await _taskBoosts;
            _ctsBoosts?.Dispose();
        }
        _timerBoosts.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly Dictionary<BoostType, string> _boostMap = new()
    {
        { BoostType.Gold, "iBoostG" },
        { BoostType.Class, "iBoostCP" },
        { BoostType.Reputation, "iBoostRep" },
        { BoostType.Experience, "iBoostXP" }
    };
}
