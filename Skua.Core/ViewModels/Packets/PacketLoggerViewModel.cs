using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;

namespace Skua.Core.ViewModels;
public partial class PacketLoggerViewModel : BotControlViewModelBase
{
    public PacketLoggerViewModel(IEnumerable<PacketLogFilterViewModel> filters, IFlashUtil flash, IFileDialogService fileDialog)
        : base("Packet Logger")
    {
        _flash = flash;
        _fileDialog = fileDialog;
        _packetFilters = filters.ToList();
        ClearPacketLogsCommand = new RelayCommand(PacketLogs.Clear);
    }

    private readonly IFlashUtil _flash;
    private readonly IFileDialogService _fileDialog;
    [ObservableProperty]
    private ObservableCollection<string> _packetLogs = new();
    [ObservableProperty]
    private List<PacketLogFilterViewModel> _packetFilters;

    private bool _isReceivingPackets;
    public bool IsReceivingPackets
    {
        get { return _isReceivingPackets; }
        set 
        {
            if(SetProperty(ref _isReceivingPackets, value))
                ToggleLogger();
        }
    }

    public IRelayCommand ClearPacketLogsCommand { get; }

    [RelayCommand]
    private void SavePacketLogs()
    {
        _fileDialog.SaveText(_packetLogs);
    }

    [RelayCommand]
    private void ClearFilters()
    {
        _packetFilters.ForEach(f => f.IsChecked = false);
    }

    private void ToggleLogger()
    {
        if (_isReceivingPackets)
            _flash.FlashCall += LogPackets;
        else
            _flash.FlashCall -= LogPackets;
    }

    private bool _filterEnabled => _packetFilters.Where(f => !f.IsChecked).Any();

    private void LogPackets(string function, object[] args)
    {
        if (function != "packet")
            return;

        if (!_filterEnabled)
        {
            PacketLogs.Add(args[0].ToString()!);
            return;
        }

        string[] packet = args[0].ToString()!.Split(new[] { '%' }, StringSplitOptions.RemoveEmptyEntries);
        foreach(Predicate<string[]> filter in _packetFilters.Where(f => !f.IsChecked).Select(f => f.Filter))
        {
            if (filter.Invoke(packet))
                return;
        }

        PacketLogs.Add(args[0].ToString()!);
    }
}
