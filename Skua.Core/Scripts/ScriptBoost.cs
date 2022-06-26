using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

namespace Skua.Core.Scripts;
public class ScriptBoost : IScriptBoost
{
    private readonly Lazy<IScriptInventory> _lazyInventory;
    private readonly Lazy<IScriptBank> _lazyBank;
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
    public ScriptBoost(
        Lazy<IFlashUtil> flash,
        Lazy<IScriptSend> send,
        Lazy<IScriptMap> map,
        Lazy<IScriptInventory> inventory,
        Lazy<IScriptBank> bank,
        Lazy<IScriptWait> wait)
    {
        _lazyInventory = inventory;
        _lazyBank = bank;
        _lazySend = send;
        _lazyWait = wait;
        _lazyMap = map;
        _lazyFlash = flash;
    }

    private Thread? BoostsThread;
    private CancellationTokenSource? BoostsCTS;

    public event Action? BoostsStarted;
    public event Action? BoostsStopped;

    public bool Enabled => BoostsThread?.IsAlive ?? false;
    public bool UseClassBoost { get; set; } = false;
    public int ClassBoostID { get; set; }
    public bool UseExperienceBoost { get; set; } = false;
    public int ExperienceBoostID { get; set; }
    public bool UseGoldBoost { get; set; } = false;
    public int GoldBoostID { get; set; }
    public bool UseReputationBoost { get; set; } = false;
    public int ReputationBoostID { get; set; }

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

    private int SearchBoost(string name, bool searchBank = true)
    {
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
        if (BoostsThread?.IsAlive ?? false)
            return;

        BoostsStarted?.Invoke();
        BoostsThread = new(() =>
        {
            BoostsCTS = new();
            Poll(BoostsCTS.Token);
            BoostsCTS.Dispose();
            BoostsCTS = null;
        })
        {
            Name = "Boosts Thread"
        };
        BoostsThread.Start();
    }

    public void Stop()
    {
        BoostsStopped?.Invoke();
        BoostsCTS?.Cancel();
        Wait.ForTrue(() => !Enabled, 10);
    }

    private void Poll(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            _UseBoost(UseGoldBoost, GoldBoostID, BoostType.Gold);

            _UseBoost(UseClassBoost, ClassBoostID, BoostType.Class);

            _UseBoost(UseExperienceBoost, ExperienceBoostID, BoostType.Experience);

            _UseBoost(UseReputationBoost, ReputationBoostID, BoostType.Reputation);

            if(!token.IsCancellationRequested)
                Thread.Sleep(5000);
        }
    }

    private void _UseBoost(bool useBoost, int id, BoostType boostType)
    {
        if (!useBoost || id == 0 || IsBoostActive(boostType))
            return;

        UseBoost(id);
        Thread.Sleep(1000);
    }

    private readonly Dictionary<BoostType, string> _boostMap = new()
    {
        { BoostType.Gold, "iBoostG" },
        { BoostType.Class, "iBoostCP" },
        { BoostType.Reputation, "iBoostRep" },
        { BoostType.Experience, "iBoostXP" }
    };
}
