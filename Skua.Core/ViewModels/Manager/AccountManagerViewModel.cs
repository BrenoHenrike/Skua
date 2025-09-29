using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using Skua.Core.AppStartup;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models;
using Skua.Core.Models.Servers;
using Skua.Core.Utils;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Skua.Core.ViewModels.Manager;

public sealed partial class AccountManagerViewModel : BotControlViewModelBase
{
    public AccountManagerViewModel(ISettingsService settingsService, IDialogService dialogService, IFileDialogService fileService)
        : base("Accounts")
    {
        Messenger.Register<AccountManagerViewModel, RemoveAccountMessage>(this, (r, m) => r._RemoveAccount(m.Account));
        Messenger.Register<AccountManagerViewModel, AccountSelectedMessage>(this, AccountSelected);
        // Register to receive LoadScriptMessage from ScriptRepoManagerViewModel
        StrongReferenceMessenger.Default.Register<AccountManagerViewModel, LoadScriptMessage, int>(this,
            (int)MessageChannels.ScriptStatus, (r, m) => r.HandleLoadScript(m));
        _settingsService = settingsService;
        _dialogService = dialogService;
        _fileService = fileService;
        ServerList = new();
        Task.Run(async () => await _GetServers());
        Accounts = new();
        _GetSavedAccounts();
        _syncThemes = _settingsService.Get("SyncThemes", false);
        // TODO different clients path
    }

    private const string _separator = "{=}";
    private readonly string[] _arrSeparator = { _separator };
    private readonly string _exePath = Path.Combine(AppContext.BaseDirectory, "Skua.exe");

    private readonly ISettingsService _settingsService;
    private readonly IDialogService _dialogService;
    private readonly IFileDialogService _fileService;

    public RangedObservableCollection<AccountItemViewModel> Accounts { get; }

    [ObservableProperty]
    private string _scriptPath = string.Empty;

    [ObservableProperty]
    private bool _startWithScript;

    [ObservableProperty]
    private int _columns = 1;

    [ObservableProperty]
    private int _selectedAccountQuant;

    [ObservableProperty]
    private string _usernameInput;

    [ObservableProperty]
    private string _displayNameInput;

    [ObservableProperty]
    private Server _selectedServer;

    [ObservableProperty]
    private bool _useNameAsDisplay;

    private List<Server> _cachedServers = new();

    [ObservableProperty]
    private RangedObservableCollection<Server> _serverList;

    private bool _syncThemes;

    public string PasswordInput { private get; set; }

    [RelayCommand]
    public void ChangeScriptPath()
    {
        string? folderPath = _fileService.OpenFile(ClientFileSources.SkuaScriptsDIR, "Skua Scripts (*.cs)|*.cs");

        if (!string.IsNullOrEmpty(folderPath))
            ScriptPath = folderPath;
    }

    [RelayCommand]
    public void AddAccount()
    {
        if (string.IsNullOrEmpty(UsernameInput) || string.IsNullOrEmpty(PasswordInput))
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

        UsernameInput = string.Empty;
        DisplayNameInput = string.Empty;
        Messenger.Send<ClearPasswordBoxMessage>();

        _SaveAccounts();
    }

    [RelayCommand]
    public async Task StartAccounts()
    {
        // TODO show dialog to choose between clients
        // TODO manage ids for sync in the future

        _syncThemes = _settingsService.Get("SyncThemes", false);
        foreach (var acc in Accounts.Where(a => a.UseCheck))
        {
            _LaunchAcc(acc.Username, acc.Password, acc.DisplayName);
            await Task.Delay(1000);
        }
    }

    [RelayCommand]
    public async Task StartAllAccounts()
    {
        _syncThemes = _settingsService.Get("SyncThemes", false);
        foreach (var acc in Accounts)
        {
            _LaunchAcc(acc.Username, acc.Password, acc.DisplayName);
            await Task.Delay(1000);
        }
    }

    [RelayCommand]
    public async Task RemoveAccounts()
    {
        foreach (var acc in Accounts.Where(a => a.UseCheck).ToList())
            _RemoveAccount(acc);

        _SaveAccounts();
    }

    [RelayCommand]
    public void OpenGetScripts()
    {
        var services = Ioc.Default.GetService<IServiceProvider>();
        if (services != null)
        {
            ManagedWindows.RegisterForManager(services);
        }

        var windowService = Ioc.Default.GetService<IWindowService>();
        if (windowService != null)
        {
            windowService.ShowManagedWindow("Script Repo Manager");
        }
    }

    private void _RemoveAccount(AccountItemViewModel account)
    {
        Accounts.Remove(account);
        SelectedAccountQuant--;

        _SaveAccounts();
    }

    private void _SaveAccounts()
    {
        StringCollection accs = new();
        foreach (var account in Accounts)
            accs.Add($"{account.DisplayName}{_separator}{account.Username}{_separator}{account.Password}");

        _settingsService.Set("ManagedAccounts", accs);
    }

    private void _LaunchAcc(string username, string password, string displayName = null)
    {
        try
        {
            ProcessStartInfo psi = new(_exePath)
            {
                ArgumentList =
                {
                    "-u",
                    username,
                    "-p",
                    password,
                    "-s",
                    SelectedServer?.Name ?? "Twilly"
                },
                WorkingDirectory = AppContext.BaseDirectory
            };

            if (_syncThemes)
            {
                psi.ArgumentList.Add("--use-theme");
                psi.ArgumentList.Add(_settingsService.Get("CurrentTheme", "no-theme"));
            }

            if (StartWithScript)
            {
                psi.ArgumentList.Add("--run-script");
                psi.ArgumentList.Add(ScriptPath);
            }

            var process = Process.Start(psi);
            if (process != null)
            {
                // Send message to LauncherViewModel to add this process with the account display name
                string accountName = !string.IsNullOrEmpty(displayName) ? displayName : username;
                StrongReferenceMessenger.Default.Send(new AddProcessMessage(process, accountName));
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessageBox($"Error while starting process: {ex.Message}", "Launch Error");
        }
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

    private async Task _GetServers()
    {
        string? response = await HttpClients.GetGHClient()
            .GetStringAsync($"http://content.aq.com/game/api/data/servers");

        if (response is null)
            return;

        _cachedServers = JsonConvert.DeserializeObject<List<Server>>(response)!;
        ServerList.AddRange(_cachedServers);
        SelectedServer = ServerList[0];
    }

    private void AccountSelected(AccountManagerViewModel recipient, AccountSelectedMessage message)
    {
        if (message.Add)
            recipient.SelectedAccountQuant++;
        else
            recipient.SelectedAccountQuant--;
    }

    private void HandleLoadScript(LoadScriptMessage message)
    {
        if (string.IsNullOrEmpty(message.Path))
            return;

        // Set the script path to the loaded script
        ScriptPath = message.Path;

        // Optionally show a confirmation message
        _dialogService.ShowMessageBox($"Script loaded: {Path.GetFileName(message.Path)}", "Script Loaded");
    }
}