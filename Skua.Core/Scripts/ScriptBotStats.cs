using CommunityToolkit.Mvvm.ComponentModel;
using Skua.Core.Interfaces;
using System.Diagnostics;

namespace Skua.Core.Scripts;

public partial class ScriptBotStats : ObservableObject, IScriptBotStats, IAsyncDisposable
{
    public ScriptBotStats()
    {
        _sw = Stopwatch.StartNew();
        _ctsTimer = new();
        _timer = new(TimeSpan.FromMilliseconds(1000));
        _taskTimer = HandleTimerAsync(_timer, _ctsTimer.Token);
    }

    private readonly Stopwatch _sw;
    private readonly PeriodicTimer _timer;
    private readonly Task _taskTimer;
    private readonly CancellationTokenSource _ctsTimer;

    [ObservableProperty]
    private int _kills;

    [ObservableProperty]
    private int _questsAccepted;

    [ObservableProperty]
    private int _questsCompleted;

    [ObservableProperty]
    private int _deaths;

    [ObservableProperty]
    private int _relogins;

    [ObservableProperty]
    private int _drops;

    [ObservableProperty]
    private TimeSpan _time;

    public void Reset()
    {
        Kills = 0;
        QuestsAccepted = 0;
        QuestsCompleted = 0;
        Deaths = 0;
        Relogins = 0;
        Drops = 0;
        _sw.Restart();
    }

    private async Task HandleTimerAsync(PeriodicTimer timer, CancellationToken token)
    {
        try
        {
            while (await timer.WaitForNextTickAsync(token))
                Time = _sw.Elapsed;
        }
        catch { }
    }

    public async ValueTask DisposeAsync()
    {
        _ctsTimer.Cancel();
        await _taskTimer;
        _timer.Dispose();
        _ctsTimer?.Dispose();
        GC.SuppressFinalize(this);
    }
}