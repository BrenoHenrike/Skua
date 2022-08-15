using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Skua.Core.ViewModels;
public partial class BotWindowViewModel : ObservableObject
{
    public BotWindowViewModel(IEnumerable<BotControlViewModelBase> views)
    {
        BotViews = new(views);
        _selectedItem = BotViews[0];
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(MoveNextCommand), nameof(MovePrevCommand))]
    private int _selectedIndex;

    private BotControlViewModelBase _selectedItem;
    public BotControlViewModelBase SelectedItem
    {
        get { return _selectedItem; }
        set
        {
            var lastView = _selectedItem;
            if (SetProperty(ref _selectedItem, value))
            {
                lastView.IsActive = false;
                _selectedItem.IsActive = true;
                
            }
        }
    }

    public ObservableCollection<BotControlViewModelBase> BotViews { get; set; }

    [RelayCommand]
    private void Home()
    {
        SelectedIndex = 0;
    }

    [RelayCommand(CanExecute = nameof(CanMoveNext))]
    private void MoveNext()
    {
        SelectedIndex++;
    }
    private bool CanMoveNext()
    {
        return SelectedIndex < BotViews.Count - 1;
    }

    [RelayCommand(CanExecute = nameof(CanMovePrev))]
    private void MovePrev()
    {
        SelectedIndex--;
    }
    private bool CanMovePrev()
    {
        return SelectedIndex > 0;
    }
}
