namespace Skua.Core.Messaging;
public record LoadScriptMessage(string Path);

public record EditScriptMessage(string Path);

public record StartScriptMessage(string Path);
