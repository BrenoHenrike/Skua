namespace Skua.Core.ViewModels;
public record InterceptedPacketViewModel(string Packet, bool? Outbound)
{
    public override string ToString()
    {
        return Packet;
    }
}