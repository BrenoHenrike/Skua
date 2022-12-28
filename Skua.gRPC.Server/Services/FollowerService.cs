using Grpc.Core;

namespace Skua.gRPC.Server.Services;

public class FollowerService : Follower.FollowerBase
{
    private readonly FollowerHandler _handler;

    public FollowerService(FollowerHandler handler)
    {
        _handler = handler;
    }

    public override async Task StartFollowing(FollowerRequest request, IServerStreamWriter<FollowerResponse> responseStream, ServerCallContext context)
    {
        while(!context.CancellationToken.IsCancellationRequested)
        {
            if (context.CancellationToken.IsCancellationRequested)
                break;

            var commands = await _handler.ProcessCommand();

            foreach(var command in commands)
            {
                await responseStream.WriteAsync(command);
            }
        }
    }
}
