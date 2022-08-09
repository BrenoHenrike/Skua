using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using Skua.Core.Messaging;

namespace Skua.Core.ViewModels;
public partial class FastTravelViewModel : BotControlViewModelBase
{
    public FastTravelViewModel(IMapService mapService, ISettingsService settings, IDialogService dialogService) 
        : base("Fast Travel")
    {
        Messenger.Register<FastTravelViewModel, RemoveFastTravelMessage>(this, RemoveFastTravel);
        Messenger.Register<FastTravelViewModel, EditFastTravelMessage>(this, EditFastTravel);
        MapService = mapService;
        _settings = settings;
        _dialogService = dialogService;
        _travelCommand = new RelayCommand<object>(mapService.Travel);
        Editor = new(mapService, _travelCommand);
        List<FastTravelItemViewModel> DefaultFastTravels = new();
        foreach (string? item in _settings.Get<StringCollection>("FastTravels")!)
        {
            if (string.IsNullOrWhiteSpace(item))
                continue;
            string[] values = item.Split(',');
            DefaultFastTravels.Add(new FastTravelItemViewModel(values[0], values[1], values[2], values[3], _travelCommand));
        }
        FastTravelItems = new(DefaultFastTravels);
    }

    private readonly ISettingsService _settings;
    private readonly IDialogService _dialogService;
    private readonly IRelayCommand<object> _travelCommand;
    [ObservableProperty]
    private int _selectedIndex = 0;

    public IMapService MapService { get; }
    public FastTravelEditorViewModel Editor { get; }
    public ObservableCollection<FastTravelItemViewModel> FastTravelItems { get; }

    [RelayCommand]
    private void AddFastTravel()
    {
        if (!Editor.Travel.Validate())
            return;

        FastTravelItems.Add(new FastTravelItemViewModel(
            Editor.Travel.DescriptionName,
            Editor.Travel.MapName,
            Editor.Travel.Cell,
            Editor.Travel.Pad,
            _travelCommand));
    }

    [RelayCommand]
    private void SaveFastTravels()
    {
        StringCollection values = new();
        values.AddRange(FastTravelItems.Select(i => i.ToString()).ToArray());
        _settings.Set("FastTravels", values);
    }

    [RelayCommand]
    private void ClearFastTravels()
    {
        if (_dialogService.ShowMessageBox("This will clear all fast travels from the list. Continue?", "Clear Fast Travels", true) == true)
            FastTravelItems.Clear();
    }

    private void RemoveFastTravel(FastTravelViewModel recipient, RemoveFastTravelMessage message)
    {
        if (message.FastTravel is null)
            return;

        recipient.FastTravelItems.Remove(message.FastTravel);
    }

    private void EditFastTravel(FastTravelViewModel recipient, EditFastTravelMessage message)
    {
        if (message.FastTravel is null)
            return;

        int index = recipient.FastTravelItems.IndexOf(message.FastTravel);
        FastTravelEditorDialogViewModel dialog = new(new(MapService, message.FastTravel));
        if(_dialogService.ShowDialog(dialog) == true)
            recipient.FastTravelItems[index] = dialog.Editor.Travel;
    }
}
