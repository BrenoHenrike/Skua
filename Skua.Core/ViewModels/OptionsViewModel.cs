using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Interfaces.Services;
using Skua.Core.Models.Servers;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels;
public class OptionsViewModel : BotControlViewModelBase, IDisposable
{
    private readonly IScriptOption Options;
    private readonly ISettingsService Settings;
    private readonly IScriptServers Servers;

    public ObservableCollection<OptionItemViewModel> OptionItems { get; }
    private IList<Server> _servers => Servers.CachedServers;
    public RangedObservableCollection<Server> ServersList { get; }
    private Server _selectedServer;
    public Server SelectedServer
    {
        get { return _selectedServer; }
        set { SetProperty(ref _selectedServer, value); }
    }

    public IRelayCommand ResetOptionsCommand { get; }
    public IRelayCommand SaveOptionsCommand { get; }

    public OptionsViewModel(IEnumerable<OptionItemViewModel> optionItems, IScriptServers servers, ISettingsService settings, IScriptOption options)
        : base("Game Options")
    {
        Options = options;
        Settings = settings;
        Servers = servers;
        ServersList = new(Servers.CachedServers);
        OptionItems = new(optionItems);
        Servers.PropertyChanged += Servers_PropertyChanged;
        ResetOptionsCommand = new RelayCommand(ResetOptions);
        SaveOptionsCommand = new RelayCommand(SaveOptions);
    }

    private void Servers_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Servers.CachedServers))
            ServersList.ReplaceRange(_servers);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Servers.PropertyChanged -= Servers_PropertyChanged;
    }

    public void ResetOptions()
    {
        Options.ResetOptions();
    }

    public void SaveOptions()
    {
        StringCollection options = new();
        Settings.SetValue("GameOptions", options);
    }
}
