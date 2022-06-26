using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using System.ComponentModel;
using Skua.Core.Interfaces.Services;
using System.Collections.Specialized;

namespace Skua.Core.ViewModels;
public class FastTravelViewModel : BotControlViewModelBase
{
    public FastTravelViewModel(IMapService mapService, ISettingsService settings, IDialogService dialogService) 
        : base("Fast Travel")
    {
        MapService = mapService;
        Settings = settings;
        _dialogService = dialogService;
        List<FastTravelItemViewModel> DefaultFastTravels = new();
        foreach (string? item in Settings.GetValue<StringCollection>("FastTravels")!)
        {
            if (string.IsNullOrWhiteSpace(item))
                continue;
            string[] values = item.Split(',');
            DefaultFastTravels.Add(new FastTravelItemViewModel(values[0], values[1], values[2], values[3]));
        }
        FastTravelItems = new(DefaultFastTravels);
        TravelCommand = new RelayCommand<object>(s => mapService.Travel(s));
        GetCurrentCommand = new RelayCommand(() => (MapName, Cell, Pad) = mapService.GetCurrentLocation());
        AddFastTravelCommand = new RelayCommand(AddFastTravel);
        RemoveFastTravelCommand = new RelayCommand<FastTravelItemViewModel>(RemoveFastTravel);
        SetEditingTravelItemCommand = new RelayCommand<FastTravelItemViewModel>(SetEditingTravelItem);
        EditFastTravelCommand = new RelayCommand<FastTravelItemViewModel>(EditFastTravel);
        SaveFastTravelsCommand = new RelayCommand(SaveFastTravels);
        ClearFastTravelsCommand = new RelayCommand(ClearFastTravels);
    }

    private readonly IMapService MapService;
    private readonly ISettingsService Settings;
    private readonly IDialogService _dialogService;
    private string _descriptionName = string.Empty;
    private string _mapName = "battleon";
    private string _cell = "Enter";
    private string _pad = "Spawn";
    private int _selectedIndex = 0;
    private int _privateRoomNumber = 111111;
    private bool _usePrivateRoom;

    public bool UsePrivateRoom
    {
        get { return _usePrivateRoom; }
        set
        {
            SetProperty(ref _usePrivateRoom, value);
            MapService.UsePrivateRoom = value;
        }
    }
    public int PrivateRoomNumber
    {
        get { return _privateRoomNumber; }
        set
        {
            SetProperty(ref _privateRoomNumber, value);
            MapService.PrivateRoomNumber = value;
        }
    }
    public string DescriptionName
    {
        get { return _descriptionName; }
        set { SetProperty(ref _descriptionName, value); }
    }
    public string MapName
    {
        get { return _mapName; }
        set { SetProperty(ref _mapName, value); }
    }
    public string Cell
    {
        get { return _cell; }
        set { SetProperty(ref _cell, value); }
    }
    public string Pad
    {
        get { return _pad; }
        set { SetProperty(ref _pad, value); }
    }
    public int SelectedIndex
    {
        get { return _selectedIndex; }
        set { SetProperty(ref _selectedIndex, value); }
    }
    public ObservableCollection<FastTravelItemViewModel> FastTravelItems { get; }

    public IRelayCommand AddFastTravelCommand { get; }
    public IRelayCommand RemoveFastTravelCommand { get; }
    public IRelayCommand EditFastTravelCommand { get; }
    public IRelayCommand SetEditingTravelItemCommand { get; }
    public IRelayCommand TravelCommand { get; }
    public IRelayCommand GetCurrentCommand { get; }
    public IRelayCommand SaveFastTravelsCommand { get; }
    public IRelayCommand ClearFastTravelsCommand { get; }

    private void AddFastTravel()
    {
        if (string.IsNullOrEmpty(Cell))
            Cell = "Enter";
        if (string.IsNullOrEmpty(Pad))
            Pad = "Spawn";
        if (string.IsNullOrEmpty(MapName))
            MapName = MapService.MapName;
        if (string.IsNullOrEmpty(DescriptionName))
            DescriptionName = MapService.MapName;

        FastTravelItems.Add(new FastTravelItemViewModel(DescriptionName, MapName, Cell, Pad));
    }

    private void RemoveFastTravel(FastTravelItemViewModel? item)
    {
        if (item is null)
            return;

        FastTravelItems.Remove(item);
    }

    private void EditFastTravel(FastTravelItemViewModel? item)
    {
        if (item is null)
            return;
        FastTravelItems[SelectedIndex] = item;
    }

    private void SetEditingTravelItem(FastTravelItemViewModel? item)
    {
        if (item is null)
            return;
        SelectedIndex = FastTravelItems.IndexOf(item);
    }

    private void ClearFastTravels()
    {
        if(_dialogService.ShowMessageBox("This will delete all fast travels from the list. Continue?", "Clear Fast Travels", true) == true)
            FastTravelItems.Clear();
    }

    private void SaveFastTravels()
    {
        StringCollection values = new();
        foreach (FastTravelItemViewModel item in FastTravelItems)
        {
            values.Add(item.ToString());
        }
        Settings.SetValue("FastTravels", values);
    }
}
