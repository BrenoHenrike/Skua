using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Skua.gRPC.Server.Services;

public class MainAccountService : MainAccount.MainAccountBase
{
    private readonly FollowerHandler _handler;

    public MainAccountService(FollowerHandler handler)
    {
        _handler = handler;
    }

    public override async Task<Empty> SendCommand(CommandRequest request, ServerCallContext context)
    {
        await _handler.ReceiveCommand(request.Command);

        return new Empty();
    }
}
