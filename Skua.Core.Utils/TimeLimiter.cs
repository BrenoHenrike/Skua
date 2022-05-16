namespace Skua.Core.Utils;

public class TimeLimiter
{
    private Dictionary<string, int> _last = new();
    /// <summary>
    /// Runs an <see cref="Action"/> if the specified <paramref name="delay"/> has passed since its last invoke.
    /// </summary>
    /// <param name="name">Name of the action (must be unique for your action).</param>
    /// <param name="delay">Delay between the invoking of the <paramref name="action"/></param>
    /// <param name="action"><see cref="Action"/> to invoke.</param>
    /// <returns><see langword="true"/> if the action was invoked.</returns>
    public bool LimitedRun(string name, int delay, Action action)
    {
        bool run = !_last.TryGetValue(name, out int time) || Environment.TickCount - time >= delay;
        if (run)
        {
            action();
            _last[name] = Environment.TickCount;
        }
        return run;
    }
}
