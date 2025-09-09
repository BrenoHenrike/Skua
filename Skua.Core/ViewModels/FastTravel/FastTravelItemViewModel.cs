using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Messaging;

namespace Skua.Core.ViewModels;

public partial class FastTravelItemViewModel : ObservableObject
{
    public FastTravelItemViewModel(IRelayCommand<object> travel)
    {
        TravelCommand = travel;
    }

    public FastTravelItemViewModel(string descriptionName, string mapName, string cell, string pad, IRelayCommand<object> travel)
    {
        MapName = mapName;
        DescriptionName = descriptionName;
        Cell = cell;
        Pad = pad;
        TravelCommand = travel;
    }

    [ObservableProperty]
    private string _descriptionName = string.Empty;

    [ObservableProperty]
    private string _mapName = "battleon";

    [ObservableProperty]
    private string _cell = "Enter";

    [ObservableProperty]
    private string _pad = "Spawn";

    public IRelayCommand<object> TravelCommand { get; }

    [RelayCommand]
    private void Remove()
    {
        WeakReferenceMessenger.Default.Send<RemoveFastTravelMessage>(new(this));
    }

    [RelayCommand]
    private void Edit()
    {
        WeakReferenceMessenger.Default.Send<EditFastTravelMessage>(new(this));
    }

    public bool Validate()
    {
        return
            !string.IsNullOrWhiteSpace(Cell)
            && !string.IsNullOrWhiteSpace(Pad)
            && !string.IsNullOrWhiteSpace(MapName)
            && !string.IsNullOrWhiteSpace(DescriptionName);
    }

    public override string ToString()
    {
        return $"{_descriptionName},{_mapName},{_cell},{_pad}";
    }
}