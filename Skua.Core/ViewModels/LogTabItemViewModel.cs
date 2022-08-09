using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models;

namespace Skua.Core.ViewModels;
public partial class LogTabViewModel : ObservableRecipient
{
    public LogTabViewModel(string title, ILogService logService, IClipboardService clipBoard, IFileDialogService fileDialog, LogType logType)
    {
        Messenger.Register<LogTabViewModel, LogsChangedMessage>(this, LogsChanged);

        Title = title;
        _logService = logService;
        _clipBoard = clipBoard;
        _fileDialog = fileDialog;
        _logType = logType;
    }

    private readonly ILogService _logService;
    private readonly IClipboardService _clipBoard;
    private readonly IFileDialogService _fileDialog;
    private readonly LogType _logType;

    public string Title { get; }
    public List<string> Logs => _logService.GetLogs(_logType);

    [RelayCommand]
    private void SaveLog()
    {
        _fileDialog.SaveText(LogsToString());
    }

    [RelayCommand]
    private void ClearLog()
    {
        _logService.ClearLog(_logType);
    }

    [RelayCommand]
    private void CopyLog()
    {
        _clipBoard.SetText(LogsToString());
    }

    private string LogsToString()
    {
        return string.Join(Environment.NewLine, Logs);
    }

    private void LogsChanged(LogTabViewModel recipient, LogsChangedMessage message)
    {
        if (message.LogType == recipient._logType)
            recipient.OnPropertyChanged(nameof(recipient.Logs));
    }
}
