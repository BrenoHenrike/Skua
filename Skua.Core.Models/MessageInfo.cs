namespace Skua.Core.Models;

/// <summary>
/// A class holding information about a packet.
/// </summary>
public class MessageInfo
{
    /// <summary>
    /// The content of the packet being intercepted. This can be modified.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Whether or not to pass the packet through the proxy.
    /// </summary>
    public bool Send { get; set; } = true;

    /// <summary>
    /// Creates a new instance of the MessageInfo class with the given content.
    /// </summary>
    /// <param name="content">The content of this message.</param>
    public MessageInfo(string content)
    {
        Content = content;
    }
}

