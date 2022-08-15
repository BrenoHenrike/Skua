using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Skua.Core.ViewModels;
public partial class GrabberViewModel : BotControlViewModelBase
{
    public GrabberViewModel(IEnumerable<GrabberListViewModel> grabberTabs)
        : base("Grabber", 600, 450)
    {
        _grabberTabs = new(grabberTabs);
        _selectedTab = _grabberTabs[0];
    }

    [ObservableProperty]
    private ObservableCollection<GrabberListViewModel> _grabberTabs;

    private GrabberListViewModel _selectedTab;
    public GrabberListViewModel SelectedTab
    {
        get { return _selectedTab; }
        set
        {
            var lastTab = _selectedTab;
            if (SetProperty(ref _selectedTab, value))
            {
                lastTab.IsActive = false;
                _selectedTab.IsActive = true;
            }
        }
    }
}
