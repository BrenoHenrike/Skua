using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using Skua.Core.Interfaces;

namespace Skua.App.Services;
public class DropService : IDropService
{
    public DropService(IScriptOption options, IScriptDrop drops)
    {
        Options = options;
        Drops = drops;
        timer = new(TimeSpan.FromMilliseconds(3000), DispatcherPriority.Normal, Tick, Dispatcher.CurrentDispatcher);
        timer.Stop();
    }
    private readonly IScriptOption Options;
    private readonly IScriptDrop Drops;
    private readonly DispatcherTimer timer;
    private bool AcceptAllDrops;
    private bool RejectAllDrops;

    private void Tick(object? sender, EventArgs e)
    {
        if(AcceptAllDrops)
        {
            Task.Run(() => Drops.PickupAll());
        }
        else if(RejectAllDrops)
        {
            Task.Run(() => Drops.RejectAll());
        }
    }

    public void ToggleAcceptAllDrops(bool value)
    {
        AcceptAllDrops = value;
        if (AcceptAllDrops)
            RejectAllDrops = false;
        ToggleTimer();
    }

    public void ToggleRejectAllDrops(bool value)
    {
        RejectAllDrops = value;
        if (RejectAllDrops)
            AcceptAllDrops = false;
        ToggleTimer();
    }

    private void ToggleTimer()
    {
        if (!timer.IsEnabled && (AcceptAllDrops || RejectAllDrops))
            timer.Start();
        if(!AcceptAllDrops && !RejectAllDrops)
            timer.Stop();
    }
}
