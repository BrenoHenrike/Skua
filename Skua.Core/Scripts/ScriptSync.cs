using CommunityToolkit.Mvvm.DependencyInjection;
using Grpc.Core;
using Grpc.Net.Client;
using Newtonsoft.Json.Linq;
using Skua.Core.Interfaces;
using Skua.gRPC.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skua.Core.Scripts;
public sealed class ScriptSync : IScriptSync
{
    public ScriptSync(int id, IScriptInterface bot)
    {
        // TODO move channel
        var channel = GrpcChannel.ForAddress("http://localhost:5074");
        _client = new Synchronizer.SynchronizerClient(channel);
        _bot = bot;
        _id = id;
        //_syncTask = CallResponseHandler();

        _commands.Add("join", arguments =>
        {
            if (arguments.Length == 1)
                bot.Map.Join(arguments[0]);
            else if (arguments.Length == 3)
                bot.Map.Join(arguments[0], arguments[1], arguments[2]);
        });
        _commands.Add("jump", arguments =>
        {
            bot.Map.Jump(arguments[0], arguments[1]);
        });
        _commands.Add("kill", arguments =>
        {
            bot.Kill.Monster(arguments[0]);
        });
        _commands.Add("hunt", arguments =>
        {
            bot.Hunt.Monster(arguments[0]);
        });
        _commands.Add("move", arguments =>
        {
            bot.Player.WalkTo(int.Parse(arguments[0]), int.Parse(arguments[1]));
        });
    }

    private IScriptInterface _bot;
    private int _id;
    private Task _syncTask;
    private CancellationTokenSource _cts;
    private readonly Synchronizer.SynchronizerClient _client;
    private readonly Dictionary<string, Command> _commands = new();
    public void ClearCommands()
    {
        _commands.Clear();
    }

    public bool RegisterCommand(string name, Command action)
    {
        if(_commands.ContainsKey(name))
            return false;

        _commands[name] = action;
        return true;
    }

    public void OverrideCommand(string name, Command action)
    {
        if(_commands.ContainsKey(name))
        {
            _commands[name] = action;
            return;
        }

        _commands.Add(name, action);
    }

    public bool RemoveCommand(string name)
    {
        return _commands.Remove(name);
    }

    public void SendCommand(int id, string name, params string[] arguments)
    {
        var request = new SendCommandRequest
        {
            Id = id,
            Command = name
        };
        request.Arguments.AddRange(arguments);
        _client.SendCommand(request);
    }

    public string GetResponse(int id, string name, params string[] arguments)
    {
        return string.Empty;
    }

    public void SendCommandToAll(string name, params string[] arguments)
    {
        var request = new SendCommandRequest
        {
            Id = 0,
            Command = name
        };
        request.Arguments.AddRange(arguments);
        _client.SendCommand(request);
    }

    //private async Task CallResponseHandler()
    //{
    //    _cts = new();
    //    using var call = _client.StartSync(new SyncRequest()
    //    {
    //        Id = _id
    //    }, cancellationToken: _cts.Token);
    //    while (await call.ResponseStream.MoveNext(_cts.Token))
    //    {
    //        var current = call.ResponseStream.Current;
    //        if(_commands.TryGetValue(current.Command, out Command cmd))
    //        {
    //            cmd(current.Arguments.ToArray());
    //            //_client.
    //        }
    //    }
    //}
}
