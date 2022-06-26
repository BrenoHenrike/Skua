using Microsoft.Toolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels;
public class PacketSpammerViewModel : BotControlViewModelBase
{
    public PacketSpammerViewModel(IScriptSend send, IFileDialogService fileDialog)
        : base("Packet Spammer")
    {
        _send = send;
        _fileDialog = fileDialog;
        AddPacketCommand = new RelayCommand(AddPacket);
        RemovePacketCommand = new RelayCommand(RemovePacket);
        ClearPacketsCommand = new RelayCommand(() => Packets.Clear());
        SendPacketCommand = new RelayCommand(SendPacket);
        ToggleSpammerCommand = new RelayCommand(ToggleSpammer);
        SpammerCommand = new AsyncRelayCommand(Spammer);
        LoadSpammerCommand = new RelayCommand(LoadSpammer);
        SaveSpammerCommand = new RelayCommand(SaveSpammer);
    }

    private readonly IScriptSend _send;
    private readonly IFileDialogService _fileDialog;
    private RangedObservableCollection<string> _packets = new();
    public RangedObservableCollection<string> Packets
    {
        get { return _packets; }
        set { SetProperty(ref _packets, value); }
    }
    private bool _sendToClient;
    public bool SendToClient
    {
        get { return _sendToClient; }
        set { SetProperty(ref _sendToClient, value); }
    }
    private int _spamDelay = 1000;
    public int SpamDelay
    {
        get { return _spamDelay; }
        set { SetProperty(ref _spamDelay, value); }
    }
    private string _packetText = string.Empty;
    public string PacketText
    {
        get { return _packetText; }
        set { SetProperty(ref _packetText, value); }
    }
    private int _sendTimes = 1;
    public int SendTimes
    {
        get { return _sendTimes; }
        set { SetProperty(ref _sendTimes, value); }
    }
    private int _selectedIndex = -1;

    public int SelectedIndex
    {
        get { return _selectedIndex; }
        set { SetProperty(ref _selectedIndex, value); }
    }

    public IRelayCommand AddPacketCommand { get; }
    public IRelayCommand RemovePacketCommand { get; }
    public IRelayCommand ClearPacketsCommand { get; }
    public IRelayCommand LoadSpammerCommand { get; }
    public IRelayCommand SaveSpammerCommand { get; }
    public IRelayCommand SendPacketCommand { get; }
    public IRelayCommand ToggleSpammerCommand { get; }
    public IAsyncRelayCommand SpammerCommand { get; }

    private void ToggleSpammer()
    {
        if (SpammerCommand.IsRunning)
            SpammerCommand.Cancel();
        else
            SpammerCommand.ExecuteAsync(null);
    }

    private async Task Spammer(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            for (int i = 0; i < _packets.Count; i++)
            {
                SelectedIndex = i;
                Send(_packets[i]);
                await Task.Delay(_spamDelay, token);
            }
        }
    }

    private void SendPacket()
    {
        if (string.IsNullOrWhiteSpace(PacketText))
            return;

        Send(PacketText);
    }

    private void Send(string packet)
    {
        bool json = packet.StartsWith("{");
        if (SendToClient)
        {
            _send.ClientPacket(packet, json ? "json" : "str");
        }
        else
        {
            _send.Packet(packet, json ? "Json" : "String");
        }
    }

    private void RemovePacket()
    {
        if (SelectedIndex < 0)
            return;

        Packets.RemoveAt(SelectedIndex);
    }

    private void AddPacket()
    {
        if (string.IsNullOrWhiteSpace(PacketText))
            return;

        Packets.Add(PacketText);
    }

    private void SaveSpammer()
    {
        _fileDialog.SaveText(_packets);
    }

    private void LoadSpammer()
    {
        IEnumerable<string>? packets = _fileDialog.OpenText();
        if (packets is null)
            return;
        Packets.Clear();
        Packets.AddRange(packets);
    }
}
