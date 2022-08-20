using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels;
public partial class LogTabViewModel : ObservableRecipient
{
    public LogTabViewModel(string title, ILogService logService, IDispatcherService dispatcherService, LogType logType)
    {
        Messenger.Register<LogTabViewModel, AddLogMessage>(this, AddLog);

        Title = title;
        _logService = logService;
        _dispatcherService = dispatcherService;
        _logType = logType;
    }

    private readonly ILogService _logService;
    private readonly IDispatcherService _dispatcherService;
    private readonly LogType _logType;

    public string Title { get; }
    public RangedObservableCollection<string> Logs { get; } = new();

    [RelayCommand]
    private void SaveLog()
    {
        Messenger.Send<SaveLogsMessage>(new(Logs));
    }

    [RelayCommand]
    private void ClearLog()
    {
        _logService.ClearLog(_logType);
        Logs.Clear();
    }

    [RelayCommand]
    private void CopyLog()
    {
        Messenger.Send<CopyLogsMessage>(new(Logs));
    }

    private void AddLog(LogTabViewModel recipient, AddLogMessage message)
    {
        if (message.LogType != recipient._logType)
            return;

        _dispatcherService.Invoke(() =>
        {
            recipient.Logs.Add(message.Text);
        });
    }
}
