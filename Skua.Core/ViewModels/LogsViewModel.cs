using System.Collections.ObjectModel;
using Skua.Core.Interfaces;
using Skua.Core.Models;

namespace Skua.Core.ViewModels;
public class LogsViewModel : BotControlViewModelBase
{
    private ObservableCollection<LogTabItemViewModel> _logTabs;
    public ObservableCollection<LogTabItemViewModel> LogTabs
    {
        get { return _logTabs; }
        set { SetProperty(ref _logTabs, value); }
    }

    private LogTabItemViewModel _selectedTab;
    public LogTabItemViewModel SelectedTab
    {
        get { return _selectedTab; }
        set { SetProperty(ref _selectedTab, value); }
    }

    public LogsViewModel(ILogService logService, IClipboardService clipBoard, IEnumerable<LogTabItemViewModel> logTabs)
        : base("Logs")
    {
        _logTabs = new(logTabs);
        _selectedTab = _logTabs[0];
    }
}
