using System.Collections.Immutable;
using System.ComponentModel;

namespace Skua.Core.Interfaces;

public interface IMapService : INotifyPropertyChanged
{
    string MapName { get; }
    string Cell { get; }
    string Pad { get; }
    List<string> Cells { get; }
    ImmutableList<string> Pads { get; }
    bool UsePrivateRoom { get; set; }
    int PrivateRoomNumber { get; set; }

    (string mapName, string cell, string pad) GetCurrentLocation();

    (string cell, string pad) GetCurrentCell();

    void Travel(object? info);

    void Jump(string cell, string pad);
}