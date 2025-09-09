using Skua.Core.Models;

namespace Skua.Core.Interfaces;

/// <summary>
/// Intercepts messages between the game client and server.
/// </summary>
public interface IInterceptor
{
    /// <summary>
    /// Indicates the priority of this interceptor. Interceptors with a lower priority value will be executed first.
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Intercepts/modifies a message.
    /// </summary>
    /// <param name="message">The message being intercepted.</param>
    /// <param name="outbound">Whether or not this packet is outbound (client -&gt; server).</param>
    /// <returns>The (un)modified message.</returns>
    void Intercept(MessageInfo message, bool outbound);
}