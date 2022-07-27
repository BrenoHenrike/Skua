using Skua.Core.Interfaces;
using System.Collections.ObjectModel;

namespace Skua.Core.ViewModels;
public class GrabberViewModel : BotControlViewModelBase
{
    public GrabberViewModel(IEnumerable<GrabberListViewModel> grabberTabs)
        : base("Grabber", 600, 450)
    {
        _grabberTabs = new(grabberTabs);
        _selectedTab = _grabberTabs[0];
    }
    private ObservableCollection<GrabberListViewModel> _grabberTabs;
    public ObservableCollection<GrabberListViewModel> GrabberTabs
    {
        get { return _grabberTabs; }
        set { SetProperty(ref _grabberTabs, value); }
    }
    private GrabberListViewModel _selectedTab;
    public GrabberListViewModel SelectedTab
    {
        get { return _selectedTab; }
        set { SetProperty(ref _selectedTab, value); }
    }
}
