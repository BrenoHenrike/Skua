using System.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Skua.Core.Interfaces;

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
    private Task _taskTimer;
    private CancellationTokenSource _ctsTimer;

    [ObservableProperty]
    private int _Kills;
    [ObservableProperty]
    private int _QuestsAccepted;
    [ObservableProperty]
    private int _QuestsCompleted;
    [ObservableProperty]
    private int _Deaths;
    [ObservableProperty]
    private int _Relogins;
    [ObservableProperty]
    private int _Drops;
    [ObservableProperty]
    private TimeSpan _Time;

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
