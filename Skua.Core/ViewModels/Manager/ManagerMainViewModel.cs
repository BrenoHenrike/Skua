using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels.Manager;
public partial class ManagerMainViewModel : ObservableRecipient
{
    public ManagerMainViewModel(IEnumerable<TabItemViewModel> tabs, IDialogService dialogService, ISettingsService settingsService)
    {
        Tabs = new(tabs);
        _selectedTab = Tabs[0];
        _dialogService = dialogService;
        _settingsService = settingsService;
        _isAuthenticated = !string.IsNullOrEmpty(_settingsService.Get<string>("UserGitHubToken"));
        _title = $"Skua Manager - {_settingsService.Get("ApplicationVersion", "0.0.0.0")}";
    }

    [ObservableProperty]
    private string _title;

    [RelayCommand]
    private void ShowManager()
    {
        StrongReferenceMessenger.Default.Send<ShowMainWindowMessage>();
    }

    [RelayCommand]
    private void CheckUpdate()
    {
        StrongReferenceMessenger.Default.Send<CheckClientUpdateMessage>();
    }

    [RelayCommand]
    private void UpdateScripts()
    {
        StrongReferenceMessenger.Default.Send<UpdateScriptsMessage>(new(false));
    }

    [RelayCommand]
    private void ResetScripts()
    {
        StrongReferenceMessenger.Default.Send<UpdateScriptsMessage>(new(true));
    }

    [ObservableProperty]
    private bool _isAuthenticated;
    private readonly IDialogService _dialogService;
    private readonly ISettingsService _settingsService;

    public RangedObservableCollection<TabItemViewModel> Tabs { get; set; }

    private TabItemViewModel _selectedTab;
    public TabItemViewModel SelectedTab
    {
        get { return _selectedTab; }
        set
        {
            var lastTab = _selectedTab.Content;
            if (SetProperty(ref _selectedTab, value))
            {
                if(lastTab is ObservableRecipient previous)
                    previous.IsActive = false;
                if(_selectedTab.Content is ObservableRecipient current)
                    current.IsActive = true;
            }
        }
    }

    [RelayCommand]
    private void OpenGHAuthDialog()
    {
        _dialogService.ShowDialog(Ioc.Default.GetRequiredService<GitHubAuthViewModel>());

        string? token = _settingsService.Get<string>("UserGitHubToken");
        if (!string.IsNullOrWhiteSpace(token))
        {
            HttpClients.UserGitHubClient = new(token);
            IsAuthenticated = true;
        }
    }
}
