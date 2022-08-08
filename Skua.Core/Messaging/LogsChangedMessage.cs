using Skua.Core.Models;

namespace Skua.Core.Messaging;
public sealed record LogsChangedMessage(LogType LogType);
