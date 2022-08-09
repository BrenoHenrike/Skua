using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Skua.Core.ViewModels;
public partial class BotWindowViewModel : ObservableRecipient
{
    public BotWindowViewModel(IEnumerable<BotControlViewModelBase> views)
    {
        BotViews = new(views);
        HomeCommand = new RelayCommand(() => SelectedIndex = 0);
        MoveNextCommand = new RelayCommand(() => SelectedIndex++, () => SelectedIndex < BotViews.Count - 1);
        MovePrevCommand = new RelayCommand(() => SelectedIndex--, () => SelectedIndex > 0);
        _selectedItem = BotViews[0];
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(MoveNextCommand), nameof(MovePrevCommand))]
    private BotControlViewModelBase _selectedItem;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(MoveNextCommand), nameof(MovePrevCommand))]
    private int _selectedIndex;

    public ObservableCollection<BotControlViewModelBase> BotViews { get; set; }
    public IRelayCommand HomeCommand { get; }
    public IRelayCommand MovePrevCommand { get; }
    public IRelayCommand MoveNextCommand { get; }
}
