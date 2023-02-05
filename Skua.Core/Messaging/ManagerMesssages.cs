using CommunityToolkit.Mvvm.Messaging.Messages;
using Skua.Core.Models.GitHub;
using Skua.Core.ViewModels;

namespace Skua.Core.Messaging;
public sealed record CheckClientUpdateMessage();
public sealed record DownloadClientUpdateMessage(UpdateInfo UpdateInfo);
public sealed record UpdateScriptsMessage(bool Reset);
public sealed class UpdateStartedMessage : AsyncRequestMessage<bool> { }
public sealed record UpdateFinishedMessage();
public sealed record ClearPasswordBoxMessage();
public sealed record RemoveAccountMessage(AccountItemViewModel Account);
