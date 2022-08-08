using Skua.Core.Interfaces;
using Skua.Core.ViewModels;

namespace Skua.Core.Messaging;
public sealed record PluginLoadedMessage(IPluginContainer Container);
public sealed record PluginUnloadedMessage(IPluginContainer Container);

public sealed record AddPluginMenuItemMessage(MainMenuItemViewModel ViewModel);
public sealed record RemovePluginMenuItemMessage(MainMenuItemViewModel ViewModel);
