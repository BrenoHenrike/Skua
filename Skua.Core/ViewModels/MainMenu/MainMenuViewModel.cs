using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Skua.Core.Interfaces.Services;

namespace Skua.Core.ViewModels;
public class MainMenuViewModel : ObservableObject
{
    private readonly IWindowService _windowService;

    public MainMenuViewModel(IWindowService windowService)
    {
        _windowService = windowService;
        AddMenuItemCommand = new RelayCommand(AddMenuItem);
        MainMenuItems = new(new[] { new MainMenuItemViewModel(), new MainMenuItemViewModel() });
    }

    private ObservableCollection<MainMenuItemViewModel> _mainMenuItems = new();
    public ObservableCollection<MainMenuItemViewModel> MainMenuItems
    {
        get { return _mainMenuItems; }
        set { SetProperty(ref _mainMenuItems, value); }
    }

    public IRelayCommand AddMenuItemCommand { get; }

    public void AddMenuItem()
    {
        _windowService.ShowWindow<BotWindowViewModel>();
    }
}
