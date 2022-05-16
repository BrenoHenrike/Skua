using Newtonsoft.Json;
using System.Diagnostics;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Utils;

namespace Skua.Core.Scripts;
public class ScriptInterface : IScriptInterface
{
    private CancellationTokenSource? ScriptInterfaceCTS;
    private readonly Thread ScriptInterfaceThread;
    private bool shouldExit;

    public bool ShouldExit => shouldExit;
    public bool IsWorldLoaded => !Flash.IsNull("world");

    public IScriptManager ScriptManager { get; }
    public IFlashUtil Flash { get; }
    public IScriptBoost Boosts { get; }
    public IScriptBotStats Stats { get; }
    public IScriptCombat Combat { get; }
    public IScriptDrop Drops { get; }
    public IScriptEvent Events { get; }
    public IScriptFaction Reputation { get; }
    public IScriptHouseInv House { get; }
    public IScriptInventory Inventory { get; }
    public IScriptTempInv TempInv { get; }
    public IScriptBank Bank { get; }
    public IScriptLite Lite { get; }
    public IScriptOption Options { get; }
    public IScriptMap Map { get; }
    public IScriptMonster Monsters { get; }
    public IScriptPlayer Player { get; }
    public IScriptQuest Quests { get; }
    public IScriptSend Send { get; }
    public IScriptShop Shops { get; }
    public IScriptSkill Skills { get; }
    public IScriptWait Wait { get; }
    public IScriptServers Servers { get; }
    public IScriptHandlers Handlers { get; }

    public ScriptInterface(IScriptManager manager, IFlashUtil flash, IScriptHandlers handlers, IScriptServers server, IScriptBoost boosts, IScriptBotStats stats, IScriptCombat combat, IScriptDrop drops, IScriptEvent events, IScriptFaction rep, IScriptHouseInv house, IScriptInventory inventory, IScriptTempInv tempInv, IScriptBank bank, IScriptLite lite, IScriptOption options, IScriptMap map, IScriptMonster monsters, IScriptPlayer player, IScriptQuest quests, IScriptSend send, IScriptShop shops, IScriptSkill skills, IScriptWait wait)
    {
        ScriptManager = manager;
        Boosts = boosts;
        Stats = stats;
        Combat = combat;
        Drops = drops;
        Events = events;
        Reputation = rep;
        House = house;
        Inventory = inventory;
        TempInv = tempInv;
        Bank = bank;
        Lite = lite;
        Options = options;
        Map = map;
        Monsters = monsters;
        Player = player;
        Quests = quests;
        Send = send;
        Shops = shops;
        Skills = skills;
        Wait = wait;
        Servers = server;
        Handlers = handlers;
        Flash = flash;

        Flash.FlashCall += HandleFlashCall;

        Schedule(0, async b => await b.Servers.GetServers());

        ScriptInterfaceThread = new(() =>
        {
            ScriptInterfaceCTS = new();
            ScriptTimer(ScriptInterfaceCTS.Token);
            ScriptInterfaceCTS.Dispose();
            ScriptInterfaceCTS = null;
        })
        {
            Name = "ScriptInterface"
        };
    }

    public Task Schedule(int delay, Func<IScriptInterface, Task> function)
    {
        return Task.Run(async () => { await Task.Delay(delay); await function(this); });
    }

    public Task Schedule(int delay, Action<IScriptInterface> action)
    {
        return Task.Run(async () => { await Task.Delay(delay); action(this); });
    }

    public void Log(string message)
    {
        // TODO Logger
    }

    public void Sleep(int ms)
    {
        Thread.Sleep(ms);
    }

    public void Initialize()
    {
        if (!ScriptInterfaceThread.IsAlive)
            ScriptInterfaceThread.Start();
    }

    private const int _timerDelay = 20;
    private TimeLimiter _limit = new();
    private void ScriptTimer(CancellationToken token)
    {
        bool catching = false;
        int lastConnChange = 0;
        string lastConnDetail = "";

        Stopwatch sw = new();

        while (!token.IsCancellationRequested)
        {
            try
            {
                sw.Restart();

                if (IsWorldLoaded && Player.Playing)
                {
                    Servers.LastIP = Player.ServerIP ?? Servers.LastIP;

                    if (Options.RestPackets && !Player.InCombat && (Player.Health < Player.MaxHealth || Player.Mana < Player.MaxMana))
                        _limit.LimitedRun("rest", 1000, () => Send.Packet("%xt%zm%restRequest%1%%"));

                    if (!catching)
                    {
                        Flash.Call("catchPackets");
                        catching = true;
                    }

                    _limit.LimitedRun("opts", 300, () =>
                    {
                        if (Options.Magnetise)
                            Flash.Call("magnetise");
                        if (Options.InfiniteRange)
                            Flash.Call("infiniteRange");
                        if (Options.AggroMonsters)
                            Flash.CallGameFunction("world.aggroAllMon");
                        if (Options.AggroAllMonsters)
                            Send.Packet($"%xt%zm%aggroMon%{Map.RoomID}%{string.Join("%", Monsters.MapMonsters.Select(m => m.MapID))}%");
                        if (Options.SkipCutscenes)
                            Flash.Call("skipCutscenes");
                        if (Options.LagKiller)
                            Flash.Call("killLag", true);
                        Player.WalkSpeed = Options.WalkSpeed;
                    });
                }

                _limit.LimitedRun("connDetail", 100, () => (lastConnChange, lastConnDetail) = CheckStuckonLoading(lastConnChange, lastConnDetail));

                if (ScriptManager.ScriptRunning)
                    RunScriptHandlers();

                sw.Stop();
                Thread.Sleep(Math.Max(10, _timerDelay - (int)sw.Elapsed.TotalMilliseconds));
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error in timer thread: {e.Message}");
            }
        }
    }

