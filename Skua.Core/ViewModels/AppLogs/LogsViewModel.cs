using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;

namespace Skua.Core.ViewModels;
public partial class LogsViewModel : BotControlViewModelBase
{
    public LogsViewModel(IEnumerable<LogTabViewModel> logTabs, IClipboardService clipboardService, IFileDialogService fileDialogService)
        : base("Logs")
    {
        Messenger.Register<LogsViewModel, SaveLogsMessage>(this, SaveLogs);
        Messenger.Register<LogsViewModel, CopyLogsMessage>(this, CopyLogs);

        _logTabs = new(logTabs);
        _selectedTab = _logTabs[0];
        _clipboard = clipboardService;
        _fileDialog = fileDialogService;
    }

    private void CopyLogs(LogsViewModel recipient, CopyLogsMessage message)
    {
        recipient._clipboard.SetText(recipient.LogsToString(message.Logs));
    }

    private void SaveLogs(LogsViewModel recipient, SaveLogsMessage message)
    {
        recipient._fileDialog.SaveText(recipient.LogsToString(message.Logs));
    }

    [ObservableProperty]
    private ObservableCollection<LogTabViewModel> _logTabs;
    [ObservableProperty]
    private LogTabViewModel _selectedTab;
    private readonly IClipboardService _clipboard;
    private readonly IFileDialogService _fileDialog;

    private string LogsToString(IEnumerable<string> logs)
    {
        return string.Join(Environment.NewLine, logs);
    }
}
