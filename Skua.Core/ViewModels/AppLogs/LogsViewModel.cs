using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Skua.Core.ViewModels;

public partial class LogsViewModel : BotControlViewModelBase
{
    public LogsViewModel(IEnumerable<LogTabViewModel> logTabs)
        : base("Logs")
    {
        _logTabs = new(logTabs);
        _selectedTab = _logTabs[0];
    }

    [ObservableProperty]
    private ObservableCollection<LogTabViewModel> _logTabs;

    [ObservableProperty]
    private LogTabViewModel _selectedTab;
}