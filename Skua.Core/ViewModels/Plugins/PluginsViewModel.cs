using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels;
public partial class PluginsViewModel : BotControlViewModelBase
{
    public PluginsViewModel(IPluginManager pluginManager, IFileDialogService fileService)
        : base("Plugins")
    {
        Messenger.Register<PluginsViewModel, PluginLoadedMessage>(this, PluginLoaded);
        Messenger.Register<PluginsViewModel, PluginUnloadedMessage>(this, PluginUnLoaded);
        PluginManager = pluginManager;
        _fileService = fileService;
        LoadPluginCommand = new RelayCommand(LoadPlugin);
        UnloadAllPluginsCommand = new RelayCommand(UnloadAll);
        foreach(var container in PluginManager.Containers)
            _plugins.Add(new(container, container.OptionContainer.Options.Count > 0));
    }

    private void PluginUnLoaded(PluginsViewModel recipient, PluginUnloadedMessage message)
    {
        PluginItemViewModel? plugin = recipient.Plugins.First(p => p.Container.Plugin.Name == message.Container.Plugin.Name);
        if (plugin is null)
            return;

        recipient.Plugins.Remove(plugin);
    }

    private void PluginLoaded(PluginsViewModel recipient, PluginLoadedMessage message)
    {
        recipient.Plugins.Add(new(message.Container, message.Container.OptionContainer.Options.Count > 0));
    }

    private void UnloadAll()
    {
        PluginManager.Containers.ToList().ForEach(c => PluginManager.Unload(c.Plugin));
    }

    private void LoadPlugin()
    {
        string? file = _fileService.Open("DLL files |*.dll");

        if (string.IsNullOrEmpty(file))
            return;

        PluginManager.Load(file);
    }

    [ObservableProperty]
    private RangedObservableCollection<PluginItemViewModel> _plugins = new();
    private readonly IFileDialogService _fileService;

    public IPluginManager PluginManager { get; }

    public IRelayCommand LoadPluginCommand { get; }
    public IRelayCommand UnloadAllPluginsCommand { get; }
}
