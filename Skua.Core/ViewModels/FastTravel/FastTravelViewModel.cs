using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Interfaces.Services;
using System.Collections.Specialized;
using Microsoft.Toolkit.Mvvm.ComponentModel;
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
        Settings = settings;
        _dialogService = dialogService;
        _travelCommand = new RelayCommand<object>(mapService.Travel);
        Editor = new(mapService, _travelCommand);
        List<FastTravelItemViewModel> DefaultFastTravels = new();
        foreach (string? item in Settings.Get<StringCollection>("FastTravels")!)
        {
            if (string.IsNullOrWhiteSpace(item))
                continue;
            string[] values = item.Split(',');
            DefaultFastTravels.Add(new FastTravelItemViewModel(values[0], values[1], values[2], values[3], _travelCommand));
        }
        FastTravelItems = new(DefaultFastTravels);
        AddFastTravelCommand = new RelayCommand(AddFastTravel);
        SaveFastTravelsCommand = new RelayCommand(SaveFastTravels);
        ClearFastTravelsCommand = new RelayCommand(ClearFastTravels);
    }

    public IMapService MapService { get; }
    private readonly ISettingsService Settings;
    private readonly IDialogService _dialogService;
    private readonly IRelayCommand<object> _travelCommand;

    [ObservableProperty]
    private int _selectedIndex = 0;
    public FastTravelEditorViewModel Editor { get; }
    public ObservableCollection<FastTravelItemViewModel> FastTravelItems { get; }

    public IRelayCommand AddFastTravelCommand { get; }
    public IRelayCommand SaveFastTravelsCommand { get; }
    public IRelayCommand ClearFastTravelsCommand { get; }

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

    private void ClearFastTravels()
    {
        if(_dialogService.ShowMessageBox("This will clear all fast travels from the list. Continue?", "Clear Fast Travels", true) == true)
            FastTravelItems.Clear();
    }

    private void SaveFastTravels()
    {
        StringCollection values = new();
        values.AddRange(FastTravelItems.Select(i => i.ToString()).ToArray());
        Settings.Set("FastTravels", values);
    }
}
