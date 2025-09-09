using Skua.Core.Interfaces.Auras;
using Skua.Core.Interfaces.Services;
using Skua.Core.Models.Auras;
using System.Collections.Concurrent;

namespace Skua.Core.Services;

/// <summary>
/// Service that monitors aura changes and publishes events.
/// </summary>
public class AuraMonitorService : IAuraMonitorService
{
    private readonly IScriptSelfAuras _selfAuras;
    private readonly IScriptTargetAuras _targetAuras;
    private readonly Timer _pollTimer;
    private readonly ConcurrentDictionary<string, AuraState> _selfAuraStates = new();
    private readonly ConcurrentDictionary<string, AuraState> _targetAuraStates = new();
    private readonly object _lockObject = new();
    private bool _isMonitoring;
    private bool _disposed;

    /// <summary>
    /// Event fired when an aura is activated.
    /// </summary>
    public event Action<string, DateTime, int, int, SubjectType>? AuraActivated;

    /// <summary>
    /// Event fired when an aura is deactivated.
    /// </summary>
    public event Action<string, SubjectType>? AuraDeactivated;

    /// <summary>
    /// Event fired when an aura's stack value changes.
    /// </summary>
    public event Action<string, int, int, SubjectType>? AuraStackChanged;

    /// <summary>
    /// Gets a value indicating whether the service is currently monitoring aura changes.
    /// </summary>
    public bool IsMonitoring => _isMonitoring;

    /// <summary>
    /// Gets the number of active event subscribers.
    /// </summary>
    public int SubscriberCount
    {
        get
        {
            return (AuraActivated?.GetInvocationList().Length ?? 0) +
                   (AuraDeactivated?.GetInvocationList().Length ?? 0) +
                   (AuraStackChanged?.GetInvocationList().Length ?? 0);
        }
    }

    private class AuraState
    {
        public string Name { get; set; } = string.Empty;
        public int StackValue { get; set; }
        public DateTime TimeStarted { get; set; }
        public int DurationSeconds { get; set; }
        public bool IsActive { get; set; }
    }

    public AuraMonitorService(
        IScriptSelfAuras selfAuras,
        IScriptTargetAuras targetAuras)
    {
        _selfAuras = selfAuras;
        _targetAuras = targetAuras;
        _pollTimer = new Timer(PollAuras, null, Timeout.Infinite, Timeout.Infinite);
    }

    /// <summary>
    /// Ensures monitoring is active if there are any event subscribers.
    /// </summary>
    /// <param name="pollIntervalMs">Polling interval in milliseconds (default 100ms).</param>
    public void EnsureMonitoring(int pollIntervalMs = 100)
    {
        lock (_lockObject)
        {
            // Start monitoring if we have subscribers but aren't monitoring
            if (!_isMonitoring && SubscriberCount > 0)
            {
                _isMonitoring = true;
                _pollTimer.Change(0, pollIntervalMs);
            }
            // Stop monitoring if no subscribers
            else if (_isMonitoring && SubscriberCount == 0)
            {
                _isMonitoring = false;
                _pollTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _selfAuraStates.Clear();
                _targetAuraStates.Clear();
            }
        }
    }

    /// <summary>
    /// Starts monitoring aura changes.
    /// </summary>
    /// <param name="pollIntervalMs">Polling interval in milliseconds (default 100ms).</param>
    public void StartMonitoring(int pollIntervalMs = 100)
    {
        if (_isMonitoring) return;

        lock (_lockObject)
        {
            _isMonitoring = true;
            _pollTimer.Change(0, pollIntervalMs);
        }
    }

    /// <summary>
    /// Stops monitoring aura changes.
    /// </summary>
    public void StopMonitoring()
    {
        if (!_isMonitoring) return;

        lock (_lockObject)
        {
            _isMonitoring = false;
            _pollTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }

    private void PollAuras(object? state)
    {
        // Auto-stop monitoring if no subscribers
        if (SubscriberCount == 0)
        {
            StopMonitoring();
            return;
        }

        if (!_isMonitoring) return;

        try
        {
            // Monitor self auras
            CheckAuras(_selfAuras.Auras, _selfAuraStates, SubjectType.Self);

            // Monitor target auras
            CheckAuras(_targetAuras.Auras, _targetAuraStates, SubjectType.Target);
        }
        catch
        {
            // Silently handle polling errors to avoid spam
        }
    }

    private void CheckAuras(List<Aura>? currentAuras, ConcurrentDictionary<string, AuraState> stateDict, SubjectType subject)
    {
        if (currentAuras == null) return;

        var currentAuraNames = new HashSet<string>(currentAuras.Select(a => a.Name ?? string.Empty));

        // Check for new or changed auras
        foreach (var aura in currentAuras)
        {
            if (string.IsNullOrEmpty(aura.Name)) continue;

            var stackValue = aura.Value;

            if (stateDict.TryGetValue(aura.Name, out var existingState))
            {
                // Check if stack value changed
                if (existingState.StackValue != stackValue)
                {
                    var oldValue = existingState.StackValue;
                    existingState.StackValue = stackValue;

                    // Fire stack changed event
                    AuraStackChanged?.Invoke(aura.Name, oldValue, stackValue, subject);
                }
            }
            else
            {
                // New aura activated
                var newState = new AuraState
                {
                    Name = aura.Name,
                    StackValue = stackValue,
                    TimeStarted = aura.TimeStamp ?? DateTime.Now,
                    DurationSeconds = aura.Duration,
                    IsActive = true
                };

                stateDict[aura.Name] = newState;

                // Fire activation event
                AuraActivated?.Invoke(
                    aura.Name,
                    newState.TimeStarted,
                    newState.DurationSeconds,
                    stackValue,
                    subject
                );
            }
        }

        // Check for removed auras
        var keysToRemove = new List<string>();
        foreach (var kvp in stateDict)
        {
            if (!currentAuraNames.Contains(kvp.Key))
            {
                keysToRemove.Add(kvp.Key);
            }
        }

        foreach (var key in keysToRemove)
        {
            if (stateDict.TryRemove(key, out var removedState))
            {
                // Fire deactivation event
                AuraDeactivated?.Invoke(removedState.Name, subject);
            }
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        StopMonitoring();
        _pollTimer?.Dispose();
        _disposed = true;
    }
}