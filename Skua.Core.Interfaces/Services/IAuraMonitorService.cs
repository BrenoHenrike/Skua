using Skua.Core.Models.Auras;

namespace Skua.Core.Interfaces.Services;

/// <summary>
/// Interface for the aura monitoring service that tracks aura changes and fires events.
/// </summary>
public interface IAuraMonitorService : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the service is currently monitoring aura changes.
    /// </summary>
    bool IsMonitoring { get; }

    /// <summary>
    /// Gets the number of active event subscribers.
    /// </summary>
    int SubscriberCount { get; }

    /// <summary>
    /// Event fired when an aura is activated.
    /// Parameters: auraName, timeStarted, duration, stackValue, subject
    /// </summary>
    event Action<string, DateTime, int, int, SubjectType>? AuraActivated;

    /// <summary>
    /// Event fired when an aura is deactivated.
    /// Parameters: auraName, subject
    /// </summary>
    event Action<string, SubjectType>? AuraDeactivated;

    /// <summary>
    /// Event fired when an aura's stack value changes.
    /// Parameters: auraName, oldValue, newValue, subject
    /// </summary>
    event Action<string, int, int, SubjectType>? AuraStackChanged;

    /// <summary>
    /// Ensures monitoring is active if there are any event subscribers.
    /// </summary>
    /// <param name="pollIntervalMs">Polling interval in milliseconds (default 100ms).</param>
    void EnsureMonitoring(int pollIntervalMs = 100);

    /// <summary>
    /// Starts monitoring aura changes.
    /// </summary>
    /// <param name="pollIntervalMs">Polling interval in milliseconds (default 100ms).</param>
    void StartMonitoring(int pollIntervalMs = 100);

    /// <summary>
    /// Stops monitoring aura changes.
    /// </summary>
    void StopMonitoring();
}