using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;

namespace Skua.Core.ViewModels;

public partial class FastTravelItemViewModel : ObservableRecipient
{
    public FastTravelItemViewModel(IRelayCommand<object> travel)
    {
        TravelCommand = travel;
        EditCommand = new RelayCommand(EditTravel);
        RemoveCommand = new RelayCommand(RemoveTravel);
    }
    public FastTravelItemViewModel(string descriptionName, string mapName, string cell, string pad, IRelayCommand<object> travel)
    {
        MapName = mapName;
        DescriptionName = descriptionName;
        Cell = cell;
        Pad = pad;
        TravelCommand = travel;
        EditCommand = new RelayCommand(EditTravel);
        RemoveCommand = new RelayCommand(RemoveTravel);
    }

    public bool Validate()
    {
        return 
            !string.IsNullOrWhiteSpace(Cell)
            && !string.IsNullOrWhiteSpace(Pad)
            && !string.IsNullOrWhiteSpace(MapName)
            && !string.IsNullOrWhiteSpace(DescriptionName);
    }

    private void RemoveTravel()
    {
        Messenger.Send<RemoveFastTravelMessage>(new(this));
    }

    private void EditTravel()
    {
        Messenger.Send<EditFastTravelMessage>(new(this));
    }

    [ObservableProperty]
    private string _descriptionName = string.Empty;
    [ObservableProperty]
    private string _mapName = "battleon";
    [ObservableProperty]
    private string _cell = "Enter";
    [ObservableProperty]
    private string _pad = "Spawn";

    public override string ToString()
    {
        return $"{_descriptionName},{_mapName},{_cell},{_pad}";
    }

    public IRelayCommand<object> TravelCommand { get; }
    public IRelayCommand RemoveCommand { get; }
    public IRelayCommand EditCommand { get; }
}
