using System.Collections.Immutable;
using Microsoft.Toolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels;
public class JumpViewModel : BotControlViewModelBase
{
    public JumpViewModel(IMapService mapService)
        : base("Jump")
    {
        MapService = mapService;
        Pads = MapService.Pads;
        JumpToCommand = new RelayCommand(() => MapService.Jump(SelectedCell, SelectedPad));
        GetCurrentCommand = new RelayCommand(() => (SelectedCell, SelectedPad) = MapService.GetCurrentCell());
        UpdateCellsCommand = new RelayCommand(() => UpdateCells());
    }

    private readonly IMapService MapService;
    private string _selectedCell = string.Empty;
    private string _selectedPad = string.Empty;
    private RangedObservableCollection<string> _cells = new();

    public RangedObservableCollection<string> Cells
    {
        get { return _cells; }
        set { SetProperty(ref _cells, value); }
    }
    public ImmutableList<string> Pads { get; }
    public string SelectedCell
    {
        get { return _selectedCell; }
        set { SetProperty(ref _selectedCell, value); }
    }
    public string SelectedPad
    {
        get { return _selectedPad; }
        set { SetProperty(ref _selectedPad, value); }
    }

    public IRelayCommand JumpToCommand { get; }
    public IRelayCommand GetCurrentCommand { get; }
    public IRelayCommand UpdateCellsCommand { get; }

    private string _lastMap = string.Empty;
    public bool UpdateCells()
    {
        string map = MapService.MapName;
        if (!string.IsNullOrEmpty(map) && _lastMap != map)
        {
            Cells.ReplaceRange(MapService.Cells);
            _lastMap = map;
            return true;
        }
        return false;
    }
}
