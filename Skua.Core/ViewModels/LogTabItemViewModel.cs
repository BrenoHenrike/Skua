using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models;

namespace Skua.Core.ViewModels;
public partial class LogTabViewModel : ObservableRecipient
{
    public LogTabViewModel(string title, ILogService logService, IClipboardService clipBoard, IFileDialogService fileDialog, LogType logType)
    {
        Messenger.Register<LogTabViewModel, LogsChangedMessage>(this, Receive);
        Title = title;
        _logService = logService;
        _fileDialog = fileDialog;
        _logType = logType;
        CopyLogCommand = new RelayCommand(() => clipBoard.SetText(LogsToString()));
        ClearLogCommand = new RelayCommand(ClearLog);
        SaveLogCommand = new RelayCommand(Save);
    }

    private readonly ILogService _logService;
    private readonly IFileDialogService _fileDialog;
    private readonly LogType _logType;

    public string Title { get; }
    public List<string> Logs => _logService.GetLogs(_logType);

    public IRelayCommand ClearLogCommand { get; }
    public IRelayCommand CopyLogCommand { get; }
    public IRelayCommand SaveLogCommand { get; }

    private void Save()
    {
        _fileDialog.SaveText(LogsToString());
    }

    private void ClearLog()
    {
        _logService.ClearLog(_logType);
    }

    private string LogsToString()
    {
        return string.Join(Environment.NewLine, Logs);
    }

    private void Receive(LogTabViewModel recipient, LogsChangedMessage message)
    {
        if (message.LogType == recipient._logType)
            recipient.OnPropertyChanged(nameof(recipient.Logs));
    }
}
