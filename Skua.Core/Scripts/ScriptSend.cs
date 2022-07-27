using Skua.Core.Interfaces;

namespace Skua.Core.Scripts;
public class ScriptSend : IScriptSend
{
    private readonly Lazy<IFlashUtil> _lazyFlash;
    private IFlashUtil Flash => _lazyFlash.Value;

    public ScriptSend(Lazy<IFlashUtil> flash)
    {
        _lazyFlash = flash;
    }

    public void Packet(string packet, string type = "String")
    {
        Flash.CallGameFunction($"sfc.send{type}", packet);
    }

    public Task PacketSpam(string packet, string type, int delay, CancellationToken token)
    {
        return Task.Factory.StartNew(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                Packet(packet, type);
                await Task.Delay(delay, token);
            }
        }, token);
    }

    public void ClientPacket(string packet, string type = "str")
    {
        Flash.Call("sendClientPacket", packet, type);
    }

    public Task ClientPacketSpam(string packet, string type, int delay, CancellationToken token)
    {
        return Task.Factory.StartNew(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                ClientPacket(packet, type);
                await Task.Delay(delay, token);
            }
        }, token);
    }

    public void Whisper(string name, string message)
    {
        Flash.CallGameFunction("sfc.sendString", $"%xt%zm%whisper%1%{message}%{name}%");
    }

    public void ClientServer(string message, string sentBy = "Skua")
    {
        SendMSGPacket(message, sentBy, "server");
    }

    public void ClientModerator(string message, string sentBy = "Skua")
    {
        SendMSGPacket(message, sentBy, "moderator");
    }

    public void ClientWarning(string message, string sentBy = "Skua")
    {
        SendMSGPacket(message, sentBy, "warning");
    }

    public void ClientEvent(string message, string sentBy = "Skua")
    {
        SendMSGPacket(message, sentBy, "event");
    }

    public void ClientGuild(string message, string sentBy = "Skua")
    {
        SendMSGPacket(message, sentBy, "guild");
    }

    public void ClientWhisper(string message, string sentBy = "Skua")
    {
        SendMSGPacket(message, sentBy, "whisper");
    }

    public void ClientZone(string message, string sentBy = "Skua")
    {
        SendMSGPacket(message, sentBy, "zone");
    }

    private void SendMSGPacket(string message, string sentBy, string messageType)
    {
        Flash.Call("sendClientPacket", $"%xt%chatm%0%{messageType}~{message}%{sentBy}%", "str");
    }
}