    /// <summary>
    /// Checks if the player is stuck in the loading screen.
    /// </summary>
    /// <param name="lastConnChange">Last time the loading message changed.</param>
    /// <param name="lastConnDetail">Last loading message.</param>
    /// <returns>The last loading message and its time</returns>
    private (int newTime, string newText) CheckStuckonLoading(int lastConnChange, string lastConnDetail)
    {
        string connDetail = Flash.IsNull("mcConnDetail.stage") ? "null" : Flash.GetGameObject("mcConnDetail.txtDetail.text", "null")!;
        if (connDetail == "null")
            return (Environment.TickCount, connDetail);
        if (Environment.TickCount - lastConnChange >= Options.LoadTimeout && connDetail == lastConnDetail)
        {
            if (connDetail.Contains("loading map") && !_waitForLogin && string.IsNullOrEmpty(Map.LastMap))
            {
                Map.Join("battleon");
                Map.Reload();
                Handlers.RegisterOnce(500, b =>
                {
                if (Flash.GetGameObject("mcConnDetail.txtDetail.text") == "loading map")
                    {
                        Logout();
                        return;
                    }
                    Map.Join(Map.LastMap);
                });
            }
            else
            {
                Logout();
            }
        }
        return (Environment.TickCount, connDetail);

        void Logout()
        {
            _waitForLogin = false;
            _reloginCTS?.Cancel();
            Servers.Logout();
        }
    }

    /// <summary>
    /// Run all registered handlers, if the handler returns <see langword="false"/> it is removed from the list.
    /// </summary>
    private void RunScriptHandlers()
    {
        if (Handlers.CurrentHandlers.Count <= 0)
            return;
        List<IHandler> rem = new();
        foreach (IHandler handler in Handlers.CurrentHandlers)
        {
            _limit.LimitedRun("handler_" + handler.Name, handler.Ticks * _timerDelay, () =>
            {
                if (!handler.Function(this))
                    rem.Add(handler);
            });
        }
        Handlers.Remove(rem);
    }

