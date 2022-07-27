using System.Collections.Immutable;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Skua.Core.Interfaces;
using Skua.Core.ViewModels;

namespace Skua.Core.Services;
public partial class MapService : ObservableObject, IMapService
{
    private readonly IScriptMap Map;
    private readonly IScriptPlayer Player;

    public MapService(IScriptMap map, IScriptPlayer player)
    {
        Map = map;
        Player = player;
    }
    public string MapName => Map.Name;

    public string Cell => Player.Cell;

    public string Pad => Player.Pad;

    public List<string> Cells => Map.Cells;
    
    [ObservableProperty]
    private bool _UsePrivateRoom;
    [ObservableProperty]
    private int _PrivateRoomNumber = 111111;

    public ImmutableList<string> Pads { get; } = ImmutableList.Create("Spawn", "Center", "Left", "Right", "Up", "Down", "Top", "Bottom");

    public (string cell, string pad) GetCurrentCell()
    {
        if (!Player.Playing)
            return ("Spawn", "Enter");
        return (Cell, Pad);
    }

    public (string mapName, string cell, string pad) GetCurrentLocation()
    {
        if (!Player.Playing)
            return ("Battleon", "Spawn", "Enter");
        return (MapName, Cell, Pad);
    }

    public void Travel(object? info)
    {
        if (!Player.Playing || info is null)
            return;
        Task.Run(() =>
        {
            FastTravelItemViewModel vm = (info as FastTravelItemViewModel)!;
            if (vm.MapName != "tercessuinotlim")
            {
                Map.Join(UsePrivateRoom ? $"{vm.MapName}-{PrivateRoomNumber}" : vm.MapName, vm.Cell, vm.Pad);
                return;
            }

            if (Map.Name == "tercessuinotlim")
            {
                Map.Jump(vm.Cell, vm.Pad);
                return;
            }
            Map.Jump("m22", "Left");
            Map.Join(UsePrivateRoom ? $"tercessuinotlim-{PrivateRoomNumber}" : "tercessuinotlim", vm.Cell, vm.Pad);
        });
    }

    public void Jump(string cell, string pad)
    {
        if (!Player.LoggedIn)
            return;
        Map.Jump(cell, pad);
    }
}
