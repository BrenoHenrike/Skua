using System.ComponentModel;

namespace Skua.Core.Messaging;

public record FlashChangedMessage<T>(T Flash) where T : IComponent;

public record FlashErrorMessage(Exception Exception, string Function, params object[] Args);
