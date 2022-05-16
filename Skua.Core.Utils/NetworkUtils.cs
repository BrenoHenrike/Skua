using System.Net;
using System.Net.Sockets;

namespace Skua.Core.Utils;

public class NetworkUtils
{
    public static int GetAvailablePort()
    {
        int port;
        using (Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            port = (socket.LocalEndPoint as IPEndPoint).Port;
        }
        return port;
    }
}