    private void HandleFlashCall(string name, object[] args)
    {
        switch (name)
        {
            case "requestLoadGame":
                Flash.Call("loadClient", Array.Empty<object>());
                break;
            case "debug":
                Debug.WriteLine(args[0]);
                break;
            case "pext":
                dynamic packet = JsonConvert.DeserializeObject<dynamic>((string)args[0])!;
                string type = packet["params"].type;
                dynamic data = packet["params"].dataObj;
                if (type is not null and "json")
                {
                    string cmd = data.cmd;
                    switch (cmd)
                    {
                        case "event":
                            string zone = data.args?["zoneSet"]!;
                            if (zone is not null)
                                Events.OnRunToArea(zone);
                            break;
                        case "moveToArea":
                            if (Options.CustomName != null)
                                Options.CustomName = Options.CustomName;
                            if (Options.CustomGuild != null)
                                Options.CustomGuild = Options.CustomGuild;
                            Events.OnMapChanged(Convert.ToString(data.strMapName));
                            Map.FilePath = Convert.ToString(data.strMapFileName);
                            Map.LastMap = Convert.ToString(data.strMapName);
                            break;
                        case "ct":
                            dynamic p = data.p?[Player.Username.ToLower()]!;
                            if (p is not null && p.intHP == 0)
                            {
                                Stats.Deaths++;
                                Events.OnPlayerDeath();
                                break;
                            }
                            dynamic anims = data.anims?[0]!;
                            if (anims is not null)
                            {
                                string msg = anims["msg"];
                                if (msg is not null && msg.Contains("prepares a counter attack!"))
                                {
                                    Events.OnCounterAttack(false);
                                    break;
                                }
                            }
                            if (data.a is not null)
                            {
                                for (int i = 0; i < data.a?.Count ?? 5; i++)
                                {
                                    dynamic a = data.a?[i]!;
                                    if (a is null)
                                        continue;
                                    if (a.aura is not null && (string)a.aura["nam"] == "Counter Attack")
                                    {
                                        Events.OnCounterAttack(true);
                                        break;
                                    }
                                }
                            }
                            break;
                        case "sellItem":
                            Wait.ItemSellEvent.Set();
                            break;
                        case "buyItem":
                            if (data.bitSuccess == 1)
                                Wait.ItemBuyEvent.Set();
                            break;
                        case "dropItem":
                            string items = Convert.ToString(data["items"]);
                            InventoryItem drop = JsonConvert.DeserializeObject<Dictionary<string, InventoryItem>>(items)!.First().Value;
                            Events.OnItemDropped(drop);
                            if (Drops.CurrentDropInfos.All(d => d.ID != drop.ID))
                                Drops.CurrentDropInfos.Add(drop);
                            break;
                        case "addItems":
                            string addItems = Convert.ToString(data["items"]);
                            Dictionary<int, dynamic> obj = JsonConvert.DeserializeObject<Dictionary<int, dynamic>>(addItems)!;
                            ItemBase invItem = Inventory.GetItem(obj.Keys.First())!;
                            if (invItem is null)
                                invItem = TempInv.GetItem(obj.Keys.First())!;
                            if (!invItem.Temp)
                                Stats.Drops++;
                            Events.OnItemDropped(invItem, true, Convert.ToInt32(obj.Values.First().iQtyNow));
                            break;
                        case "getDrop":
                            // TODO check getDrop packet
                            if (data.bSuccess == 1)
                                Stats.Drops += (int)data.iQty;
                            break;
                        case "addGoldExp":
                            // TODO check addGoldExp packet
                            string aa = Convert.ToString(packet);
                            Debug.WriteLine(aa);
                            if (data.typ == "m")
                            {
                                Stats.Kills++;
                                Events.OnMonsterKilled();
                            }
                            break;
                        case "ccqr":
                            if (data.bSuccess == 1)
                            {
                                Stats.QuestsCompleted++;
                                Events.OnQuestTurnIn(Convert.ToInt32(data.QuestID));
                            }
                            break;
                        case "loadBank":
                            Wait.BankLoadEvent.Set();
                            break;
                        case "loadShop":
                            Shops.OnLoaded(Shops.ID, Shops.Name, Shops.Items);
                            break;
                    }
                }
                else if (type is not null and "str")
                {
                    string cmd = data[0];
                    switch (cmd)
                    {
                        case "uotls":
                            if (Player.Username == (string)data[2] && data[3] == "afk:true")
                                Events.OnPlayerAFK();
                            break;
                    }
                }
                Events.OnExtensionPacket(packet);
                break;
            case "packet":
                string[] parts = ((string)args[0]).Split('%', StringSplitOptions.RemoveEmptyEntries);
                switch (parts[2])
                {
                    case "moveToCell":
                        Events.OnCellChanged(Map.Name, parts[4], parts[5]);
                        break;
                    case "buyItem":
                        Events.OnTryBuyItem(int.Parse(parts[5]), int.Parse(parts[4]), int.Parse(parts[6]));
                        break;
                    case "acceptQuest":
                        Stats.QuestsAccepted++;
                        Events.OnQuestAccepted(int.Parse(parts[4]));
                        break;
                    case "cmd":
                        if (parts.Length >= 5 && parts[4] == "logout")
                            OnLogout();
                        break;
                }
                break;
        }
    }

    private Task? _reloginTask;
    private volatile bool _waitForLogin;
    private CancellationTokenSource? _reloginCTS;
    private void OnLogout()
    {
        Bank.Loaded = false;
        Drops.CurrentDropInfos.Clear();
        if (!Options.AutoRelogin || _waitForLogin)
            return;

        if (_reloginTask is not null)
        {
            Log("Relogin task already running.");
            _waitForLogin = true;
            return;
        }

        Log("Autorelogin triggered.");
        bool wasRunning = ScriptManager.ScriptRunning;
        ScriptManager.StopScript();
        bool kicked = Player.Kicked;
        _waitForLogin = true;
        Servers.Logout();
        Events.OnReloginTriggered(kicked);

        Relogin((!Options.SafeRelogin && !kicked) ? 5000 : 70000, wasRunning);

        void Relogin(int delay, bool startScript)
        {
            Log($"Waiting {delay}ms for relogin.");
            _reloginCTS = new CancellationTokenSource();
            _reloginTask = Schedule(delay, async _ =>
            {
                Stats.Relogins++;
                bool relogged = await Servers.EnsureRelogin(_reloginCTS.Token);
                if (startScript)
                    await ScriptManager.StartScriptAsync();
                Log($"Relogin was {(relogged ? "successful" : "cancelled or unsuccessful")}.");
                _reloginCTS.Dispose();
                _reloginCTS = null;
                _reloginTask = null;
                _waitForLogin = false;
            });
        }
    }
}
