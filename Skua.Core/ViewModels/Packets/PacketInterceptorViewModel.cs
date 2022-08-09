using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging.Messages;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Servers;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels;
public partial class PacketInterceptorViewModel : BotControlViewModelBase
{
    public PacketInterceptorViewModel(ICaptureProxy gameProxy, IScriptServers server)
        : base("Packet Interceptor")
    {
        Messenger.Register<PacketInterceptorViewModel, PropertyChangedMessage<bool>>(this, RunningChanged);
        _gameProxy = gameProxy;
        _server = server;
        ClearPacketsCommand = new RelayCommand(Packets.Clear);
        SynchronizationContext? context = SynchronizationContext.Current;
        void addFunc(InterceptedPacketViewModel st) => context?.Send(obj => Packets.Add((InterceptedPacketViewModel)obj!), st);
        _logger = new InterceptorLogger(addFunc);
        IsLogging = true;
    }

    private readonly ICaptureProxy _gameProxy;
    private readonly IScriptServers _server;
    private readonly InterceptorLogger _logger;
    [ObservableProperty]
    private Server? _selectedServer;
    [ObservableProperty]
    private RangedObservableCollection<InterceptedPacketViewModel> _packets = new();

    private bool _isLogging;
    public bool IsLogging
    {
        get { return _isLogging; }
        set
        {
            if (SetProperty(ref _isLogging, value))
            {
                if (value)
                    _gameProxy.Interceptors.Add(_logger);
                else
                    _gameProxy.Interceptors.Remove(_logger);
            }
        }
    }
    public bool Running => _gameProxy.Running;
    public List<Server> ServerList => _server.ServerList;
    public IRelayCommand ClearPacketsCommand { get; }


    [RelayCommand]
    private void ConnectInterceptor()
    {
        if (_gameProxy.Running)
        {
            _gameProxy.Stop();
            OnPropertyChanged(nameof(Running));
            return;
        }

        if (SelectedServer is null)
            return;

        IPAddress ip = IPAddress.TryParse(SelectedServer.IP, out IPAddress? addr) ? addr : Dns.GetHostEntry(SelectedServer.IP).AddressList[0];
        _gameProxy.Destination = new IPEndPoint(ip, 5588);
        _gameProxy.Start();
        _server.Logout();
        _server.Login();
        _server.ConnectIP("127.0.0.1");
        OnPropertyChanged(nameof(Running));
    }

    private void RunningChanged(PacketInterceptorViewModel recipient, PropertyChangedMessage<bool> message)
    {
        if (message.PropertyName == nameof(ICaptureProxy.Running))
            recipient.OnPropertyChanged(nameof(recipient.Running));
    }
}

public class InterceptorLogger : IInterceptor
{
    private readonly Action<InterceptedPacketViewModel> _addFunc;

    public int Priority => int.MaxValue;
    public InterceptorLogger(Action<InterceptedPacketViewModel> addFunc)
    {
        _addFunc = addFunc;
    }

    public void Intercept(MessageInfo message, bool outbound)
    {
        _addFunc(new(message.Content, message.Send ? outbound : null));
    }
}
