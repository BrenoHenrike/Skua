using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels;
public sealed partial class NotifyDropViewModel : BotControlViewModelBase
{
    private readonly char[] _dropsSeparator = { '|' };
    private readonly ISoundService _soundService;

    public NotifyDropViewModel(ISoundService soundService)
        : base("Notify Drop", 400, 300)
    {
        RemoveAllDropsCommand = new RelayCommand(NotifyDropList.Clear);
        _soundService = soundService;
    }

    private bool _subscribed = false;
    [ObservableProperty]
    private string _addDropInput = string.Empty;
    [ObservableProperty]
    private int _soundCount = 5;
    [ObservableProperty]
    private int _soundDelay = 200;
    public RangedObservableCollection<string> NotifyDropList { get; set; } = new();
    public IRelayCommand RemoveAllDropsCommand { get; }

    [RelayCommand]
    private void RemoveDrops(IList<object>? items)
    {
        if (items is null)
            return;
        IEnumerable<string> drops = items.Cast<string>();
        if (drops.Any())
            NotifyDropList.RemoveRange(drops);

        if (NotifyDropList.Count == 0)
        {
            StrongReferenceMessenger.Default.Unregister<ItemDroppedMessage, int>(this, (int)MessageChannels.GameEvents);
            _subscribed = false;
        }
    }

    [RelayCommand]
    private void AddDrop()
    {
        if (string.IsNullOrWhiteSpace(AddDropInput))
            return;

        IEnumerable<string> drops = AddDropInput.Split(_dropsSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (drops.Any())
        {
            NotifyDropList.AddRange(drops.Except(NotifyDropList));
            OnPropertyChanged(nameof(NotifyDropList));

            if (!_subscribed)
            {
                StrongReferenceMessenger.Default.Register<NotifyDropViewModel, ItemDroppedMessage, int>(this, (int)MessageChannels.GameEvents, ItemDropped);
                _subscribed = true;
            }
        }

        AddDropInput = string.Empty;
    }

    [RelayCommand]
    private void TestBeep()
    {
        Task.Factory.StartNew(async () =>
        {
            for (int i = 0; i < SoundCount; i++)
            {
                _soundService.Beep();
                await Task.Delay(SoundDelay);
            }
        });
    }

    private void ItemDropped(NotifyDropViewModel recipient, ItemDroppedMessage message)
    {
        foreach (var item in recipient.NotifyDropList.ToList())
        {
            if (item == message.Item.Name)
            {
                Task.Factory.StartNew(async () =>
                {
                    for (int i = 0; i < recipient.SoundCount; i++)
                    {
                        recipient._soundService.Beep();
                        await Task.Delay(recipient.SoundDelay);
                    }
                });
                break;
            }
        }
    }
}
