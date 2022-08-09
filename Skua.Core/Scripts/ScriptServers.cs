using System.Diagnostics;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models.Servers;
using Skua.Core.Utils;
using Skua.Core.Flash;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Skua.Core.Scripts;

public partial class ScriptServers : ObservableRecipient, IScriptServers
{
    public ScriptServers(
        Lazy<IFlashUtil> flash,
        Lazy<IScriptPlayer> player,
        Lazy<IScriptWait> wait,
        Lazy<IScriptOption> options,
        Lazy<IScriptBotStats> stats,
        Lazy<IScriptManager> manager)
    {
        _lazyFlash = flash;
        _lazyPlayer = player;
        _lazyWait = wait;
        _lazyOptions = options;
        _lazyStats = stats;
        _lazyManager = manager;
    }

    private readonly Lazy<IFlashUtil> _lazyFlash;
    private readonly Lazy<IScriptPlayer> _lazyPlayer;
    private readonly Lazy<IScriptWait> _lazyWait;
    private readonly Lazy<IScriptOption> _lazyOptions;
    private readonly Lazy<IScriptBotStats> _lazyStats;
    private readonly Lazy<IScriptManager> _lazyManager;

    private IFlashUtil Flash => _lazyFlash.Value;
    private IScriptPlayer Player => _lazyPlayer.Value;
    private IScriptWait Wait => _lazyWait.Value;
    private IScriptOption Options => _lazyOptions.Value;
    private IScriptBotStats Stats => _lazyStats.Value;
    private IScriptManager Manager => _lazyManager.Value;

    public string LastIP { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    [ObjectBinding("serialCmd.servers")]
    private List<Server> _serverList = new();
    [ObservableProperty]
    [NotifyPropertyChangedRecipients]
    private List<Server> _cachedServers = new();

    public async ValueTask<List<Server>> GetServers(bool forceUpdate = false)
    {
        if (CachedServers.Count > 0 && !forceUpdate)
            return CachedServers;

        string? response = await HttpClients.GetGHClient().GetStringAsync($"http://content.aq.com/game/api/data/servers").ConfigureAwait(false);
        return response is null ? new() : CachedServers = JsonConvert.DeserializeObject<List<Server>>(response)!;
    }

    [MethodCallBinding("login", GameFunction = true)]
    private void _login(string username, string password) { }

    public void Login() => Login(Player.Username, Player.Password);

    [MethodCallBinding("connectTo", RunMethodPost = true, GameFunction = true)]
    private bool _connectIP(string ip)
    {
        Wait.ForTrue(() => !Manager.ShouldExit && Player.Playing && Flash.IsWorldLoaded, 30);
        return Player.Playing;
    }

    public bool Reconnect(string serverName, int loginDelay = 2000)
    {
        Login(Player.Username, Player.Password);
        Thread.Sleep(loginDelay);
        return ((IScriptServers)this).Connect(serverName);
    }

    public bool Reconnect(Server server, int loginDelay = 2000)
    {
        Login(Player.Username, Player.Password);
        Thread.Sleep(loginDelay);
        return ((IScriptServers)this).Connect(server);
    }

    [MethodCallBinding("logout", RunMethodPost = true, GameFunction = true)]
    private void _logout()
    {
        Flash.CallGameFunction("gotoAndPlay", "Login");
    }

    public bool Relogin(Server? server = null)
    {
        if (server is null)
            server = Options.AutoReloginAny ? ServerList.Find(x => x.IP != LastIP)! : CachedServers.First(s => s.Name == Options.ReloginServer) ?? ServerList[0];
        return ReloginIP(server.IP);
    }

    public bool ReloginIP(string ip)
    {
        bool autoRelogSwitch = Options.AutoRelogin;
        Options.AutoRelogin = false;
        Thread.Sleep(2000);
        Logout();
        Stats.Relogins++;
        Login(Player.Username, Player.Password);
        Thread.Sleep(2000);
        ConnectIP(ip);
        Wait.ForTrue(() => Player.Playing && Flash.IsWorldLoaded, 30);
        Options.AutoRelogin = autoRelogSwitch;
        return Player.Playing;
    }

    public bool Relogin(string serverName)
    {
        Server s = ServerList.Find(x => x.Name.Contains(serverName))!;
        if (s is not null)
            return ReloginIP(s.IP);
        Trace.WriteLine($"Server with name \"{serverName}\" was not found.");
        return false;
    }

    public bool EnsureRelogin(string serverName)
    {
        Server s = ServerList.Find(x => x.Name.Contains(serverName))!;
        int tries = 0;
        while (!Relogin(s.IP) && !Manager.ShouldExit && !Player.Playing && ++tries < Options.ReloginTries)
            Thread.Sleep(Options.ReloginTryDelay);
        return Player.Playing;
    }

    public async Task<bool> EnsureRelogin(CancellationToken token)
    {
        int tries = 0;
        await GetServers(true);
        try
        {
            while (!token.IsCancellationRequested && !Manager.ShouldExit && !Player.Playing && ++tries < Options.ReloginTries)
            {
                Login();
                await Task.Delay(3000, token);
                Server server = Options.AutoReloginAny ? ServerList.Find(x => x.IP != LastIP)! : CachedServers.FirstOrDefault(s => s.Name == Options.ReloginServer) ?? ServerList[0];
                ConnectIP(server.IP);
                using CancellationTokenSource waitLogin = new(Options.LoginTimeout);
                try
                {
                    while ((!Player.Playing || !Flash.IsWorldLoaded) && !waitLogin.IsCancellationRequested)
                        await Task.Delay(500, token);
                }
                catch { }
            }
        }
        catch { }
        return Player.Playing;
    }
}
