using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Skua.Core.Models.Quests;

namespace Skua.gRPC.Server.Services;

public class SynchronizerService : Synchronizer.SynchronizerBase
{
    private readonly SyncManager _syncManager;
    private int _id;

    public SynchronizerService(SyncManager syncManager)
    {
        _syncManager = syncManager;
    }

    public override async Task StartSync(IAsyncStreamReader<SyncRequest> requestStream, IServerStreamWriter<SyncResponse> responseStream, ServerCallContext context)
    {
        var request = ReceiveCommand(requestStream, context);
        var responses = ProcessCommand(responseStream, context);

        await Task.WhenAll(request, responses);
    }

    private async Task ReceiveCommand(IAsyncStreamReader<SyncRequest> requestStream, ServerCallContext context)
    {
        bool added = false;
        while(await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested)
        {
            var current = requestStream.Current;
            if(!added)
            {
                _id = current.Id;
                _syncManager.AddClient(current.Id);
                added = true;
            }

        }
    }

    private async Task ProcessCommand(IServerStreamWriter<SyncResponse> responseStream, ServerCallContext context)
    {
        while (!context.CancellationToken.IsCancellationRequested)
        {
            if (context.CancellationToken.IsCancellationRequested)
                break;

            var command = await _syncManager.ProcessCommand(_id);

            await responseStream.WriteAsync(command);

            await _syncManager.WaitResponses();
        }
    }

    public override async Task<Empty> Reply(ReplyRequest request, ServerCallContext context)
    {
        await _syncManager.AddReply(request);
        return new Empty();
    }

    public override async Task<Empty> SendCommand(SendCommandRequest request, ServerCallContext context)
    {
        await _syncManager.ReceiveCommand(request);

        return new Empty();
    }
}
