using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models.Items;
using Skua.Core.Utils;

namespace Skua.Core.Scripts;

public partial class ScriptDrop : ObservableRecipient, IScriptDrop, IAsyncDisposable
{
    private readonly Lazy<IScriptSend> _lazySend;
    private readonly Lazy<IScriptWait> _lazyWait;
    private readonly Lazy<IScriptMap> _lazyMap;
    private readonly Lazy<IScriptOption> _lazyOptions;
    private readonly Lazy<IScriptPlayer> _lazyPlayer;
    private readonly Lazy<IFlashUtil> _lazyFlash;
    private IScriptSend Send => _lazySend.Value;
    private IScriptWait Wait => _lazyWait.Value;
    private IScriptMap Map => _lazyMap.Value;
    private IScriptOption Options => _lazyOptions.Value;
    private IScriptPlayer Player => _lazyPlayer.Value;
    private IFlashUtil Flash => _lazyFlash.Value;
    public ScriptDrop(
        Lazy<IFlashUtil> flash,
        Lazy<IScriptSend> send,
        Lazy<IScriptMap> map,
        Lazy<IScriptWait> wait,
        Lazy<IScriptOption> options,
        Lazy<IScriptPlayer> player)
    {
        _lazySend = send;
        _lazyWait = wait;
        _lazyMap = map;
        _lazyOptions = options;
        _lazyPlayer = player;
        _lazyFlash = flash;

        StrongReferenceMessenger.Default.Register<ScriptDrop, ItemDroppedMessage, int>(this, (int)MessageChannels.GameEvents, AddDrop);
        StrongReferenceMessenger.Default.Register<ScriptDrop, LogoutMessage, int>(this, (int)MessageChannels.GameEvents, ClearDrops);
        StrongReferenceMessenger.Default.Register<ScriptDrop, ScriptStoppedMessage, int>(this, (int)MessageChannels.ScriptStatus, ScriptStopped);
        Messenger.Register<ScriptDrop, PropertyChangedMessage<bool>>(this, OptionsChanged);

        _timerDrops = new(TimeSpan.FromMilliseconds(1000));
    }

    private readonly PeriodicTimer _timerDrops;
    private Task? _taskDrops;
    private CancellationTokenSource? _ctsDrops;

    [ObservableProperty]
    private int _interval;
    [ObservableProperty]
    private bool _rejectElse;
    public bool Enabled => _taskDrops is not null;
    private readonly SynchronizedList<string> _toPickup = new();
    public IEnumerable<string> ToPickup => _toPickup.Items;
    private readonly SynchronizedList<int> _toPickupIDs = new();
    public IEnumerable<int> ToPickupIDs => _toPickupIDs.Items;
    private readonly SynchronizedList<ItemBase> _currentDropInfos = new();
    public IEnumerable<ItemBase> CurrentDropInfos => _currentDropInfos.Items;
    public IEnumerable<string> CurrentDrops => CurrentDropInfos.Select(x => x.Name.Trim()).ToList();

    public void Pickup(string name)
    {
        if (!CurrentDrops.Contains(name, StringComparer.OrdinalIgnoreCase))
            return;

        ItemBase drop = _currentDropInfos.Find(d => d.Name == name)!;
        Send.Packet($"%xt%zm%getDrop%{Map.RoomID}%{drop.ID}%");
        _currentDropInfos.Remove(drop);
        OnPropertyChanged(nameof(CurrentDropInfos));
        OnPropertyChanged(nameof(CurrentDrops));
    }

    public void Pickup(int id)
    {
        if (!CurrentDropInfos.Contains(i => i.ID == id))
            return;

        Send.Packet($"%xt%zm%getDrop%{Map.RoomID}%{id}%");
        _currentDropInfos.Remove(CurrentDropInfos.SingleOrDefault(d => d.ID == id)!);
        OnPropertyChanged(nameof(CurrentDropInfos));
        OnPropertyChanged(nameof(CurrentDrops));
    }

    public void Pickup(params string[] names)
    {
        foreach (string name in names)
        {
            Pickup(name);
            if (Options.SafeTimings)
                Wait.ForPickup(name);
        }
    }

    public void Pickup(params int[] ids)
    {
        foreach (int id in ids)
        {
            Pickup(id);
            if (Options.SafeTimings)
                Wait.ForPickup(id);
        }
    }

    public void PickupACItems()
    {
        Pickup(CurrentDropInfos.Where(d => d.Coins).Select(d => d.Name).ToArray());
    }

    public void PickupAll(bool skipWait = false)
    {
        _currentDropInfos.Items.ToList().ForEach(d => Pickup(d.Name));
        _currentDropInfos.Clear();
        OnPropertyChanged(nameof(CurrentDropInfos));
        OnPropertyChanged(nameof(CurrentDrops));
        if (Options.SafeTimings && !skipWait)
            Wait.ForPickup("*");
    }

