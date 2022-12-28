using Skua.Core.Interfaces;

namespace Skua.gRPC.Server;

public class FollowerHandler
{
    private readonly SemaphoreSlim _semaphore = new(1);
    private List<string> _commands = new();

    public async Task ReceiveCommand(string command)
    {
        await _semaphore.WaitAsync();
        _commands.Add(command);
        _semaphore.Release();
    }

    public async Task<List<FollowerResponse>> ProcessCommand()
    {
        while (_commands.Count <= 0)
            await Task.Delay(1000);

        try
        {
            await _semaphore.WaitAsync();
            List<FollowerResponse> list = new();
            foreach (var command in _commands)
            {
                list.Add(new FollowerResponse()
                {
                    Command = command
                });
            }
            return list;
        }
        catch
        {
            return new();
        }
        finally
        {
            _commands.Clear();
            _semaphore.Release();
        }
    }
}
