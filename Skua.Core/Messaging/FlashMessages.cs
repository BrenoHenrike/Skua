using System.ComponentModel;

namespace Skua.Core.Messaging;

public sealed record FlashChangedMessage<T>(T Flash) where T : IComponent;

public sealed record FlashErrorMessage(Exception Exception, string Function, params object[] Args);