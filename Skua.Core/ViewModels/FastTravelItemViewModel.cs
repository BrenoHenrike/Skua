using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Skua.Core.ViewModels;

public class FastTravelItemViewModel : ObservableObject
{
    public FastTravelItemViewModel() { }
    public FastTravelItemViewModel(string descriptionName, string mapName, string cell, string pad)
    {
        MapName = mapName;
        DescriptionName = descriptionName;
        Cell = cell;
        Pad = pad;
    }

    private string _descriptionName = string.Empty;
    private string _mapName = "battleon";
    private string _cell = "Enter";
    private string _pad = "Spawn";
    public string MapName
    {
        get { return _mapName; }
        set { SetProperty(ref _mapName, value); }
    }
    public string DescriptionName
    {
        get { return _descriptionName; }
        set { SetProperty(ref _descriptionName, value); }
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

    public override string ToString()
    {
        return $"{_descriptionName},{_mapName},{_cell},{_pad}";
    }
}
