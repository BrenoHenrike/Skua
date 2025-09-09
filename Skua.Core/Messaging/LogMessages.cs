using Skua.Core.Models;

namespace Skua.Core.Messaging;
public sealed record LogsChangedMessage(LogType LogType);
public sealed record AddLogMessage(LogType LogType, string Text);
public sealed record SaveLogsMessage(IEnumerable<string> Logs);
public sealed record CopyLogsMessage(IEnumerable<string> Logs);