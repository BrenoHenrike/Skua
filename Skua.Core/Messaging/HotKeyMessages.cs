namespace Skua.Core.Messaging;
public record EditHotKeyMessage(string Title, string KeyGesture);
public record HotKeyErrorMessage(string Binding);
