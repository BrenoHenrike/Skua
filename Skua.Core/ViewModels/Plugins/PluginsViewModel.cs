using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels;
public partial class PluginsViewModel : BotControlViewModelBase
{
    public PluginsViewModel(IPluginManager pluginManager, IFileDialogService fileService)
        : base("Plugins")
    {
        PluginManager = pluginManager;
        _fileService = fileService;
    }

    protected override void OnActivated()
    {
        StrongReferenceMessenger.Default.Register<PluginsViewModel, PluginLoadedMessage, int>(this, (int)MessageChannels.Plugins, PluginLoaded);
        StrongReferenceMessenger.Default.Register<PluginsViewModel, PluginUnloadedMessage, int>(this, (int)MessageChannels.Plugins, PluginUnLoaded);

        _plugins.Clear();
        foreach (var container in PluginManager.Containers)
            _plugins.Add(new(container, container.OptionContainer.Options.Count > 0));
    }

    protected override void OnDeactivated()
    {
        StrongReferenceMessenger.Default.UnregisterAll(this);
    }

    private readonly IFileDialogService _fileService;
    [ObservableProperty]
    private RangedObservableCollection<PluginItemViewModel> _plugins = new();

    public IPluginManager PluginManager { get; }

    [RelayCommand]
    private void UnloadAllPlugins()
    {
        PluginManager.Containers.ToList().ForEach(c => PluginManager.Unload(c.Plugin));
    }

    [RelayCommand]
    private void LoadPlugin()
    {
        string? file = _fileService.OpenFile("DLL files |*.dll");

        if (string.IsNullOrEmpty(file))
            return;

        PluginManager.Load(file);
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
}
