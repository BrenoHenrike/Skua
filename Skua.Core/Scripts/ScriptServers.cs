using System.Diagnostics;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models.Servers;
using Skua.Core.PostSharp;
using Skua.Core.Utils;

namespace Skua.Core.Scripts;

public class ScriptServers : ScriptableObject, IScriptServers
{
    public string LastIP { get; set; } = default!;
    public string LastName { get; set; } = default!;
    [ObjectBinding("serialCmd.servers")]
    public List<Server> ServerList { get; } = new();
    public List<Server> CachedServers { get; private set; } = new();

    public async ValueTask<List<Server>> GetServers(bool forceUpdate = false)
    {
        if (CachedServers.Count > 0 && !forceUpdate)
            return CachedServers;

        var response = await HttpClients.GitHubClient.GetStringAsync($"http://content.aq.com/game/api/data/servers").ConfigureAwait(false);
        if (response is null)
            return new();
        return CachedServers = JsonConvert.DeserializeObject<List<Server>>(response)!;
    }

    [MethodCallBinding("login", GameFunction = true)]
    public void Login(string username, string password) { }

    [MethodCallBinding("connectTo", RunMethodPost = true, GameFunction = true)]
    public bool ConnectIP(string ip)
    {
        Bot.Wait.ForTrue(() => !Bot.ShouldExit && Bot.Player.Playing && Bot.IsWorldLoaded, 30);
        return Bot.Player.Playing;
    }

    public bool Reconnect(string serverName, int loginDelay = 2000)
    {
        Login(Bot.Player.Username, Bot.Player.Password);
        Bot.Sleep(loginDelay);
        return ((IScriptServers)this).Connect(serverName);
    }

    public bool Reconnect(Server server, int loginDelay = 2000)
    {
        Login(Bot.Player.Username, Bot.Player.Password);
        Bot.Sleep(loginDelay);
        return ((IScriptServers)this).Connect(server);
    }

    [MethodCallBinding("logout", RunMethodPost = true, GameFunction = true)]
    public void Logout()
    {
        Bot.Flash.CallGameFunction("gotoAndPlay", "Login");
    }

    public bool Relogin(Server? server = null)
    {
        if (server is null)
            server = Bot.Options.AutoReloginAny ? ServerList.Find(x => x.IP != LastIP)! : Bot.Options.LoginServer ?? ServerList[0];
        return ReloginIP(server.IP);
    }

    public bool ReloginIP(string ip)
    {
        bool autoRelogSwitch = Bot.Options.AutoRelogin;
        Bot.Options.AutoRelogin = false;
        Bot.Sleep(2000);
        Logout();
        Bot.Stats.Relogins++;
        Login(Bot.Player.Username, Bot.Player.Password);
        Bot.Sleep(2000);
        ConnectIP(ip);
        Bot.Wait.ForTrue(() => Bot.Player.Playing && Bot.IsWorldLoaded, 30);
        Bot.Options.AutoRelogin = autoRelogSwitch;
        return Bot.Player.Playing;
    }

    public bool Relogin(string serverName)
    {
        Server s = ServerList.Find(x => x.Name.Contains(serverName))!;
        if (s is not null)
            return ReloginIP(s.IP);
        Debug.WriteLine($"Server with name \"{serverName}\" was not found.");
        return false;
    }

    public bool EnsureRelogin(string serverName)
    {
        Server s = ServerList.Find(x => x.Name.Contains(serverName))!;
        int tries = 0;
        while (!Relogin(s.IP) && !Bot.ShouldExit && !Bot.Player.Playing && ++tries < Bot.Options.ReloginTries)
            Bot.Sleep(Bot.Options.ReloginTryDelay);
        return Bot.Player.Playing;
    }

    public async Task<bool> EnsureRelogin(CancellationToken token)
    {
        int tries = 0;
        while (!token.IsCancellationRequested && !Bot.Player.Playing && ++tries < Bot.Options.ReloginTries)
        {
            Login(Bot.Player.Username, Bot.Player.Password);
            await Task.Delay(2000, token);
            await GetServers(true);
            Server server = Bot.Options.AutoReloginAny ? ServerList.Find(x => x.IP != LastIP)! : Bot.Options.LoginServer ?? ServerList[0];
            ConnectIP(server.IP);
            while ((!Bot.Player.Playing || !Bot.IsWorldLoaded) && !token.IsCancellationRequested)
                await Task.Delay(500, token);
        }
        return Bot.Player.Playing;
    }
}
