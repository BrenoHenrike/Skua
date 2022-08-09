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
    [ObservableProperty]
    private GrabberListViewModel _selectedTab;
}
