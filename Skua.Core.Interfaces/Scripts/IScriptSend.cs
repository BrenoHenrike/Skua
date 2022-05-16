namespace Skua.Core.Interfaces;

public interface IScriptSend
{
    /// <summary>
    /// Sends the specified packet to the server.
    /// </summary>
    /// <param name="packet">Packet to be sent.</param>
    /// <param name="type">Type of the packet being sent (String, Json).</param>
    /// <remarks>Be careful when using this method. Incorrect use of this method may cause you to be kicked (or banned, although very unlikely).</remarks>
    void Packet(string packet, string type = "String");
    /// <summary>
    /// Sends the specified packet to the client (simulates a response as if the server sent the packet).
    /// </summary>
    /// <param name="packet">Packet to be sent.</param>
    /// <param name="type">Type of the packet (xml, json, str).</param>
    void ClientPacket(string packet, string type = "str");
    /// <summary>
    /// Sends a client side packet message with gray color surrounded by asterisks (*) (event messages).
    /// </summary>
    /// <param name="message">Message to be sent.</param>
    /// <param name="sentBy">Name of who sent the message.</param>
    void ClientEvent(string message, string sentBy = "Skua");
    /// <summary>
    /// Sends a client side packet message with green color (guild messages).
    /// </summary>
    /// <param name="message">Message to be sent.</param>
    /// <param name="sentBy">Name of who sent the message.</param>
    void ClientGuild(string message, string sentBy = "Skua");
    /// <summary>
    /// Sends a client side packet message with golden color (moderator messages).
    /// </summary>
    /// <param name="message">Message to be sent.</param>
    /// <param name="sentBy">Name of who sent the message.</param>
    void ClientModerator(string message, string sentBy = "Skua");
    /// <summary>
    /// Sends a client side packet message with blue color (server messages).
    /// </summary>
    /// <param name="message">Message to be sent.</param>
    /// <param name="sentBy">Name of who sent the message.</param>
    void ClientServer(string message, string sentBy = "Skua");
    /// <summary>
    /// Sends a client side packet message with red color (warning messages).
    /// </summary>
    /// <param name="message">Message to be sent.</param>
    /// <param name="sentBy">Name of who sent the message.</param>
    void ClientWarning(string message, string sentBy = "Skua");
    /// <summary>
    /// Sends a client side packet message with purple color (whiper messages).
    /// </summary>
    /// <param name="message">Message to be sent.</param>
    /// <param name="sentBy">Name of who sent the message.</param>
    void ClientWhisper(string message, string sentBy = "Skua");
    /// <summary>
    /// Sends a client side packet message with default chat color.
    /// </summary>
    /// <param name="message">Message to be sent.</param>
    /// <param name="sentBy">Name of who sent the message.</param>
    void ClientZone(string message, string sentBy = "Skua");
    /// <summary>
    /// Sends a whisper to a player.
    /// </summary>
    /// <param name="name">Name of the player</param>
    /// <param name="message">Message to send to the player</param>
    /// <remarks>Be careful when using this method. This Whisper is not client sided and can be tracked.</remarks>
    void Whisper(string name, string message);
    /// <summary>
    /// Keeps sending a client side packet until the given token is cancelled.
    /// </summary>
    /// <param name="packet">Packet to spam.</param>
    /// <param name="type">Type of the packet. This can be xml, json or str.</param>
    /// <param name="delay">Delay between packets.</param>
    /// <param name="token">Cancellation token to be assigned.</param>
    /// <returns>A <see cref="Task"/> representing the packet spammer.</returns>
    Task ClientPacketSpam(string packet, string type, int delay, CancellationToken token);
    /// <summary>
    /// Keeps sending a packet until the given token is cancelled.
    /// </summary>
    /// <param name="packet">Packet to spam.</param>
    /// <param name="type">Type of the packet being sent (String, Json). The default is string.</param>
    /// <param name="delay">Delay between packets.</param>
    /// <param name="token">Cancellation token to be assigned.</param>
    /// <returns>A <see cref="Task"/> representing the packet spammer.</returns>
    Task PacketSpam(string packet, string type, int delay, CancellationToken token);
}
