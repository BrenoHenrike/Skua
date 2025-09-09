using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Servers;
using Skua.Core.Utils;
using System.Net;

namespace Skua.Core.ViewModels;

public partial class PacketInterceptorViewModel : BotControlViewModelBase
{
    public PacketInterceptorViewModel(ICaptureProxy gameProxy, IScriptServers server)
        : base("Packet Interceptor")
    {
        _gameProxy = gameProxy;
        _server = server;
        ClearPacketsCommand = new RelayCommand(Packets.Clear);
        SynchronizationContext? context = SynchronizationContext.Current;
        void addFunc(InterceptedPacketViewModel st) => context?.Send(obj => Packets.Add((InterceptedPacketViewModel)obj!), st);
        _logger = new InterceptorLogger(addFunc);
        IsLogging = true;
    }

    protected override void OnActivated()
    {
        Messenger.Register<PacketInterceptorViewModel, PropertyChangedMessage<bool>>(this, RunningChanged);
        OnPropertyChanged(nameof(Running));
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
    public List<Server> ServerList => _server.CachedServers;
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

        var options = Ioc.Default.GetRequiredService<IScriptOption>();
        bool relogin = options.AutoRelogin;
        options.AutoRelogin = false;
        IPAddress ip = IPAddress.TryParse(SelectedServer.IP, out IPAddress? addr) ? addr : Dns.GetHostEntry(SelectedServer.IP).AddressList[0];
        _gameProxy.Destination = new IPEndPoint(ip, 5588);
        _gameProxy.Start();
        _server.Logout();
        _server.Login();
        _server.ConnectIP("127.0.0.1");
        OnPropertyChanged(nameof(Running));
        options.AutoRelogin = relogin;
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