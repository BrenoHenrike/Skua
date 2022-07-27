using Skua.Core.Interfaces;
using Skua.Core.ViewModels;

namespace Skua.Core.Messaging;
public record PluginLoadedMessage(IPluginContainer Container);

public record PluginUnloadedMessage(IPluginContainer Container);

public record AddPluginMenuItemMessage(MainMenuItemViewModel ViewModel);
public record RemovePluginMenuItemMessage(MainMenuItemViewModel ViewModel);
