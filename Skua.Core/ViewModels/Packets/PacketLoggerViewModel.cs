using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;

namespace Skua.Core.ViewModels;
public class PacketLoggerViewModel : BotControlViewModelBase
{
    public PacketLoggerViewModel(IEnumerable<PacketLogFilterViewModel> filters, IFlashUtil flash, IFileDialogService fileDialog)
        : base("Packet Logger")
    {
        _flash = flash;
        _fileDialog = fileDialog;
        _packetFilters = filters.ToList();
        ClearPacketLogsCommand = new RelayCommand(PacketLogs.Clear);
        ClearFiltersCommand = new RelayCommand(ClearFilters);
        SavePacketLogsCommand = new RelayCommand(SavePacketLogs);
    }

    private readonly IFlashUtil _flash;
    private readonly IFileDialogService _fileDialog;
    private ObservableCollection<string> _packetLogs = new();
    public ObservableCollection<string> PacketLogs
    {
        get { return _packetLogs; }
        set { SetProperty(ref _packetLogs, value); }
    }
    private List<PacketLogFilterViewModel> _packetFilters;
    public List<PacketLogFilterViewModel> PacketFilters
    {
        get { return _packetFilters; }
        set { SetProperty(ref _packetFilters, value); }
    }
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
    public IRelayCommand SavePacketLogsCommand { get; }
    public IRelayCommand ClearFiltersCommand { get; }

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

    private void SavePacketLogs()
    {
        _fileDialog.SaveText(_packetLogs);
    }

    private void ClearFilters()
    {
        _packetFilters.ForEach(f => f.IsChecked = false);
    }
}
