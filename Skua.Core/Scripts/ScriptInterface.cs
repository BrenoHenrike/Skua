using Newtonsoft.Json;
using System.Diagnostics;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Utils;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Messaging;
using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.Models;
using Skua.Core.Interfaces.Auras;

namespace Skua.Core.Scripts;
public class ScriptInterface : IScriptInterface, IScriptInterfaceManager
{
    private CancellationTokenSource? ScriptInterfaceCTS;
    private readonly Thread ScriptInterfaceThread;
    private const int _timerDelay = 20;
    private readonly TimeLimiter _limit = new();
    private readonly ILogService _logger;
    private readonly IDialogService _dialogService;

    public bool ShouldExit => Manager.ShouldExit;
    public Version Version { get; }

    public IScriptStatus Manager { get; }
    public IFlashUtil Flash { get; }
    public IScriptAuto Auto { get; }
    public IMessenger Messenger { get; }
    public IScriptBoost Boosts { get; }
    public IScriptBotStats Stats { get; }

    public IScriptSelfAuras SelfAuras { get; }
    public IScriptTargetAuras TargetAuras { get; }

    public IScriptCombat Combat { get; }
    public IScriptKill Kill { get; }
    public IScriptHunt Hunt { get; }
    public IScriptDrop Drops { get; }
    public IScriptEvent Events { get; }
    public IScriptFaction Reputation { get; }
    public IScriptHouseInv House { get; }
    public IScriptInventory Inventory { get; }
    public IScriptTempInv TempInv { get; }
    public IScriptBank Bank { get; }
    public IScriptInventoryHelper InvHelper { get; }
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
    public ICaptureProxy GameProxy { get; }
    public IScriptOptionContainer? Config => Manager.Config;
    public Random Random { get; set; } = new Random();

    public ScriptInterface(
        ILogService logger,
        IScriptManager manager,
        IFlashUtil flash,
        IScriptHandlers handlers,
        IScriptServers server,
        IScriptBoost boosts,
        IScriptBotStats stats,
        IScriptSelfAuras scriptSelfAuras,
        IScriptTargetAuras scriptTargetAuras,
        IScriptCombat combat,
        IScriptDrop drops,
        IScriptEvent events,
        IScriptFaction rep,
        IScriptHouseInv house,
        IScriptInventory inventory,
        IScriptTempInv tempInv,
        IScriptBank bank,
        IScriptInventoryHelper invManager,
        IScriptLite lite,
        IScriptOption options,
        IScriptMap map,
        IScriptMonster monsters,
        IScriptPlayer player,
        IScriptQuest quests,
        IScriptSend send,
        IScriptShop shops,
        IScriptSkill skills,
        IScriptWait wait,
        IScriptKill kill,
        IScriptHunt hunt,
        ICaptureProxy gameProxy,
        IScriptAuto auto,
        IDialogService dialogService,
        ISettingsService settingsService)
    {
        _logger = logger;
        Manager = manager;
        Boosts = boosts;
        Stats = stats;
        SelfAuras = scriptSelfAuras;
        TargetAuras = scriptTargetAuras;
        Combat = combat;
        Kill = kill;
        Hunt = hunt;
        GameProxy = gameProxy;
        Auto = auto;
        Messenger = StrongReferenceMessenger.Default;
        _dialogService = dialogService;
        Drops = drops;
        Events = events;
        Reputation = rep;
        House = house;
        Inventory = inventory;
        TempInv = tempInv;
        Bank = bank;
        InvHelper = invManager;
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

        Version = Version.Parse(settingsService.Get("ApplicationVersion", "0.0.0.0"));

        Flash.FlashCall += HandleFlashCall;

        ScriptInterfaceThread = new(() =>
        {
            ScriptInterfaceCTS = new();
            ScriptTimer(ScriptInterfaceCTS.Token);
            ScriptInterfaceCTS?.Dispose();
            ScriptInterfaceCTS = null;
        })
        {
            Name = "ScriptInterface",
            IsBackground = true
        };

        IScriptInterface.Instance = this;
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
        CheckScriptTermination();
        _logger.ScriptLog(message);
    }

