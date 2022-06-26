using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Models;

namespace Skua.Core.ViewModels;
public class LogTabItemViewModel : ObservableObject, IDisposable
{
    public LogTabItemViewModel(string title, ILogService logService, IClipboardService clipBoard, IFileDialogService fileDialog, LogType logType)
    {
        Title = title;
        LogService = logService;
        _fileDialog = fileDialog;
        LogType = logType;
        _log = GetLog();
        LogService.PropertyChanged += LogService_PropertyChanged;
        CopyLogCommand = new RelayCommand(() => clipBoard.SetText(Log));
        ClearLogCommand = new RelayCommand(ClearLog);
        SaveLogCommand = new RelayCommand(Save);
    }

    // TODO Change to messenger
    private void LogService_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == $"{LogType}Logs")
        {
            Log = GetLog();
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        LogService.PropertyChanged -= LogService_PropertyChanged;
    }

    private readonly ILogService LogService;
    private readonly IFileDialogService _fileDialog;
    private readonly LogType LogType;

    public string Title { get; }
    private string _log;
    public string Log
    {
        get { return _log; }
        set { SetProperty(ref _log, value); }
    }

    public IRelayCommand ClearLogCommand { get; }
    public IRelayCommand CopyLogCommand { get; }
    public IRelayCommand SaveLogCommand { get; }

    private string GetLog()
    {
        return LogType switch
        {
            LogType.Debug => LogService.DebugLogs,
            LogType.Script => LogService.ScriptLogs,
            LogType.Flash => LogService.FlashLogs,
            _ => string.Empty,
        };
    }

    private void Save()
    {
        _fileDialog.SaveText(_log);
    }

    private void ClearLog()
    {
        LogService.ClearLog(LogType);
    }
}
