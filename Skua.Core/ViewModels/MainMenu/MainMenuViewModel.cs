using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using System.Collections.ObjectModel;

namespace Skua.Core.ViewModels;

public partial class MainMenuViewModel : ObservableRecipient
{
    public MainMenuViewModel(IEnumerable<MainMenuItemViewModel> mainMenuItems, AutoViewModel auto, JumpViewModel jump, IWindowService windowService)
    {
        StrongReferenceMessenger.Default.Register<MainMenuViewModel, AddPluginMenuItemMessage, int>(this, (int)MessageChannels.Plugins, AddPluginMenuItem);
        StrongReferenceMessenger.Default.Register<MainMenuViewModel, RemovePluginMenuItemMessage, int>(this, (int)MessageChannels.Plugins, RemovePluginMenuItem);

        AutoViewModel = auto;
        JumpViewModel = jump;
        _windowService = windowService;

        _plugins = new(new[] { new MainMenuItemViewModel("View Plugins", new RelayCommand(ShowPlugins)) });

        MainMenuItems = new(mainMenuItems);
    }

    private readonly IWindowService _windowService;

    [ObservableProperty]
    private ObservableCollection<MainMenuItemViewModel> _mainMenuItems = new();

    [ObservableProperty]
    private ObservableCollection<MainMenuItemViewModel> _plugins;

    public AutoViewModel AutoViewModel { get; }
    public JumpViewModel JumpViewModel { get; }

    [RelayCommand]
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