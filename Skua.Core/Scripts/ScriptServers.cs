using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Skua.Core.Flash;
using Skua.Core.Interfaces;
using Skua.Core.Models.Servers;
using Skua.Core.Utils;

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

    private bool _loginInfoSetted = false;
    private string _username;
    private string _password;

    public string LastIP { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    // Flash Objects Binding

    [ObjectBinding("serialCmd.servers")]
    private List<Server> _serverList = new();

    [ObservableProperty]
    [NotifyPropertyChangedRecipients]
    private List<Server> _cachedServers = new();

    // Flash Methods Binding

    [MethodCallBinding("login", GameFunction = true)]
    private void _login(string username, string password)
    { }

    [MethodCallBinding("logout", RunMethodPost = true, GameFunction = true)]
    private void _logout()
    {
        Flash.CallGameFunction("gotoAndPlay", "Login");
    }

    [MethodCallBinding("connectTo", RunMethodPost = true, GameFunction = true)]
    private bool _connectIP(string ip)
    {
        Wait.ForTrue(() => !Manager.ShouldExit && Player.Playing && Flash.IsWorldLoaded, 30);
        return Player.Playing;
    }

    [MethodCallBinding("connectTo", RunMethodPost = true, GameFunction = true)]
    private bool _connectIP(string ip, int port)
    {
        Wait.ForTrue(() => !Manager.ShouldExit && Player.Playing && Flash.IsWorldLoaded, 30);
        return Player.Playing;
    }

    [MethodCallBinding("connectToServer", RunMethodPost = true)]
    private bool _connectToServer(string server)
    {
        Wait.ForTrue(() => !Manager.ShouldExit && Player.Playing && Flash.IsWorldLoaded, 30);
        return Player.Playing;
    }

    public void Login() => Login(Player.Username, Player.Password);

    public async ValueTask<List<Server>> GetServers(bool forceUpdate = false)
    {
        if (CachedServers.Count > 0 && !forceUpdate)
            return CachedServers;

        string? response = await HttpClients.GetGHClient()
            .GetStringAsync($"http://content.aq.com/game/api/data/servers")
            .ConfigureAwait(false);

        if (response is null)
            return new();

        CachedServers = JsonConvert.DeserializeObject<List<Server>>(response)!;

        return CachedServers;
    }

    public void SetLoginInfo(string username, string password)
    {
        _username = username;
        _password = password;
        _loginInfoSetted = true;
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

    public bool Relogin(Server? server = null)
    {
        if (server is null)
        {
            if (Options.AutoReloginAny)
                server = ServerList.Find(x => x.IP != LastIP)!;
            else
                server = CachedServers.First(s => s.Name == Options.ReloginServer) ?? ServerList[0];
        }

        return ReloginIP(server.IP);
    }

    public bool ReloginIP(string ip)
    {
        bool autoRelogSwitch = Options.AutoRelogin;
        Options.AutoRelogin = false;

        Thread.Sleep(2000);
        Logout();

        Stats.Relogins++;
        if (_loginInfoSetted)
            Login(_username, _password);
        else
            Login(Player.Username, Player.Password);

        Thread.Sleep(2000);
        ConnectIP(ip);

        Wait.ForTrue(() => Player.Playing && Flash.IsWorldLoaded, 30);
        Options.AutoRelogin = autoRelogSwitch;

        return Player.Playing;
    }

    public bool Relogin(string serverName)
    {
        bool autoRelogSwitch = Options.AutoRelogin;
        Options.AutoRelogin = false;

        Logout();
        Stats.Relogins++;

        if (_loginInfoSetted)
            Login(_username, _password);
        else
            Login(Player.Username, Player.Password);

        Server server = ServerList.Find(x => x.Name.Contains(serverName))
            ?? CachedServers.Find(x => x.Name.Contains(serverName))!;
        ConnectToServer(JsonConvert.SerializeObject(server));

        Wait.ForTrue(() => Player.Playing && Flash.IsWorldLoaded, 30);
        Options.AutoRelogin = autoRelogSwitch;

        return Player.Playing;
    }

    public bool EnsureRelogin(string serverName)
    {
        int tries = 0;
        while (!Relogin(serverName) && !Manager.ShouldExit && !Player.Playing && ++tries < Options.ReloginTries)
            Task.Delay(Options.ReloginTryDelay).Wait();

        return Player.Playing;
    }

    public async Task<bool> EnsureRelogin(CancellationToken token)
    {
        Server server;
        await GetServers(true);
        if (Options.AutoReloginAny)
            server = ServerList.Find(x => x.IP != LastIP)!;
        else
            server = CachedServers.FirstOrDefault(s => s.Name == Options.ReloginServer) ?? ServerList[0];

        return await EnsureLogin(server, token);
    }

    public async Task<bool> EnsureRelogin(string serverName, CancellationToken token)
    {
        Server? server = ServerList.Find(x => x.Name == serverName);
        server ??= ServerList[0];

        return await EnsureLogin(server, token);
    }

    private async Task<bool> EnsureLogin(Server server, CancellationToken token)
    {
        int tries = 0;
        try
        {
            while (!token.IsCancellationRequested && !Manager.ShouldExit && !Player.Playing && ++tries < Options.ReloginTries)
            {
                Login();

                if (!Flash.Call<bool>("clickServer", server.Name))
                    ConnectIP(server.IP, server.Port);

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