    public void Sleep(int ms)
    {
        CheckScriptTermination();
        Thread.Sleep(ms);
    }

    private void CheckScriptTermination()
    {
        if (Manager.ShouldExit && Thread.CurrentThread.Name == "Script Thread")
            throw new OperationCanceledException();
    }
    public bool? ShowMessageBox(string message, string caption, bool yesAndNo = false)
    {
        return _dialogService.ShowMessageBox(message, caption, yesAndNo);
    }

    public DialogResult ShowMessageBox(string message, string caption, params string[] buttons)
    {
        return _dialogService.ShowMessageBox(message, caption, buttons);
    }

    public void Initialize()
    {
        if (!ScriptInterfaceThread.IsAlive)
            ScriptInterfaceThread.Start();
    }

    public async Task StopTimerAsync()
    {
        ScriptInterfaceCTS?.Cancel();
        await Wait.ForTrueAsync(() => ScriptInterfaceCTS == null, 20);
    }

    public void Stop(bool runScriptStoppingEvent = true)
    {
        Manager.StopScript(runScriptStoppingEvent);
    }

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

                if (Flash.IsWorldLoaded && Player.Playing)
                {
                    Servers.LastIP = Player.ServerIP ?? Servers.LastIP;

                    if (Options.RestPackets && !Player.InCombat)
                        _limit.LimitedRun("rest", 1200, () => Send.Packet("%xt%zm%restRequest%1%%"));

                    if (!catching)
                    {
                        Flash.Call("catchPackets");
                        catching = true;
                    }

                    _limit.LimitedRun("opts", 250, CheckOptions);
                }

                _limit.LimitedRun("connDetail", 100, () => (lastConnChange, lastConnDetail) = CheckStuckonLoading(lastConnChange, lastConnDetail));

                if (Manager.ScriptRunning)
                    RunScriptHandlers();

