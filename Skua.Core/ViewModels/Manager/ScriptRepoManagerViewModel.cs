using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models.GitHub;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels.Manager;

public partial class ScriptRepoManagerViewModel : BotControlViewModelBase
{
    public ScriptRepoManagerViewModel(IGetScriptsService getScripts, IProcessService processService)
        : base("Get Scripts", 800, 450)
    {
        _getScriptsService = getScripts;
        _processService = processService;
        OpenScriptFolderCommand = new RelayCommand(_processService.OpenVSC);
        // Set this to true to indicate we're in manager mode
        IsManagerMode = true;
    }

    protected override void OnActivated()
    {
        RefreshScriptsList();
        // Automatically refresh scripts from repository when window is opened
        if (!RefreshScriptsCommand.IsRunning)
        {
            _ = RefreshScriptsCommand.ExecuteAsync(null);
        }
    }

    private readonly IGetScriptsService _getScriptsService;
    private readonly IProcessService _processService;

    [ObservableProperty]
    private bool _isManagerMode;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DownloadedQuantity), nameof(OutdatedQuantity), nameof(ScriptQuantity), nameof(BotScriptQuantity))]
    private RangedObservableCollection<ScriptInfoManagerViewModel> _scripts = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DownloadedQuantity), nameof(OutdatedQuantity), nameof(ScriptQuantity), nameof(BotScriptQuantity))]
    private ScriptInfoManagerViewModel? _selectedItem;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _progressReportMessage = string.Empty;

    public int DownloadedQuantity => _getScriptsService?.Downloaded ?? 0;
    public int OutdatedQuantity => _getScriptsService?.Outdated ?? 0;
    public int ScriptQuantity => _getScriptsService?.Total ?? 0;
    public int BotScriptQuantity => _scripts.Count;
    public IRelayCommand OpenScriptFolderCommand { get; }

    [RelayCommand]
    private void OpenScript()
    {
        if (SelectedItem is null || !SelectedItem.Downloaded)
            return;

        StrongReferenceMessenger.Default.Send<EditScriptMessage, int>(new(SelectedItem.ManagerLocalFile), (int)MessageChannels.ScriptStatus);
    }

    [RelayCommand]
    private async Task RefreshScripts(CancellationToken token)
    {
        IsBusy = true;
        try
        {
            await Task.Run(async () =>
            {
                Progress<string> progress = new(ProgressHandler);
                await _getScriptsService.RefreshScriptsAsync(progress, token);
            }, token);
        }
        catch { }
        RefreshScriptsList();
    }

    private void RefreshScriptsList()
    {
        _scripts.Clear();
        if (_getScriptsService?.Scripts != null)
        {
            foreach (ScriptInfo script in _getScriptsService.Scripts)
            {
                if (script?.Name != null && !script.Name.Equals("null"))
                {
                    if (script.Description?.Equals("null") == true)
                        script.Description = "No description provided.";

                    if (script.Tags?.Contains("null") == true && (script.Tags.Length == 1))
                        script.Tags = new[] { "no-tags" };
                    else if (script.Tags == null)
                        script.Tags = new[] { "no-tags" };

                    _scripts.Add(new(script));
                }
            }
        }

        OnPropertyChanged(nameof(Scripts));
        OnPropertyChanged(nameof(DownloadedQuantity));
        OnPropertyChanged(nameof(OutdatedQuantity));
        OnPropertyChanged(nameof(ScriptQuantity));
        OnPropertyChanged(nameof(BotScriptQuantity));
        IsBusy = false;
    }

    public void ProgressHandler(string message)
    {
        ProgressReportMessage = message;
    }

    [RelayCommand]
    private async Task Delete()
    {
        IsBusy = true;
        if (_selectedItem is null)
            return;
        ProgressReportMessage = $"Deleting {_selectedItem.FileName}.";
        await _getScriptsService.DeleteScriptAsync(_selectedItem.Info);
        ProgressReportMessage = $"Deleted {_selectedItem.FileName}.";
        _selectedItem.Downloaded = false;
        OnPropertyChanged(nameof(DownloadedQuantity));
        OnPropertyChanged(nameof(OutdatedQuantity));
        OnPropertyChanged(nameof(ScriptQuantity));
        OnPropertyChanged(nameof(BotScriptQuantity));
        IsBusy = false;
    }

    [RelayCommand]
    private async Task Download()
    {
        IsBusy = true;
        if (_selectedItem is null)
            return;
        ProgressReportMessage = $"Downloading {_selectedItem.FileName}.";
        await _getScriptsService.ManagerDownloadScriptAsync(_selectedItem.Info);
        ProgressReportMessage = $"Downloaded {_selectedItem.FileName}.";
        _selectedItem.Downloaded = true;
        OnPropertyChanged(nameof(DownloadedQuantity));
        OnPropertyChanged(nameof(OutdatedQuantity));
        OnPropertyChanged(nameof(ScriptQuantity));
        OnPropertyChanged(nameof(BotScriptQuantity));
        IsBusy = false;
    }

    [RelayCommand]
    private async Task UpdateAll()
    {
        IsBusy = true;
        ProgressReportMessage = "Updating scripts...";
        int count = await _getScriptsService.ManagerDownloadAllWhereAsync(s => s.Outdated);
        ProgressReportMessage = $"Updated {count} scripts.";
        RefreshScriptsList();
    }

    [RelayCommand]
    private async Task DownloadAll()
    {
        IsBusy = true;
        ProgressReportMessage = "Downloading outdated/missing scripts...";
        int count = await Task.Run(async () => await _getScriptsService.ManagerDownloadAllWhereAsync(s => !s.Downloaded || s.Outdated));
        ProgressReportMessage = $"Downloaded {count} scripts.";
        RefreshScriptsList();
    }

    [RelayCommand]
    public void CancelTask()
    {
        if (RefreshScriptsCommand.IsRunning)
            RefreshScriptsCommand.Cancel();
        else if (DownloadAllCommand.IsRunning)
            DownloadAllCommand.Cancel();
        else if (UpdateAllCommand.IsRunning)
            UpdateAllCommand.Cancel();
        else if (DownloadCommand.IsRunning)
            DownloadCommand.Cancel();
        else if (DeleteCommand.IsRunning)
            DeleteCommand.Cancel();
        else
            ProgressReportMessage = string.Empty;
    }
}

// ViewModel wrapper for ScriptInfo specifically for Manager mode
public partial class ScriptInfoManagerViewModel : ObservableObject
{
    public ScriptInfoManagerViewModel(ScriptInfo info)
    {
        Info = info;
        InfoTags = info.Tags;
        // Check for manager-specific location
        Downloaded = File.Exists(info.ManagerLocalFile);
        Outdated = info.Outdated;
    }

    public ScriptInfo Info { get; }

    [ObservableProperty]
    private bool _downloaded;

    [ObservableProperty]
    private bool _outdated;

    public string[] InfoTags { get; }

    public string FileName => Info.Name;
    public string ScriptPath => Info.RelativePath;
    public string ManagerLocalFile => Info.ManagerLocalFile;

    // Load script command - sends message to load the script
    [RelayCommand]
    private void LoadScript()
    {
        if (Downloaded)
        {
            StrongReferenceMessenger.Default.Send<LoadScriptMessage, int>(
                new(ManagerLocalFile),
                (int)MessageChannels.ScriptStatus);
        }
    }
}