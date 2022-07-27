using System.Collections.Immutable;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels;
public partial class JumpViewModel : BotControlViewModelBase
{
    public JumpViewModel(IMapService mapService)
        : base("Jump")
    {
        _mapService = mapService;
        Pads = _mapService.Pads;
        JumpToCommand = new RelayCommand(JumpTo);
        GetCurrentCommand = new RelayCommand(GetCurrent);
        UpdateCellsCommand = new RelayCommand(UpdateCells);
    }

    private readonly IMapService _mapService;
    [ObservableProperty]
    private string _selectedCell = string.Empty;
    [ObservableProperty]
    private string _selectedPad = string.Empty;
    [ObservableProperty]
    private RangedObservableCollection<string> _cells = new();
    public ImmutableList<string> Pads { get; }

    public IRelayCommand JumpToCommand { get; }
    public IRelayCommand GetCurrentCommand { get; }
    public IRelayCommand UpdateCellsCommand { get; }

    private void GetCurrent()
    {
        (SelectedCell, SelectedPad) = _mapService.GetCurrentCell();
    }

    private void JumpTo()
    {
        _mapService.Jump(SelectedCell, SelectedPad);
    }

    public void UpdateCells()
    {
        Cells.ReplaceRange(_mapService.Cells);
    }
}
