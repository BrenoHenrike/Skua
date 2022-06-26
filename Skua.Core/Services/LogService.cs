using System.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Skua.Core.Interfaces;
using Skua.Core.Models;

namespace Skua.Core.Services;
public class LogService : ObservableObject, ILogService, IDisposable
{
    private readonly IFlashUtil _flash;

    public LogService(IFlashUtil flash)
    {
        Trace.Listeners.Add(new DebugListener(this));
        _flash = flash;
        _flash.FlashError += Flash_FlashError;
    }

    private void Flash_FlashError(Exception e, string function, params object[] args)
    {
        FlashLogs += $"{function} Args[{args.Length}] {(args.Length > 0 ? $"= {{{string.Join(",", args.Select(a => a.ToString()))}" : string.Empty)}}}\r\n";
    }

    public string DebugLogs { get; private set; } = string.Empty;
    public string ScriptLogs { get; private set; } = string.Empty;
    public string FlashLogs { get; private set; } = string.Empty;

    public void DebugLog(string message)
    {
        DebugLogs += message;
        OnPropertyChanged(nameof(DebugLogs));
    }

    public void FlashLog(string? message)
    {
        if (message is null)
            return;
        FlashLogs += message;
        OnPropertyChanged(nameof(FlashLogs));
    }

    public void ScriptLog(string? message)
    {
        if (message is null)
            return;
        ScriptLogs += message;
        OnPropertyChanged(nameof(ScriptLogs));
    }

    public void ClearLog(LogType logType)
    {
        switch (logType)
        {
            case LogType.Debug:
                DebugLogs = string.Empty;
                OnPropertyChanged(nameof(DebugLogs));
                break;
            case LogType.Script:
                ScriptLogs = string.Empty;
                OnPropertyChanged(nameof(ScriptLogs));
                break;
            case LogType.Flash:
                FlashLogs = string.Empty;
                OnPropertyChanged(nameof(FlashLogs));
                break;
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _flash.FlashError += Flash_FlashError;
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
        _logService.DebugLog($"{message}\r\n");
    }
}
