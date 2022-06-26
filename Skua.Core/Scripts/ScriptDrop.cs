using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Utils;

namespace Skua.Core.Scripts;

public class ScriptDrop : IScriptDrop, IDisposable
{
    private readonly Lazy<IScriptSend> _lazySend;
    private readonly Lazy<IScriptWait> _lazyWait;
    private readonly Lazy<IScriptMap> _lazyMap;
    private readonly Lazy<IScriptOption> _lazyOptions;
    private readonly Lazy<IScriptPlayer> _lazyPlayer;
    private readonly Lazy<IScriptManager> _lazyManager;
    private readonly Lazy<IFlashUtil> _lazyFlash;
    private IScriptSend Send => _lazySend.Value;
    private IScriptWait Wait => _lazyWait.Value;
    private IScriptMap Map => _lazyMap.Value;
    private IScriptOption Options => _lazyOptions.Value;
    private IScriptPlayer Player => _lazyPlayer.Value;
    private IScriptManager Manager => _lazyManager.Value;
    private IFlashUtil Flash => _lazyFlash.Value;
    public ScriptDrop(
        Lazy<IFlashUtil> flash,
        Lazy<IScriptSend> send,
        Lazy<IScriptMap> map,
        Lazy<IScriptWait> wait,
        Lazy<IScriptOption> options,
        Lazy<IScriptPlayer> player,
        Lazy<IScriptManager> manager)
    {
        _lazySend = send;
        _lazyWait = wait;
        _lazyMap = map;
        _lazyOptions = options;
        _lazyPlayer = player;
        _lazyFlash = flash;
        _lazyManager = manager;

        AppGameEvents.ItemDropped += AddDrop;
        AppGameEvents.Logout += ClearDrops;
        Manager.ApplicationShutDown += DisposeOnShutDown;
        void DisposeOnShutDown(bool b)
        {
            Dispose();
            Manager.ApplicationShutDown -= DisposeOnShutDown;
        }
    }

    private void ClearDrops()
    {
        _currentDropInfos.Clear();
    }

    private void AddDrop(ItemBase item, bool addedToInv, int quantityNow)
    {
        if (CurrentDropInfos.All(d => d.ID != item.ID))
            _currentDropInfos.Add(item);
    }

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
    internal SynchronizedList<ItemBase> _currentDropInfos { get; set; } = new();
    public IEnumerable<ItemBase> CurrentDropInfos => _currentDropInfos.Items;
    public IEnumerable<string> CurrentDrops => CurrentDropInfos.Select(x => x.Name.Trim()).ToList();

    public void Pickup(string name)
    {
        if (!CurrentDrops.Contains(name, StringComparer.OrdinalIgnoreCase))
            return;

        ItemBase drop = _currentDropInfos.Find(d => d.Name == name)!;
        Send.Packet($"%xt%zm%getDrop%{Map.RoomID}%{drop.ID}%");
        _currentDropInfos.Remove(drop);
    }

    public void Pickup(int id)
    {
        if (!CurrentDropInfos.Contains(i => i.ID == id))
            return;

        Send.Packet($"%xt%zm%getDrop%{Map.RoomID}%{id}%");
        _currentDropInfos.Remove(CurrentDropInfos.SingleOrDefault(d => d.ID == id)!);
    }

    public void Pickup(params string[] names)
    {
        for(int i = 0; i < names.Length; i++)
        {
            Pickup(names[i]);
            if (Options.SafeTimings)
                Wait.ForPickup(names[i]);
        }
    }

    public void Pickup(params int[] ids)
    {
        for(int i = 0; i < ids.Length; i++)
        {
            Pickup(ids[i]);
            if (Options.SafeTimings)
                Wait.ForPickup(ids[i]);
        }
    }

    public void PickupACItems()
    {
        Pickup(CurrentDropInfos.Where(d => d.Coins).Select(d => d.Name).ToArray());
    }

    public void PickupAll(bool skipWait = false)
    {
        _currentDropInfos.ForEach(d => Pickup(d.Name));
        _currentDropInfos.Clear();
        if (Options.SafeTimings && !skipWait)
            Wait.ForPickup("*");
    }

    public void RejectExcept(params string[] names)
    {
        if (Options.AcceptACDrops)
            PickupACItems();
        Flash.Call("rejectExcept", names.Join(',').ToLower());
        if (Options.SafeTimings)
            Wait.ForPickup("*");
    }

    public void RejectExcept(params int[] ids)
    {
        if (Options.AcceptACDrops)
            PickupACItems();
        IEnumerable<string> items = CurrentDropInfos.Where(d => ids.Contains(d.ID)).Select(d => d.Name);
        Flash.Call("rejectExcept", items.Join(',').ToLower());
        if (Options.SafeTimings)
            Wait.ForPickup("*");
    }

    public void RejectExceptFast(params string[] names)
    {
        if (Options.AcceptACDrops)
            PickupACItems();
        Flash.Call("rejectExcept", names.Join(',').ToLower());
    }

    public void RejectExceptFast(params int[] ids)
    {
        if (Options.AcceptACDrops)
            PickupACItems();
        IEnumerable<string> items = CurrentDropInfos.Where(d => ids.Contains(d.ID)).Select(d => d.Name);
        Flash.Call("rejectExcept", items.Join(',').ToLower());
    }

    public void RejectAll(bool skipWait = false)
    {
        if (Options.AcceptACDrops)
            PickupACItems();
        Flash.Call("rejectExcept", "");
        if (Options.SafeTimings && !skipWait)
            Wait.ForPickup("*");
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
        Wait.ForTrue(() => !Enabled, 20);
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
            if(Player.Playing)
            {
                if (Options.AcceptACDrops)
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

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        AppGameEvents.ItemDropped -= AddDrop;
        AppGameEvents.Logout -= ClearDrops;
    }
}
