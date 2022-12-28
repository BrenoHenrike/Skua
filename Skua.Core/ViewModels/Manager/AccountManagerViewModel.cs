using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Utils;
using System.Diagnostics;

namespace Skua.Core.ViewModels.Manager;
public sealed partial class AccountManagerViewModel : BotControlViewModelBase
{
    public AccountManagerViewModel(ISettingsService settingsService, IDialogService dialogService)
        : base("Accounts")
    {
        _settingsService = settingsService;
        _dialogService = dialogService;
        Accounts = new();
        // TODO client path
        Clients = new();
        Clients.Add(new ClientItemViewModel() { Name = "Follower", Path = "C:\\Repo\\Skua\\Skua.App.WPF.Follower\\bin\\Debug\\net6.0-windows\\Skua.App.WPF.Follower.exe" });
        Clients.Add(new ClientItemViewModel() { Name = "Sync", Path = "C:\\Repo\\Skua\\Skua.App.WPF.Sync\\bin\\Debug\\net6.0-windows\\Skua.App.WPF.Sync.exe" });
    }

    private readonly ISettingsService _settingsService;
    private readonly IDialogService _dialogService;

    public RangedObservableCollection<AccountItemViewModel> Accounts { get; }
    public List<ClientItemViewModel> Clients { get; }

    [ObservableProperty]
    private string _usernameInput;
    [ObservableProperty]
    private string _displayNameInput;

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
            DisplayName = string.IsNullOrEmpty(DisplayNameInput) ? $"{Accounts.Count + 1}" : DisplayNameInput
        });

        Messenger.Send<ClearPasswordBoxMessage>();
        UsernameInput = string.Empty;
        DisplayNameInput = string.Empty;

        // TODO save accounts
    }

    [RelayCommand]
    public async Task StartAccounts()
    {
        // TODO show dialog to choose between clients
        //_dialogService
        // TODO manage ids
        foreach (var acc in Accounts.Where(a => a.UseCheck).ToList())
        {
            ProcessStartInfo psi = new(Clients.Last().Path)
            {
                Arguments = $"--usr \"{acc.Username}\" --psw \"{acc.Password}\" --id \"{acc.DisplayName}\"",
                WorkingDirectory = Path.GetDirectoryName(Clients.Last().Path)
            };
            Process.Start(psi);
            await Task.Delay(1000);
        }
    }
}
