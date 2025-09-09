using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels.Manager;

public class ManagerOptionsViewModel : ObservableObject
{
    public ManagerOptionsViewModel(List<DisplayOptionItemViewModelBase> options, ISettingsService settingsService, IFileDialogService fileService)
    {
        ManagerOptions = options;
        _settingsService = settingsService;
        _fileService = fileService;

        string initialDirectory = _settingsService.Get("ClientDownloadPath", string.Empty);
        if (string.IsNullOrEmpty(initialDirectory))
        {
            initialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            if (Directory.Exists(initialDirectory))
                _settingsService.Set("ClientDownloadPath", initialDirectory);
            else
                _settingsService.Set("ClientDownloadPath", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)));
        }

        _downloadPath = _settingsService.Get("ClientDownloadPath", string.Empty);
        ChangeDownloadPathCommand = new RelayCommand(ChangeDownloadPath);
        OpenGHAuthCommand = new RelayCommand(OpenGHAuthDialog);
    }

    public List<DisplayOptionItemViewModelBase> ManagerOptions { get; }

    private string _downloadPath;
    private readonly ISettingsService _settingsService;
    private readonly IFileDialogService _fileService;

    public string DownloadPath
    {
        get { return _downloadPath; }
        set
        {
            if (SetProperty(ref _downloadPath, value))
                _settingsService.Set("ClientDownloadPath", value);
        }
    }

    public IRelayCommand ChangeDownloadPathCommand { get; }
    public IRelayCommand OpenGHAuthCommand { get; }

    private void ChangeDownloadPath()
    {
        string initialDirectory = _settingsService.Get("ClientDownloadPath", string.Empty);
        if (string.IsNullOrEmpty(initialDirectory))
        {
            initialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            if (Directory.Exists(initialDirectory))
                _settingsService.Set("ClientDownloadPath", initialDirectory);
            else
                _settingsService.Set("ClientDownloadPath", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)));
        }

        string? folderPath = _fileService.OpenFolder(initialDirectory);
        if (!string.IsNullOrEmpty(folderPath))
        {
            _settingsService.Set("ClientDownloadPath", folderPath);
            DownloadPath = folderPath;
        }
    }

    private void OpenGHAuthDialog()
    {
        string? previousToken = _settingsService.Get<string>("UserGitHubToken");
        Ioc.Default.GetRequiredService<IDialogService>().ShowDialog(Ioc.Default.GetRequiredService<GitHubAuthViewModel>());

        string? token = _settingsService.Get<string>("UserGitHubToken");
        if (!string.IsNullOrWhiteSpace(token) && token != previousToken)
            HttpClients.UserGitHubClient = new(token);
    }
}