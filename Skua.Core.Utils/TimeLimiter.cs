namespace Skua.Core.Utils;

public class TimeLimiter
{
    private Dictionary<string, int> _last = new();
    private const int MaxCacheSize = 1000; // Reasonable limit for cache size
    private const int CleanupThreshold = 5 * 60 * 1000; // 5 minutes in milliseconds

    /// <summary>
    /// Runs an <see cref="Action"/> if the specified <paramref name="delay"/> has passed since its last invoke.
    /// </summary>
    /// <param name="name">Name of the action (must be unique for your action).</param>
    /// <param name="delay">Delay between the invoking of the <paramref name="action"/></param>
    /// <param name="action"><see cref="Action"/> to invoke.</param>
    /// <returns><see langword="true"/> if the action was invoked.</returns>
    public bool LimitedRun(string name, int delay, Action action)
    {
        // Perform periodic cleanup to prevent unbounded growth
        if (_last.Count > MaxCacheSize)
        {
            CleanupOldEntries();
        }

        bool run = !_last.TryGetValue(name, out int time) || Environment.TickCount - time >= delay;
        if (run)
        {
            action();
            _last[name] = Environment.TickCount;
        }
        return run;
    }

    /// <summary>
    /// Removes entries older than the cleanup threshold to prevent memory growth.
    /// </summary>
    private void CleanupOldEntries()
    {
        int currentTick = Environment.TickCount;
        var keysToRemove = new List<string>();

        foreach (var kvp in _last)
        {
            // Remove entries older than cleanup threshold
            if (currentTick - kvp.Value > CleanupThreshold)
            {
                keysToRemove.Add(kvp.Key);
            }
        }

        foreach (string key in keysToRemove)
        {
            _last.Remove(key);
        }
    }
}