using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Skua.gRPC.Server;

public class SyncManager
{
    public SyncManager()
    {
        _replies = Channel.CreateUnbounded<ReplyRequest>();
    }

    private readonly Dictionary<int, object> _syncClients = new();
    private readonly SemaphoreSlim _semaphore = new(1);
    public readonly SemaphoreSlim SyncSemaphore = new(1);
    //private List<SendCommandRequest> _commands = new();
    //private readonly Dictionary<string, List<SendCommandRequest>> _commands = new();
    private readonly Dictionary<int, Channel<SendCommandRequest>> _cmdTest = new();
    private readonly Channel<ReplyRequest> _replies;

    public async Task WaitResponses()
    {
        while (_replies.Reader.Count < _cmdTest.Count)
        {
            await Task.Delay(1000);
        }
        List<ReplyRequest> replies = new();

        await foreach (var reply in _replies.Reader.ReadAllAsync())
        {
            replies.Add(reply);
        }


    }

    public async Task AddReply(ReplyRequest reply)
    {
        await _replies.Writer.WriteAsync(reply);
    }

    public bool CheckClient(int id)
    {
        return _syncClients.ContainsKey(id);
    }

    public bool AddClient(int id)
    {
        if(_cmdTest.ContainsKey(id))
            return false;

        _cmdTest.Add(id, Channel.CreateUnbounded<SendCommandRequest>());
        return true;
    }

    public async Task ReceiveCommand(SendCommandRequest command)
    {
        await _semaphore.WaitAsync();
        if (command.Id == 0)
        {
            async Task Test(ChannelWriter<SendCommandRequest> writer)
            {
                await writer.WriteAsync(command);
            }
            await Task.WhenAll(_cmdTest.Select(x => Test(x.Value.Writer)));
            _semaphore.Release();
            return;
        }

        if (_cmdTest.TryGetValue(command.Id, out Channel<SendCommandRequest> commands))
        {
            await commands.Writer.WriteAsync(command);
        }
        _semaphore.Release();
    }

    public async Task<SyncResponse> ProcessCommand(int id)
    {
        var command = await _cmdTest[id].Reader.ReadAsync();
        SyncResponse response = new()
        {
            Command = command.Command,
        };
        response.Arguments.AddRange(command.Arguments);
        return response;
    }

    //public async Task ReceiveCommand(SendCommandRequest command)
    //{
    //    await _semaphore.WaitAsync();
    //    if(command.Id == "0")
    //    {
    //        foreach (var c in _commands)
    //        {
    //            c.Value.Add(command);
    //        }
    //        _semaphore.Release();
    //        return;
    //    }
    //    if(_commands.TryGetValue(command.Id, out List<SendCommandRequest> commands))
    //    {
    //        commands.Add(command);
    //    }
    //    else
    //    {
    //        _commands.Add(command.Id, new List<SendCommandRequest> { command });
    //    }
    //    //_commands.Add(command);
    //    _semaphore.Release();
    //}

    //public async Task<List<SyncResponse>> ProcessCommand(string key)
    //{
    //_commands.TryAdd(key, new());

    //var cList = _commands[key];
    //while (cList.Count <= 0)
    //    await Task.Delay(1000);

    //try
    //{
    //    //await _semaphore.WaitAsync();
    //    List<SyncResponse> list = new();
    //    foreach (var command in cList)
    //    {
    //        list.Add(new SyncResponse()
    //        {
    //            Id = command.Id,
    //            Command = command.Command
    //        });
    //    }
    //    return list;
    //}
    //catch
    //{
    //    return new();
    //}
    //finally
    //{
    //    cList.Clear();
    //    //_commands.Clear();
    //    //_semaphore.Release();
    //}
    //}
}
