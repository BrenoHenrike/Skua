using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels;
public partial class PacketSpammerViewModel : BotControlViewModelBase
{
    public PacketSpammerViewModel(IScriptSend send, IFileDialogService fileDialog)
        : base("Packet Spammer", 700, 400)
    {
        _send = send;
        _fileDialog = fileDialog;
    }

    private readonly IScriptSend _send;
    private readonly IFileDialogService _fileDialog;
    [ObservableProperty]
    private RangedObservableCollection<string> _packets = new();
    [ObservableProperty]
    private bool _sendToClient;
    [ObservableProperty]
    private int _spamDelay = 1000;
    [ObservableProperty]
    private string _packetText = string.Empty;
    [ObservableProperty]
    private int _sendTimes = 1;
    [ObservableProperty]
    private int _selectedIndex = -1;

    [RelayCommand]
    private void ClearPackets()
    {
        Packets.Clear();
    }

    [RelayCommand]
    private void ToggleSpammer()
    {
        if (SpammerCommand.IsRunning)
            SpammerCommand.Cancel();
        else
            SpammerCommand.ExecuteAsync(null);
    }

    [RelayCommand]
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

    [RelayCommand]
    private void RemovePacket()
    {
        if (SelectedIndex < 0)
            return;

        Packets.RemoveAt(SelectedIndex);
    }

    [RelayCommand]
    private void AddPacket()
    {
        if (string.IsNullOrWhiteSpace(PacketText))
            return;

        Packets.Add(PacketText);
    }

    [RelayCommand]
    private void SaveSpammer()
    {
        _fileDialog.SaveText(_packets);
    }

    [RelayCommand]
    private void LoadSpammer()
    {
        IEnumerable<string>? packets = _fileDialog.OpenText();
        if (packets is null)
            return;
        Packets.Clear();
        Packets.AddRange(packets);
    }

    [RelayCommand]
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
            return;
        }

        _send.Packet(packet, json ? "Json" : "String");
    }
}
