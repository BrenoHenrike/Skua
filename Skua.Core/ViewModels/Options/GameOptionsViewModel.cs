using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Skua.Core.Interfaces;
using Skua.Core.Models.Servers;

namespace Skua.Core.ViewModels;
public class GameOptionsViewModel : BotControlViewModelBase
{
    private readonly IScriptServers _servers;
    private readonly IScriptOption _options;

    public GameOptionsViewModel(List<DisplayOptionItemViewModelBase> gameOptions, IScriptServers servers, IScriptOption options)
        : base("Game Options", 420, 500)
    {
        _servers = servers;
        _options = options;
        GameOptions = gameOptions;
        ResetOptionsCommand = new RelayCommand(_options.Reset);
        ResetDefaultOptionsCommand = new RelayCommand(_options.ResetToDefault);
        SaveOptionsCommand = new RelayCommand(_options.Save);
    }

    protected override void OnActivated()
    {
        Messenger.Register<GameOptionsViewModel, PropertyChangedMessage<List<Server>>>(this, ServersChanged);
        Messenger.Register<GameOptionsViewModel, PropertyChangedMessage<string>>(this, OptionServerChanged);
    }

    public List<DisplayOptionItemViewModelBase> GameOptions { get; }
    public List<string> ServersList => _servers.CachedServers.Select(s => s.Name).ToList();
    private string? _selectedServer;
    public string? SelectedServer
    {
        get { return _selectedServer; }
        set
        {
            if (SetProperty(ref _selectedServer, value) && value is not null && _options.ReloginServer != value)
                _options.ReloginServer = value;
        }
    }
    private int _columns = 2;
    public int Columns
    {
        get { return _columns; }
        set { SetProperty(ref _columns, value); }
    }


    public IRelayCommand ResetOptionsCommand { get; }
    public IRelayCommand ResetDefaultOptionsCommand { get; }
    public IRelayCommand SaveOptionsCommand { get; }

    private void ServersChanged(GameOptionsViewModel recipient, PropertyChangedMessage<List<Server>> message)
    {
        if (message.PropertyName == nameof(IScriptServers.CachedServers))
            recipient.OnPropertyChanged(nameof(recipient.ServersList));
    }

    private void OptionServerChanged(GameOptionsViewModel recipient, PropertyChangedMessage<string> message)
    {
        if (message.PropertyName == nameof(IScriptOption.ReloginServer) && message.NewValue != recipient.SelectedServer)
            recipient.SelectedServer = message.NewValue;
    }
}
