using System.Net;
using Microsoft.Toolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Servers;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels;
public class PacketInterceptorViewModel : BotControlViewModelBase, IDisposable
{
    public PacketInterceptorViewModel(ICaptureProxy gameProxy, IScriptServers server)
        : base("Packet Interceptor")
    {
        _gameProxy = gameProxy;
        _server = server;
        _server.PropertyChanged += _server_PropertyChanged;
        _gameProxy.PropertyChanged += _gameProxy_PropertyChanged;
        ConnectInterceptorCommand = new RelayCommand(Connect);
        ClearPacketsCommand = new RelayCommand(Packets.Clear);
        SynchronizationContext? context = SynchronizationContext.Current;
        void addFunc(InterceptedPacketViewModel st) => context?.Send(obj => Packets.Add((InterceptedPacketViewModel)obj!), st);
        _logger = new InterceptorLogger(addFunc);
        IsLogging = true;
    }

    private readonly ICaptureProxy _gameProxy;
    private readonly IScriptServers _server;
    private readonly InterceptorLogger _logger;

    public bool Running => _gameProxy.Running;
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
    private Server? _selectedServer;
    public Server? SelectedServer
    {
        get { return _selectedServer; }
        set { SetProperty(ref _selectedServer, value); }
    }
    public List<Server> ServerList => _server.ServerList;
    private RangedObservableCollection<InterceptedPacketViewModel> _packets = new();

    public RangedObservableCollection<InterceptedPacketViewModel> Packets
    {
        get { return _packets; }
        set { SetProperty(ref _packets, value); }
    }

    public IRelayCommand ConnectInterceptorCommand { get; }
    public IRelayCommand ClearPacketsCommand { get; }

    private void Connect()
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

    private void _gameProxy_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ICaptureProxy.Running))
            OnPropertyChanged(nameof(Running));
    }

    private void _server_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_server.ServerList))
            OnPropertyChanged(nameof(ServerList));
    }
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _gameProxy.Stop();
        _gameProxy.PropertyChanged -= _gameProxy_PropertyChanged;
        _server.PropertyChanged -= _server_PropertyChanged;
    }
}

public partial class InterceptorLogger : IInterceptor
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
