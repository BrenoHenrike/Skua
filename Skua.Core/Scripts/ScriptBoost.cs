using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

namespace Skua.Core.Scripts;
public class ScriptBoost : ScriptableObject, IScriptBoost
{
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
        return Bot.Flash.GetGameObject($"world.myAvatar.objData.{_boostMap[boost]}", 0) > 0;
    }

    public void UseBoost(int id)
    {
        Bot.Send.Packet($"%xt%zm%serverUseItem%{Bot.Map.RoomID}%+%{id}%");
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
        int id = (Bot.Inventory.Items?
                   .Where(i => i.Category == ItemCategory.ServerUse)
                   .Where(i => i.Name.Contains(name))
                   .FirstOrDefault())?.ID ?? 0;
        if (id == 0 && searchBank)
        {
            if(!Bot.Bank.Loaded)
                Bot.Bank.Load();
            id = (Bot.Bank.Items?
                   .Where(i => i.Category == ItemCategory.ServerUse)
                   .Where(i => i.Name.Contains(name))
                   .FirstOrDefault())?.ID ?? 0;
            Bot.Bank.EnsureToInventory(id, false);
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
        Bot.Wait.ForTrue(() => !Enabled, 10);
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