                sw.Stop();
                Thread.Sleep(Math.Max(10, _timerDelay - (int)sw.Elapsed.TotalMilliseconds));
            }
            catch (Exception e)
            {
                Trace.WriteLine($"Error in timer thread: {e.Message}");
            }
        }
    }

    private void CheckOptions()
    {
        if (Options.LagKiller)
            Flash.Call("killLag", true);

        if (!Player.Playing)
            return;

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
        if (Options.WalkSpeed != 8)
            Player.WalkSpeed = Options.WalkSpeed;
        if (!Lite.UntargetSelf)
            Lite.UntargetSelf = true;
        if (!Lite.UntargetDead)
            Lite.UntargetDead = true;
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
        if (connDetail.Contains("has been lost") && !_waitForLogin)
            OnLogout();
        else if (Environment.TickCount - lastConnChange >= Options.LoadTimeout && connDetail == lastConnDetail && !_waitForLogin)
        {
            if (connDetail.Contains("loading map"))
            {
                Map.Join("battleon");
                Map.Reload();
                Handlers.RegisterOnce(500, b =>
                {
                    if (Flash.GetGameObject("mcConnDetail.txtDetail.text") == "loading map")
                    {
                        Servers.Logout();
                        return;
                    }
                    Map.Join(Map.LastMap);
                });
            }
            else
            {
                Servers.Logout();
            }
        }
        if (connDetail == lastConnDetail)
            return (lastConnChange, connDetail);
        return (Environment.TickCount, connDetail);
    }

    /// <summary>
    /// Run all registered handlers, if the handler returns <see langword="false"/> it is removed from the list.
    /// </summary>
    private void RunScriptHandlers()
    {
        if (!Handlers.CurrentHandlers.Any())
            return;
        List<IHandler> rem = new();
        foreach (IHandler handler in Handlers.CurrentHandlers.ToList())
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
            case "loaded":
                Initialize();
                break;
            case "debug":
                Trace.WriteLine(args[0]);
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
                                Messenger.Send<RunToAreaMessage, int>(new(zone), (int)MessageChannels.GameEvents);
                            break;
                        case "moveToArea":
                            Options.CustomName = !string.IsNullOrWhiteSpace(Options.CustomName) ? Options.CustomName : Player.Username;
                            Options.CustomGuild = !string.IsNullOrWhiteSpace(Options.CustomGuild) ? Options.CustomGuild : Player.Guild;
                            Messenger.Send<MapChangedMessage, int>(new(Convert.ToString(data.strMapName)), (int)MessageChannels.GameEvents);
                            Map.FilePath = Convert.ToString(data.strMapFileName);
                            Map.LastMap = Convert.ToString(data.strMapName);
                            break;
                        case "ct":
                            dynamic p = data.p?[Player.Username.ToLower()]!;
                            if (p is not null && p.intHP == 0)
                            {
                                Stats.Deaths++;
                                Messenger.Send<PlayerDeathMessage, int>((int)MessageChannels.GameEvents);
                                break;
                            }
                            dynamic anims = data.anims?[0]!;
                            if (anims is not null)
                            {
                                string msg = anims["msg"];
                                if (msg is not null && msg.Contains("prepares a counter attack!"))
                                {
                                    Messenger.Send<CounterAttackMessage, int>(new(false), (int)MessageChannels.GameEvents);
                                    break;
                                }
                            }
                            if (data.a is not null)
                            {
                                foreach (var a in data.a)
                                {
                                    if (a is null)
                                        continue;

                                    if (a.aura is not null && (string)a.aura["nam"] is "Counter Attack")
                                    {
                                        Messenger.Send<CounterAttackMessage, int>(new(true), (int)MessageChannels.GameEvents);
                                        break;
                                    }
                                }
                            }
                            break;
                        case "sellItem":
                            Messenger.Send<ItemSoldMessage, int>(new(data.CharItemID, data.iQty, data.iQtyNow, data.intAmount, data.bCoins == 1), (int)MessageChannels.GameEvents);
                            break;
                        case "buyItem":
                            if (data.bitSuccess == 1)
                                Messenger.Send<ItemBoughtMessage, int>(new(Convert.ToInt32(data.CharItemID)), (int)MessageChannels.GameEvents);
                            break;
                        case "dropItem":
                            string items = Convert.ToString(data["items"]);
                            InventoryItem drop = JsonConvert.DeserializeObject<Dictionary<string, InventoryItem>>(items)!.First().Value;
                            Messenger.Send<ItemDroppedMessage, int>(new(drop), (int)MessageChannels.GameEvents);
                            break;
                        case "addItems":
                            string addItems = Convert.ToString(data["items"]);
                            Dictionary<int, dynamic> addedItem = JsonConvert.DeserializeObject<Dictionary<int, dynamic>>(addItems)!;
                            int itemID = addedItem.Keys.First()!;
                            ItemBase invItem = Inventory.GetItem(itemID)!;
                            if (invItem is null)
                                invItem = TempInv.GetItem(itemID)!;
                            if(invItem is null)
                            {
                                invItem = Bank.GetItem(itemID)!;
                                Messenger.Send<ItemAddedToBankMessage, int>(new(invItem, invItem.Quantity), (int)MessageChannels.GameEvents);
                                break;
                            }
                            if (!invItem.Temp)
                                Stats.Drops++;
                            Messenger.Send<ItemDroppedMessage, int>(new(invItem, true, Convert.ToInt32(addedItem.Values.First().iQtyNow)), (int)MessageChannels.GameEvents);
                            break;
                        case "getDrop":
                            bool toBank = Convert.ToBoolean(data.bBank);
                            if (data.bSuccess == 1)
                                Stats.Drops += (int)data.iQty;
                            if(toBank)
                            {
                                ItemBase bankItem = Bank.GetItem(Convert.ToInt32(data.ItemID))!;
                                Messenger.Send<ItemAddedToBankMessage, int>(new(bankItem, Convert.ToInt32(data.iQtyNow)), (int)MessageChannels.GameEvents);
                            }
                            break;
                        case "addGoldExp":
                            if (data.typ == "m")
                            {
                                Stats.Kills++;
                                Messenger.Send<MonsterKilledMessage, int>(new(Convert.ToInt32(data.id)), (int)MessageChannels.GameEvents);
                            }
                            break;
                        case "ccqr":
                            if (data.bSuccess == 1)
                            {
                                Stats.QuestsCompleted++;
                                Messenger.Send<QuestTurninMessage, int>(new(Convert.ToInt32(data.QuestID)), (int)MessageChannels.GameEvents);
                            }
                            break;
                        case "loadBank":
                            Messenger.Send<BankLoadedMessage, int>((int)MessageChannels.GameEvents);
                            break;
                        case "loadShop":
                            Messenger.Send<ShopLoadedMessage, int>(new(new(Shops.ID, Shops.Name, Shops.Items)), (int)MessageChannels.GameEvents);
                            break;
                    }
                }
                else if (type is not null and "str")
                {
                    string cmd = data[0];
                    switch (cmd)
                    {
                        case "popup":
                            string b = Convert.ToString(packet);
                            Debug.WriteLine(b);
                            break;
                        case "uotls":
                            if (Player.Username == (string)data[2] && data[3] == "afk:true")
                                Messenger.Send<PlayerAFKMessage, int>((int)MessageChannels.GameEvents);
                            break;
                        case "loginResponse":
                            Messenger.Send<LoginMessage, int>(new(Convert.ToString(data[4])), (int)MessageChannels.GameEvents);
                            break;
                    }
                }
                Messenger.Send<ExtensionPacketMessage, int>(new(packet), (int)MessageChannels.GameEvents);
                break;
            case "packet":
                string[] parts = ((string)args[0]).Split('%', StringSplitOptions.RemoveEmptyEntries);
                switch (parts[2])
                {
                    case "moveToCell":
                        Messenger.Send<CellChangedMessage, int>(new(Map.Name, parts[4], parts[5]), (int)MessageChannels.GameEvents);
                        break;
                    case "buyItem":
                        Messenger.Send<TryBuyItemMessage, int>(new(int.Parse(parts[5]), int.Parse(parts[4]), int.Parse(parts[6])), (int)MessageChannels.GameEvents);
                        break;
                    case "acceptQuest":
                        Stats.QuestsAccepted++;
                        Messenger.Send<QuestAcceptedMessage, int>(new(int.Parse(parts[4])), (int)MessageChannels.GameEvents);
                        break;
                    case "cmd":
                        if (parts.Length >= 5 && parts[4] == "logout")
                        {
                            Messenger.Send<LogoutMessage, int>((int)MessageChannels.GameEvents);
                            OnLogout();
                        }
                        break;
                }
                Messenger.Send<PacketMessage, int>(new((string)args[0]), (int)MessageChannels.GameEvents);
                break;
        }
    }

    private Task? _reloginTask;
    private volatile bool _waitForLogin;
    private CancellationTokenSource? _reloginCTS;
    private void OnLogout()
    {
        if (!Options.AutoRelogin || _waitForLogin)
            return;

        if (_reloginTask is not null && !_waitForLogin)
        {
            Log("Relogin task already running.");
            _waitForLogin = true;
            return;
        }

        Log("Autorelogin triggered.");
        bool wasRunning = Manager.ScriptRunning;
        Manager.StopScript();
        bool kicked = Player.Kicked;
        _waitForLogin = true;
        Messenger.Send<ReloginTriggeredMessage, int>(new(kicked), (int)MessageChannels.GameEvents);

        Relogin((!Options.SafeRelogin && !kicked) ? 5000 : 70000, wasRunning);
    }
    private void Relogin(int delay, bool startScript)
    {
        Servers.Logout();
        Log($"Waiting {delay}ms for relogin.");
        _reloginCTS = new CancellationTokenSource();
        _reloginTask = Schedule(delay, async _ =>
        {
            Stats.Relogins++;
            bool relogged = await Servers.EnsureRelogin(_reloginCTS.Token);
            if (startScript)
                await Ioc.Default.GetService<IScriptManager>()!.StartScriptAsync();
            Log($"Relogin was {(relogged ? "successful" : "cancelled or unsuccessful")}.");
            _reloginCTS.Dispose();
            _reloginCTS = null;
            _reloginTask = null;
            _waitForLogin = false;
        });
    }
}
