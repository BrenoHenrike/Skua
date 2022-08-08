using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace Skua.Core.ViewModels;
public class MainMenuViewModel : ObservableRecipient
{
    private readonly IWindowService _windowService;

    public MainMenuViewModel(IEnumerable<MainMenuItemViewModel> mainMenuItems, AutoViewModel auto, JumpViewModel jump, IWindowService windowService)
    {
        StrongReferenceMessenger.Default.Register<MainMenuViewModel, AddPluginMenuItemMessage, int>(this, (int)MessageChannels.Plugins, AddPluginMenuItem);
        StrongReferenceMessenger.Default.Register<MainMenuViewModel, RemovePluginMenuItemMessage, int>(this, (int)MessageChannels.Plugins, RemovePluginMenuItem);
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
