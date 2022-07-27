using System.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models;

namespace Skua.Core.Services;
public class LogService : ObservableRecipient, ILogService
{
    public LogService(IFlashUtil flash)
    {
        Trace.Listeners.Add(new DebugListener(this));
        Messenger.Register<LogService, FlashErrorMessage>(this, LogFlashError);
    }

    private void LogFlashError(LogService recipient, FlashErrorMessage message)
    {
        recipient.FlashLog($"{message.Function} Args[{message.Args.Length}] {(message.Args.Length > 0 ? $"= {{{string.Join(",", message.Args.Select(a => a?.ToString()))}}}" : string.Empty)}");
    }

    private List<string> _debugLogs = new();
    private List<string> _scriptLogs = new();
    private List<string> _flashLogs = new();

    public void DebugLog(string message)
    {
        _debugLogs.Add(message);
        Messenger.Send(new LogsChangedMessage(LogType.Debug));
    }

    public void FlashLog(string? message)
    {
        if (message is null)
            return;
        _flashLogs.Add(message);
        Messenger.Send(new LogsChangedMessage(LogType.Flash));
    }

    public void ScriptLog(string? message)
    {
        if (message is null)
            return;
        _scriptLogs.Add(message);
        Messenger.Send(new LogsChangedMessage(LogType.Script));
    }

    public void ClearLog(LogType logType)
    {
        switch (logType)
        {
            case LogType.Debug:
                _debugLogs.Clear();
                break;
            case LogType.Script:
                _scriptLogs.Clear();
                break;
            case LogType.Flash:
                _flashLogs.Clear();
                break;
        }
        Messenger.Send(new LogsChangedMessage(logType));
    }

    public List<string> GetLogs(LogType logType)
    {
        List<string> logs = new();
        switch (logType)
        {
            case LogType.Debug:
                logs = _debugLogs.ToList();
                break;
            case LogType.Script:
                logs = _scriptLogs.ToList();
                break;
            case LogType.Flash:
                logs = _flashLogs.ToList();
                break;
        }
        return logs;
    }
}

public class DebugListener : TraceListener
{
    private readonly ILogService _logService;

    public DebugListener(ILogService logService)
    {
        _logService = logService;
    }

    public override void Write(string? message)
    {
        if (message is null)
            return;
        _logService.DebugLog(message);
    }

    public override void WriteLine(string? message)
    {
        if (message is null)
            return;
        _logService.DebugLog(message);
    }
}
