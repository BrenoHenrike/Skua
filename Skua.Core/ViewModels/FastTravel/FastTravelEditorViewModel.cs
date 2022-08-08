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
        GetCurrentCommand = new RelayCommand(GetCurrentLocation);
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
        GetCurrentCommand = new RelayCommand(GetCurrentLocation);
    }

    private readonly IMapService _mapService;
    [ObservableProperty]
    private FastTravelItemViewModel _travel;
    public IRelayCommand GetCurrentCommand { get; }

    private void GetCurrentLocation()
    {
        (Travel.MapName, Travel.Cell, Travel.Pad) = _mapService.GetCurrentLocation();
    }
}
