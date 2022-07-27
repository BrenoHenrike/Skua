using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.ViewModels;

namespace Skua.Core.Plugins;
public class PluginHelper : IPluginHelper
{
    private readonly IMessenger _messenger;

    private Dictionary<string, MainMenuItemViewModel> _buttons = new();
    public PluginHelper(IMessenger messenger)
    {
        _messenger = messenger;
    }
    public void AddMenuButton(string text, Action action)
    {
        if (_buttons.ContainsKey(text))
            return;

        var vm = new MainMenuItemViewModel(text, new RelayCommand(action));
        _buttons.Add(text, vm);
        _messenger.Send<AddPluginMenuItemMessage>(new(vm));
    }

    public void RemoveMenuButton(string text)
    {
        if (!_buttons.ContainsKey(text))
            return;

        _messenger.Send<RemovePluginMenuItemMessage>(new(_buttons[text]));
        _buttons.Remove(text);
    }
}
