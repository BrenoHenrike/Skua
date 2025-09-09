using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models.Items;

namespace Skua.Core.ViewModels;

public partial class CurrentDropsViewModel : BotControlViewModelBase
{
    public CurrentDropsViewModel(IScriptDrop drops, IScriptPlayer player)
        : base("Current Drops", 500, 400)
    {
        _drops = drops;
        _player = player;
    }

    protected override void OnActivated()
    {
        StrongReferenceMessenger.Default.Register<CurrentDropsViewModel, ItemDroppedMessage, int>(this, (int)MessageChannels.GameEvents, CurrentDropsChanged);
        OnPropertyChanged(nameof(CurrentDrops));
    }

    protected override void OnDeactivated()
    {
        StrongReferenceMessenger.Default.UnregisterAll(this);
    }

    private readonly IScriptPlayer _player;
    private readonly IScriptDrop _drops;

    [ObservableProperty]
    private ItemBase? _selectedDrop;

    public List<ItemBase> CurrentDrops => _drops.CurrentDropInfos.ToList();

    [RelayCommand]
    private void Pickup()
    {
        if (SelectedDrop is null || !_player.Playing)
            return;

        _drops.Pickup(SelectedDrop.ID);
    }

    [RelayCommand]
    private void PickupSelected(IList<object>? items)
    {
        if (items is null || !_player.Playing)
            return;

        IEnumerable<ItemBase> drops = items.Cast<ItemBase>();
        _drops.Pickup(drops.Select(d => d.ID).ToArray());
    }

    [RelayCommand]
    private void PickupAll()
    {
        _drops.PickupAll(true);
    }

    [RelayCommand]
    private void PickupAC()
    {
        _drops.PickupACItems();
    }

    private void CurrentDropsChanged(CurrentDropsViewModel recipient, ItemDroppedMessage message)
    {
        recipient.OnPropertyChanged(nameof(recipient.CurrentDrops));
    }
}