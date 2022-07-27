using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

namespace Skua.Core.ViewModels;
public class MainMenuViewModel : ObservableRecipient
{
    private readonly IWindowService _windowService;

    public MainMenuViewModel(IEnumerable<MainMenuItemViewModel> mainMenuItems, AutoViewModel auto, JumpViewModel jump, IWindowService windowService)
    {
        Messenger.Register<MainMenuViewModel, AddPluginMenuItemMessage>(this, AddPluginMenuItem);
        Messenger.Register<MainMenuViewModel, RemovePluginMenuItemMessage>(this, RemovePluginMenuItem);
        AutoViewModel = auto;
        JumpViewModel = jump;
        _windowService = windowService;
        ShowBotWindowCommand = new RelayCommand(ShowBotWindow);

        _plugins = new(new[] { new MainMenuItemViewModel("View Plugins", new RelayCommand(ShowPlugins)) });

        MainMenuItems = new(mainMenuItems);
    }

    private ObservableCollection<MainMenuItemViewModel> _mainMenuItems = new();
    public ObservableCollection<MainMenuItemViewModel> MainMenuItems
    {
        get { return _mainMenuItems; }
        set { SetProperty(ref _mainMenuItems, value); }
    }
    private ObservableCollection<MainMenuItemViewModel> _plugins;
    public ObservableCollection<MainMenuItemViewModel> Plugins
    {
        get { return _plugins; }
        set { SetProperty(ref _plugins, value); }
    }

    public AutoViewModel AutoViewModel { get; }
    public JumpViewModel JumpViewModel { get; }

    public IRelayCommand ShowBotWindowCommand { get; }

    public void ShowBotWindow()
    {
        _windowService.ShowWindow<BotWindowViewModel>();
    }

    private void ShowPlugins()
    {
        _windowService.ShowManagedWindow("Plugins");
    }

    private void AddPluginMenuItem(MainMenuViewModel recipient, AddPluginMenuItemMessage message)
    {
        recipient.Plugins.Add(message.ViewModel);
    }

    private void RemovePluginMenuItem(MainMenuViewModel recipient, RemovePluginMenuItemMessage message)
    {
        recipient.Plugins.Remove(message.ViewModel);
    }
}
