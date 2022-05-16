using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Utils;

namespace Skua.Core.Scripts;

public class ScriptDrop : ScriptableObject, IScriptDrop
{
    private CancellationTokenSource? DropsCTS;
    private Thread? DropsThread;
    private readonly List<string> _addName = new();
    private readonly List<string> _remName = new();
    private readonly List<int> _addID = new();
    private readonly List<int> _remID = new();

    public event Action? DropsStarted;
    public event Action? DropsStopped;

    public int Interval { get; set; } = 1000;
    public List<string> ToPickup { get; } = new();
    public List<int> ToPickupIDs { get; } = new();
    public bool RejectElse { get; set; }
    public bool Enabled => DropsThread?.IsAlive ?? false;
    public List<InventoryItem> CurrentDropInfos { get; private set; } = new();
    public List<string> CurrentDrops => CurrentDropInfos.Select(x => x.Name.Trim()).ToList();

    public void Pickup(string name)
    {
        if (!CurrentDrops.Contains(name, StringComparer.OrdinalIgnoreCase))
            return;

        InventoryItem drop = CurrentDropInfos.Find(d => d.Name == name)!;
        Bot.Send.Packet($"%xt%zm%getDrop%{Bot.Map.RoomID}%{drop.ID}%");
        CurrentDropInfos.Remove(drop);
    }

    public void Pickup(int id)
    {
        if (!CurrentDropInfos.Contains(i => i.ID == id))
            return;

        Bot.Send.Packet($"%xt%zm%getDrop%{Bot.Map.RoomID}%{id}%");
        CurrentDropInfos.Remove(CurrentDropInfos.SingleOrDefault(d => d.ID == id)!);
    }

    public void Pickup(params string[] names)
    {
        for(int i = 0; i < names.Length; i++)
        {
            Pickup(names[i]);
            if (Bot.Options.SafeTimings)
                Bot.Wait.ForPickup(names[i]);
        }
    }

    public void Pickup(params int[] ids)
    {
        for(int i = 0; i < ids.Length; i++)
        {
            Pickup(ids[i]);
            if (Bot.Options.SafeTimings)
                Bot.Wait.ForPickup(ids[i]);
        }
    }

    public void PickupACItems()
    {
        Pickup(CurrentDropInfos.Where(d => d.Coins).Select(d => d.Name).ToArray());
    }

    public void PickupAll(bool skipWait = false)
    {
        CurrentDropInfos.ToList().ForEach(d => Pickup(d.Name));
        CurrentDropInfos.Clear();
        if (Bot.Options.SafeTimings && !skipWait)
            Bot.Wait.ForPickup("*");
    }

    public void RejectExcept(params string[] names)
    {
        if (Bot.Options.AcceptACDrops)
            PickupACItems();
        Bot.Flash.Call("rejectExcept", names.Join(',').ToLower());
        if (Bot.Options.SafeTimings)
            Bot.Wait.ForPickup("*");
    }

    public void RejectExcept(params int[] ids)
    {
        if (Bot.Options.AcceptACDrops)
            PickupACItems();
        var items = CurrentDropInfos.Where(d => ids.Contains(d.ID)).Select(d => d.Name);
        Bot.Flash.Call("rejectExcept", items.Join(',').ToLower());
        if (Bot.Options.SafeTimings)
            Bot.Wait.ForPickup("*");
    }

    public void RejectExceptFast(params string[] names)
    {
        if (Bot.Options.AcceptACDrops)
            PickupACItems();
        Bot.Flash.Call("rejectExcept", names.Join(',').ToLower());
    }

    public void RejectExceptFast(params int[] ids)
    {
        if (Bot.Options.AcceptACDrops)
            PickupACItems();
        var items = CurrentDropInfos.Where(d => ids.Contains(d.ID)).Select(d => d.Name);
        Bot.Flash.Call("rejectExcept", items.Join(',').ToLower());
    }

    public void RejectAll(bool skipWait = false)
    {
        if (Bot.Options.AcceptACDrops)
            PickupACItems();
        Bot.Flash.Call("rejectExcept", "");
        if (Bot.Options.SafeTimings && !skipWait)
            Bot.Wait.ForPickup("*");
    }

    public void Start()
    {
        if (DropsThread?.IsAlive ?? false)
            return;
        DropsThread = new(() =>
        {
            DropsCTS = new();
            Poll(DropsCTS.Token);
            DropsCTS.Dispose();
            DropsCTS = null;
        })
        {
            Name = "Drops Thread"
        };
        DropsStarted?.Invoke();
        DropsThread.Start();
    }

    public void Stop()
    {
        DropsStopped?.Invoke();
        DropsCTS?.Cancel();
        Bot.Wait.ForTrue(() => !Enabled, 20);
    }

    public void Add(params string[] names)
    {
        lock (_addName)
            _addName.AddRange(names);
    }

    public void Add(params int[] ids)
    {
        lock (_addID)
            _addID.AddRange(ids);
    }

    public void Clear()
    {
        lock (_remName)
            _remName.AddRange(ToPickup);
        lock (_remID)
            _remID.AddRange(ToPickupIDs);
    }

    public void Remove(params string[] names)
    {
        lock (_remName)
            _remName.AddRange(names);
    }

    public void Remove(params int[] ids)
    {
        lock (_remID)
            _remID.AddRange(ids);
    }

    private void Poll(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (_addName.Count > 0)
            {
                ToPickup.AddRange(_addName.Except(ToPickup));
                lock (_addName)
                    _addName.Clear();
            }
            if(_addID.Count > 0)
            {
                ToPickupIDs.AddRange(_addID.Except(ToPickupIDs));
                lock (_addID)
                    _addName.Clear();
            }
            if (_remName.Count > 0)
            {
                ToPickup.RemoveAll(_remName.Contains);
                lock (_remName)
                    _remName.Clear();
            }
            if (_remID.Count > 0)
            {
                ToPickupIDs.RemoveAll(_remID.Contains);
                lock (_remID)
                    _remID.Clear();
            }
            if(Bot.Player.Playing)
            {
                if (Bot.Options.AcceptACDrops)
                    PickupACItems();
                if(ToPickupIDs.Count > 0)
                {
                    Pickup(ToPickupIDs.ToArray());
                }
                if (ToPickup.Count > 0)
                {
                    Pickup(ToPickup.ToArray());
                }
                if (RejectElse)
                {
                    RejectExcept(ToPickup.ToArray());
                }
            }
            if (!token.IsCancellationRequested)
                Thread.Sleep(Interval);
        }
    }
}
