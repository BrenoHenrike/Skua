using CommunityToolkit.Mvvm.ComponentModel;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Utils;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Skua.Core.GameProxy;

public partial class CaptureProxy : ObservableRecipient, ICaptureProxy
{
    private CancellationTokenSource? _captureProxyCTS;

    /// <summary>
    /// The default port for the capture proxy to run on.
    /// </summary>
    public const int DefaultPort = 5588;

    public IPEndPoint? Destination { get; set; }
    public List<IInterceptor> Interceptors { get; } = new();

    private Thread? _thread;
    private readonly TcpListener _listener;
    private TcpClient? _forwarder;
    private TcpClient? _client;

    [ObservableProperty]
    [NotifyPropertyChangedRecipients]
    private bool _running;

    public CaptureProxy()
    {
        _listener = new TcpListener(IPAddress.Loopback, DefaultPort);
    }

    public void Start()
    {
        if (Destination == null)
            return;
        Running = true;
        _thread = new(() =>
        {
            _captureProxyCTS = new();
            _Listen(_captureProxyCTS.Token);
            _captureProxyCTS.Dispose();
            _captureProxyCTS = null;
        });
        _thread.Name = "Capture Proxy";
        _thread.Start();
    }

    public void Stop()
    {
        _listener?.Stop();
        if (_forwarder?.Connected ?? false)
        {
            _forwarder.Close();
            _forwarder.Dispose();
        }
        if (_client?.Connected ?? false)
        {
            _client.Close();
            _client.Dispose();
        }
        _captureProxyCTS?.Cancel();
        Running = false;
    }

    private void _Listen(CancellationToken token)
    {
        _listener.Start();
        while (!token.IsCancellationRequested)
        {
            TcpClient? localClient = null;
            TcpClient? localForwarder = null;
            try
            {
                localClient = _listener.AcceptTcpClient();
                localForwarder = new TcpClient();
                localForwarder.Connect(Destination!);

                _client = localClient;
                _forwarder = localForwarder;

                var client = localClient;
                var forwarder = localForwarder;

                Task.Factory.StartNew(() => _DataInterceptor(client, forwarder, true, token), token);
                Task.Factory.StartNew(() => _DataInterceptor(forwarder, client, false, token), token);
            }
            catch
            {
                // Ensure proper cleanup on error
                localClient?.Close();
                localClient?.Dispose();
                localForwarder?.Close();
                localForwarder?.Dispose();
            }
        }

        // Cleanup when exiting the loop
        _listener.Stop();
    }

    private async Task _DataInterceptor(TcpClient target, TcpClient destination, bool outbound, CancellationToken token)
    {
        byte[] msgbuf = new byte[4096];
        int read = 0;
        List<byte> cpacket = new();

        try
        {
            while (!token.IsCancellationRequested && target.Connected && destination.Connected)
            {
                read = await target.GetStream().ReadAsync(msgbuf, 0, 4096, token);

                if (read == 0)
                {
                    await Task.Delay(10, token);
                    continue;
                }

                for (int i = 0; i < read; i++)
                {
                    if (token.IsCancellationRequested)
                        break;

                    byte b = msgbuf[i];
                    if (b > 0)
                    {
                        cpacket.Add(b);
                        continue;
                    }
                    byte[] data = cpacket.ToArray();
                    cpacket.Clear();

                    MessageInfo message = new(Encoding.UTF8.GetString(data, 0, data.Length));
                    Interceptors.OrderBy(i => i.Priority).ForEach(i => i.Intercept(message, outbound));
                    if (message.Send)
                    {
                        byte[]? msg = new byte[message.Content.Length + 1];
                        Buffer.BlockCopy(_ToBytes(message.Content), 0, msg, 0, message.Content.Length);
                        await destination.GetStream().WriteAsync(msg, token);
                        msg = null;
                    }
                }
            }
        }
        catch (Exception)
        {
            // Connection lost or cancelled
        }
        finally
        {
            // Ensure connections are closed when done
            try { target.Close(); } catch { }
            try { destination.Close(); } catch { }
        }
    }

    private static byte[] _ToBytes(string s)
    {
        return s.Select(c => (byte)c).ToArray();
    }
}