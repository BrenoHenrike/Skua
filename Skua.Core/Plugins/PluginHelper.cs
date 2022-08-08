using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.ViewModels;

namespace Skua.Core.Plugins;
public class PluginHelper : IPluginHelper
{
    private readonly IMessenger _messenger;

    private Dictionary<string, MainMenuItemViewModel> _buttons = new();
    public PluginHelper()
    {
        _messenger = StrongReferenceMessenger.Default;
    }
    public void AddMenuButton(string text, Action action)
    {
        if (_buttons.ContainsKey(text))
            return;

        var vm = new MainMenuItemViewModel(text, new RelayCommand(action));
        _buttons.Add(text, vm);
        _messenger.Send<AddPluginMenuItemMessage, int>(new(vm), (int)MessageChannels.Plugins);
    }

    public void RemoveMenuButton(string text)
    {
        if (!_buttons.ContainsKey(text))
            return;

        _messenger.Send<RemovePluginMenuItemMessage, int>(new(_buttons[text]), (int)MessageChannels.Plugins);
        _buttons.Remove(text);
    }
}
