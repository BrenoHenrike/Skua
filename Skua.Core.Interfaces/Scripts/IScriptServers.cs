using System.ComponentModel;
using Skua.Core.Models.Servers;

namespace Skua.Core.Interfaces;
public interface IScriptServers : INotifyPropertyChanged
{
    void SetLoginInfo(string username, string password);
    /// <summary>
    /// The IP of the last server the player was connected to.
    /// </summary>
    string LastIP { get; set; }
    /// <summary>
    /// The name of the last server the player was connected to.
    /// </summary>
    string LastName { get; set; }
    /// <summary>
    /// The list of available game servers.
    /// </summary>
    List<Server> ServerList { get; }
    /// <summary>
    /// A list of servers cached from the game api.
    /// </summary>
    List<Server> CachedServers { get; }
    /// <summary>
    /// Gets a list of servers from the game api.
    /// </summary>
    /// <param name="forceUpdate">Whether to force an update of the list.</param>
    /// <returns>A <see cref="Server"/> object list.</returns>
    ValueTask<List<Server>> GetServers(bool forceUpdate = false);
    /// <summary>
    /// Connects to the specified game <paramref name="server"/>.
    /// </summary>
    /// <param name="server"><see cref="Server"/> to connect to.</param>
    bool Connect(Server server)
    {
        return ConnectIP(server.IP);
    }
    /// <summary>
    /// Connects to the game server with the specified <paramref name="serverName"/>.
    /// </summary>
    /// <param name="serverName">Name of the server to connect to (e.g. Artix)</param>
    bool Connect(string serverName)
    {
        Server s = ServerList.Find(x => x.Name.Contains(serverName))!;
        if (s is not null)
            return Connect(s.IP);
        return false;
    }
    /// <summary>
    /// Connects to the game server with the specified <paramref name="serverIp"/> address.
    /// </summary>
    /// <param name="ip">IP address of the server to connec to.</param>
    bool ConnectIP(string serverIp);
    /// <summary>
    /// Logs into the game with the specified username and password.
    /// </summary>
    /// <param name="username">Username to login with.</param>
    /// <param name="password">Password to login with.</param>
    void Login(string username, string password);
    void Login();
    /// <summary>
    /// Logs out of the game.
    /// </summary>
    void Logout();
    /// <summary>
    /// Logs in and connects to the server with specified <paramref name="name"/>.
    /// </summary>
    /// <param name="serverName">Name of the server to connect to.</param>
    /// <param name="loginDelay">Delay before trying to connect to the <paramref name="server"/></param>
    bool Reconnect(string serverName, int loginDelay = 2000);
    /// <summary>
    /// Logs in and connects to the specified <paramref name="server"/>.
    /// </summary>
    /// <param name="server">Server to connect to.</param>
    /// <param name="loginDelay">Delay before trying to connect to the <paramref name="server"/></param>
    bool Reconnect(Server server, int loginDelay = 2000);
    /// <summary>
    /// Logs out and then tries to login back.
    /// </summary>
    /// <param name="server">Server to connect to.</param>
    /// <returns><see langword="true"/> if the player has successfuly connected.</returns>
    /// <remarks>This will disable <see cref="IScriptOption.AutoRelogin"/>, try to login and then enable it again.</remarks>
    bool Relogin(Server server = null!);
    /// <summary>
    /// Logs out and then tries to login back.
    /// </summary>
    /// <param name="serverName">Name of the server to connect to.</param>
    /// <returns><see langword="true"/> if the player has successfuly connected.</returns>
    /// <remarks>This will disable <see cref="IScriptOption.AutoRelogin"/>, try to login and then enable it again.</remarks>
    bool Relogin(string serverName);
    /// <summary>
    /// Logs out and then tries to login back.
    /// </summary>
    /// <param name="serverName">IP of the server to connect to.</param>
    /// <returns><see langword="true"/> if the player has successfuly connected.</returns>
    /// <remarks>This will disable <see cref="IScriptOption.AutoRelogin"/>, try to login and then enable it again.</remarks>
    bool ReloginIP(string ip);
    /// <summary>
    /// Tries to relogin for the number of <see cref="IScriptOption.ReloginTries"/>.
    /// </summary>
    /// <param name="serverName">Name of the server to connect to.</param>
    /// <returns><see langword="true"/> if the relogin was successful</returns>
    /// <remarks>This will disable <see cref="IScriptOption.AutoRelogin"/>, try to login and then enable it again.</remarks>
    bool EnsureRelogin(string serverName);
    /// <summary>
    /// Tries to relogin for the number of <see cref="IScriptOption.ReloginTries"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the relogin was successful</returns>
    /// <remarks>This is mainly used for auto relogin. If you plan in using this, do a <see cref="Logout"/> first.</remarks>
    Task<bool> EnsureRelogin(CancellationToken token);
}