    public void RejectExcept(params string[] names)
    {
        if (Options.AcceptACDrops)
            PickupACItems();
        Flash.Call("rejectExcept", names.Join(',').ToLower());
    }

    public void RejectExcept(params int[] ids)
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
    }

    public void Start()
    {
        if (_taskDrops is not null)
            return;

        _ctsDrops = new();
        _taskDrops = HandleDrops(_timerDrops, _ctsDrops.Token);
        OnPropertyChanged(nameof(Enabled));
    }

    public void Stop()
    {
        if (_taskDrops is null)
            return;

        _ctsDrops?.Cancel();
        Wait.ForTrue(() => _taskDrops?.IsCompleted == true, null, 20);
        _ctsDrops?.Dispose();
        _taskDrops = null;
        OnPropertyChanged(nameof(Enabled));
    }

    public async ValueTask StopAsync()
    {
        if (_taskDrops is null)
            return;

        _ctsDrops?.Cancel();
        await Wait.ForTrueAsync(() => _taskDrops?.IsCompleted == true, 20);
        _ctsDrops?.Dispose();
        _ctsDrops = null;
        _taskDrops = null;
        OnPropertyChanged(nameof(Enabled));
    }

    public void Add(params string[] names)
    {
        _toPickup.AddRange(names.Except(_toPickup.Items));
        OnPropertyChanged(nameof(ToPickup));
    }

    public void Add(params int[] ids)
    {
        _toPickupIDs.AddRange(ids.Except(_toPickupIDs.Items));
        OnPropertyChanged(nameof(ToPickupIDs));
    }

    public void Clear()
    {
        _toPickupIDs.Clear();
        _toPickup.Clear();
        OnPropertyChanged(nameof(ToPickupIDs));
        OnPropertyChanged(nameof(ToPickup));
    }

    public void Remove(params string[] names)
    {
        _toPickup.Remove(names.Contains);
    }

    public void Remove(params int[] ids)
    {
        _toPickupIDs.Remove(ids.Contains);
    }

    private async Task HandleDrops(PeriodicTimer timer, CancellationToken token)
    {
        try
        {
            while (await timer.WaitForNextTickAsync(token))
            {
                if (!Player.Playing)
                    continue;

                if (Interval > 0)
                    await Task.Delay(Interval, token);

                if (Options.AcceptAllDrops)
                {
                    PickupAll();
                    continue;
                }

                if (Options.AcceptACDrops)
                    PickupACItems();

                if (Options.RejectAllDrops)
                {
                    RejectAll();
                    continue;
                }

                if (_toPickupIDs.Any())
                    Pickup(_toPickupIDs.Items.ToArray());

                if (_toPickup.Any())
                    Pickup(_toPickup.Items.ToArray());

                if (RejectElse)
                    RejectExcept(_toPickup.Items.ToArray());
            }
        }
        catch { }
    }

    private void OptionsChanged(ScriptDrop recipient, PropertyChangedMessage<bool> message)
    {
        if (message.Sender.GetType() != typeof(ScriptOption))
            return;

        switch(message.PropertyName)
        {
            case nameof(IScriptOption.AcceptACDrops):
            case nameof(IScriptOption.AcceptAllDrops):
            case nameof(IScriptOption.RejectAllDrops):
                recipient.Start();
                break;
        }
    }

    private void ClearDrops(ScriptDrop recipient, LogoutMessage message)
    {
        recipient._currentDropInfos.Clear();
        recipient.OnPropertyChanged(nameof(recipient.CurrentDropInfos));
        recipient.OnPropertyChanged(nameof(recipient.CurrentDrops));
    }

    private void AddDrop(ScriptDrop recipient, ItemDroppedMessage message)
    {
        if (message.AddedToInv)
            return;

        if (!recipient.CurrentDropInfos.Contains(message.Item))
            recipient._currentDropInfos.Add(message.Item);
        else
            recipient._currentDropInfos.Find(i => i.Equals(message.Item))!.Quantity += message.Item.Quantity;
        recipient.OnPropertyChanged(nameof(recipient.CurrentDropInfos));
        recipient.OnPropertyChanged(nameof(recipient.CurrentDrops));
    }

    private async void ScriptStopped(ScriptDrop recipient, ScriptStoppedMessage message)
    {
        await recipient.StopAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if(_taskDrops is not null)
        {
            _ctsDrops?.Cancel();
            await _taskDrops;
            _ctsDrops?.Dispose();
        }
        _timerDrops.Dispose();
        GC.SuppressFinalize(this);
    }
}
