using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;

namespace Skua.Core.ViewModels;

public partial class FastTravelEditorViewModel : ObservableObject
{
    public FastTravelEditorViewModel(IMapService mapService, IRelayCommand<object> travel)
    {
        _mapService = mapService;
        _travel = new(travel);
    }

    public FastTravelEditorViewModel(IMapService mapService, FastTravelItemViewModel fastTravel)
    {
        _mapService = mapService;
        _travel = new(
            fastTravel.DescriptionName,
            fastTravel.MapName,
            fastTravel.Cell,
            fastTravel.Pad,
            fastTravel.TravelCommand);
    }

    private readonly IMapService _mapService;

    [ObservableProperty]
    private FastTravelItemViewModel _travel;

    [RelayCommand]
    private void GetCurrent()
    {
        (Travel.MapName, Travel.Cell, Travel.Pad) = _mapService.GetCurrentLocation();
    }
}