using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace Skua.Core.ViewModels;
public class BotWindowViewModel : ObservableRecipient
{
    private BotControlViewModelBase _selectedItem;
    private int _selectedIndex;

    public BotWindowViewModel(IEnumerable<BotControlViewModelBase> views)
    {
        BotViews = new(views);
        HomeCommand = new RelayCommand(() => SelectedIndex = 0);
        MoveNextCommand = new RelayCommand(() => SelectedIndex++, () => SelectedIndex < BotViews.Count - 1);
        MovePrevCommand = new RelayCommand(() => SelectedIndex--, () => SelectedIndex > 0);
        _selectedItem = BotViews[0];
    }

    public BotControlViewModelBase SelectedItem
    {
        get { return _selectedItem; }
        set
        {
            SetProperty(ref _selectedItem, value);
            MoveNextCommand.NotifyCanExecuteChanged();
            MovePrevCommand.NotifyCanExecuteChanged();
        }
    }

    public int SelectedIndex
    {
        get { return _selectedIndex; }
        set
        {
            SetProperty(ref _selectedIndex, value);
            MoveNextCommand.NotifyCanExecuteChanged();
            MovePrevCommand.NotifyCanExecuteChanged();
        }
    }

    public ObservableCollection<BotControlViewModelBase> BotViews { get; set; }

    public IRelayCommand HomeCommand { get; }

    public IRelayCommand MovePrevCommand { get; }

    public IRelayCommand MoveNextCommand { get; }
}
