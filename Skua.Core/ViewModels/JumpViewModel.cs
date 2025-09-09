using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Utils;
using System.Collections.Immutable;

namespace Skua.Core.ViewModels;

public partial class JumpViewModel : BotControlViewModelBase
{
    public JumpViewModel(IMapService mapService)
        : base("Jump")
    {
        _mapService = mapService;
        Pads = _mapService.Pads;
    }

    private readonly IMapService _mapService;

    [ObservableProperty]
    private string _selectedCell = string.Empty;

    [ObservableProperty]
    private string _selectedPad = string.Empty;

    [ObservableProperty]
    private RangedObservableCollection<string> _cells = new();

    public ImmutableList<string> Pads { get; }

    [RelayCommand]
    private void GetCurrent()
    {
        (SelectedCell, SelectedPad) = _mapService.GetCurrentCell();
    }

    [RelayCommand]
    private void JumpTo()
    {
        _mapService.Jump(SelectedCell, SelectedPad);
    }

    [RelayCommand]
    public void UpdateCells()
    {
        Cells.ReplaceRange(_mapService.Cells);
    }
}