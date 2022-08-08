using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Messaging;

namespace Skua.Core.ViewModels;
public partial class CurrentDropsViewModel : BotControlViewModelBase
{
    public CurrentDropsViewModel(IScriptDrop drops, IScriptPlayer player)
        : base("Current Drops", 500, 400)
    {
        Messenger.Register<CurrentDropsViewModel, ItemDroppedMessage>(this, CurrentDropsChanged);
        _drops = drops;
        _player = player;
        PickupCommand = new RelayCommand(Pickup);
        PickupAllCommand = new RelayCommand(() => drops.PickupAll(true));
        PickupACCommand = new RelayCommand(_drops.PickupACItems);
        PickupSelectedCommand = new RelayCommand<IList<object>>(PickupSelected);
    }

    private readonly IScriptPlayer _player;
    private readonly IScriptDrop _drops;
    [ObservableProperty]
    private ItemBase? _selectedDrop;

    public List<ItemBase> CurrentDrops => _drops.CurrentDropInfos.ToList();

    public IRelayCommand PickupCommand { get; }
    public IRelayCommand PickupAllCommand { get; }
    public IRelayCommand PickupACCommand { get; }
    public IRelayCommand PickupSelectedCommand { get; }
    private void Pickup()
    {
        if (SelectedDrop is null || !_player.Playing)
            return;

        _drops.Pickup(SelectedDrop.ID);
    }

    private void PickupSelected(IList<object>? items)
    {
        if (items is null || !_player.Playing)
            return;

        IEnumerable<ItemBase> drops = items.Cast<ItemBase>();
        _drops.Pickup(drops.Select(d => d.ID).ToArray());
    }

    private void CurrentDropsChanged(CurrentDropsViewModel recipient, ItemDroppedMessage message)
    {
        recipient.OnPropertyChanged(nameof(recipient.CurrentDrops));
    }
}
