using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Utils;
using System.Diagnostics;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Skua.Core.Models.Servers;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Skua.Core.ViewModels.Manager;
public sealed partial class AccountManagerViewModel : BotControlViewModelBase, INotifyPropertyChanged
{
    public AccountManagerViewModel(ISettingsService settingsService, IDialogService dialogService)
        : base("Accounts")
    {
        Messenger.Register<AccountManagerViewModel, RemoveAccountMessage>(this, (r, m) => r._RemoveAccount(m.Account));
        _settingsService = settingsService;
        _dialogService = dialogService;
        Task.Run(async () => await _GetServers());
        Accounts = new();
        _GetSavedAccounts();

        // TODO different clients path
    }

    private const string _separator = "{=}";
    private readonly string[] _arrSeparator = { _separator };

    private readonly ISettingsService _settingsService;
    private readonly IDialogService _dialogService;

    public RangedObservableCollection<AccountItemViewModel> Accounts { get; }

    [ObservableProperty]
    private string _usernameInput;
    [ObservableProperty]
    private string _displayNameInput;
    [ObservableProperty]
    private string _selectedServer;
    [ObservableProperty]
    private bool _useNameAsDisplay;

    public string PasswordInput { private get; set; }

    [RelayCommand]
    public void AddAccount()
    {
        if(string.IsNullOrEmpty(UsernameInput) || string.IsNullOrEmpty(PasswordInput))
        {
            _dialogService.ShowMessageBox("Username and/or password must not be empty", "Missing Input");
            return;
        }

        Accounts.Add(new AccountItemViewModel()
        {
            Username = UsernameInput,
            Password = PasswordInput,
            DisplayName = string.IsNullOrEmpty(DisplayNameInput) && !UseNameAsDisplay
                ? $"{Accounts.Count + 1}"
                : UseNameAsDisplay ? UsernameInput : DisplayNameInput
        });

        Messenger.Send<ClearPasswordBoxMessage>();
        UsernameInput = string.Empty;
        DisplayNameInput = string.Empty;

        _SaveAccounts();
    }

    [RelayCommand]
    public async Task StartAccounts()
    {
        // TODO show dialog to choose between clients
        // TODO manage ids for sync in the future

        foreach (var acc in Accounts.Where(a => a.UseCheck))
            _LaunchAcc(acc.Username, acc.Password);
    }

    [RelayCommand]
    public async Task StartAllAccounts()
    {
        foreach (var acc in Accounts)
            _LaunchAcc(acc.Username, acc.Password);
    }

    [RelayCommand]
    public async Task RemoveAccounts()
    {
        foreach (var acc in Accounts.Where(a => a.UseCheck).ToList())
            _RemoveAccount(acc);

        _SaveAccounts();
    }

    private void _RemoveAccount(AccountItemViewModel account)
    {
        Accounts.Remove(account);

        _SaveAccounts();
    }

    private void _SaveAccounts()
    {
        StringCollection accs = new();
        foreach (var account in Accounts)
            accs.Add($"{account.DisplayName}{_separator}{account.Username}{_separator}{account.Password}");

        _settingsService.Set("ManagedAccounts", accs);
    }

    private void _LaunchAcc(string username, string password)
    {
        ProcessStartInfo psi = new(Path.Combine(AppContext.BaseDirectory, "Skua.exe"))
        {
            Arguments = $"--usr \"{username}\" --psw \"{password}\" --sv \"{_selectedServer}\"",
            WorkingDirectory = AppContext.BaseDirectory
        };
        Process.Start(psi);
    }

    private void _GetSavedAccounts()
    {
        Accounts.Clear();
        var accs = _settingsService.Get<StringCollection>("ManagedAccounts");
        if (accs is null)
            return;

        foreach (var acc in accs)
        {
            if (acc is null)
                continue;
            string[] info = acc.Split(_arrSeparator, StringSplitOptions.None);
            Accounts.Add(new AccountItemViewModel()
            {
                DisplayName = info[0],
                Username = info[1],
                Password = info[2]
            });
        }
    }

    private List<Server> _cachedServers = new();
    private ObservableCollection<string> _serverList;
    public ObservableCollection<string> ServersList
    {
        get
        {
            return _serverList;
        }
        set
        {
            _serverList = value;
            NotifyPropertyChanged("ServersList");
        }
    }

    private async Task _GetServers()
    {
        string? response = await HttpClients.GetGHClient()
            .GetStringAsync($"http://content.aq.com/game/api/data/servers");

        if (response is null)
            return;

        _cachedServers = JsonConvert.DeserializeObject<List<Server>>(response)!;
        _serverList = new ObservableCollection<string>(_cachedServers.Select(s => s.Name).ToList());
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void NotifyPropertyChanged(string name)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
