using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models.GitHub;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels;
public partial class ScriptRepoViewModel : BotControlViewModelBase
{
    public ScriptRepoViewModel(IGetScriptsService getScripts, IProcessService processService)
        : base("Get Scripts")
    {
        _getScriptsService = getScripts;
        _processService = processService;
        OpenScriptFolderCommand = new RelayCommand(_processService.OpenVSC);
    }

    protected override void OnActivated()
    {
        RefreshScriptsList();
    }

    private readonly IGetScriptsService _getScriptsService;
    private readonly IProcessService _processService;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DownloadedQuantity), nameof(OutdatedQuantity), nameof(ScriptQuantity))]
    private RangedObservableCollection<ScriptInfoViewModel> _scripts = new();
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DownloadedQuantity), nameof(OutdatedQuantity), nameof(ScriptQuantity))]
    private ScriptInfoViewModel? _selectedItem;
    [ObservableProperty]
    private bool _isBusy;
    [ObservableProperty]
    private string _progressReportMessage = string.Empty;

    public int DownloadedQuantity => _getScriptsService.Downloaded;
    public int OutdatedQuantity => _getScriptsService.Outdated;
    public int ScriptQuantity => _scripts.Count;
    public IRelayCommand OpenScriptFolderCommand { get; }

    [RelayCommand]
    private void StartScript()
    {
        if (SelectedItem is null || !SelectedItem.Downloaded)
            return;

        StrongReferenceMessenger.Default.Send<StartScriptMessage, int>(new(SelectedItem.LocalFile), (int)MessageChannels.ScriptStatus);
    }

    [RelayCommand]
    private void OpenScript()
    {
        if (SelectedItem is null || !SelectedItem.Downloaded)
            return;

        StrongReferenceMessenger.Default.Send<EditScriptMessage, int>(new(SelectedItem.LocalFile), (int)MessageChannels.ScriptStatus);
    }

    [RelayCommand]
    private void LoadScript()
    {
        if (SelectedItem is null || !SelectedItem.Downloaded)
            return;

        StrongReferenceMessenger.Default.Send<LoadScriptMessage, int>(new(SelectedItem.LocalFile), (int)MessageChannels.ScriptStatus);
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
        foreach (ScriptInfo script in _getScriptsService.Scripts)
            _scripts.Add(new(script));
        OnPropertyChanged(nameof(Scripts));
        OnPropertyChanged(nameof(DownloadedQuantity));
        OnPropertyChanged(nameof(OutdatedQuantity));
        OnPropertyChanged(nameof(ScriptQuantity));
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
        IsBusy = false;
    }

    [RelayCommand]
    private async Task Download()
    {
        IsBusy = true;
        if (_selectedItem is null)
            return;
        ProgressReportMessage = $"Downloading {_selectedItem.FileName}.";
        await _getScriptsService.DownloadScriptAsync(_selectedItem.Info);
        ProgressReportMessage = $"Downloaded {_selectedItem.FileName}.";
        _selectedItem.Downloaded = true;
        OnPropertyChanged(nameof(DownloadedQuantity));
        OnPropertyChanged(nameof(OutdatedQuantity));
        OnPropertyChanged(nameof(ScriptQuantity));
        IsBusy = false;
    }

    [RelayCommand]
    private async Task UpdateAll()
    {
        IsBusy = true;
        ProgressReportMessage = "Updating scripts...";
        int count = await _getScriptsService.DownloadAllWhereAsync(s => s.Outdated);
        ProgressReportMessage = $"Updated {count} scripts.";
        RefreshScriptsList();
    }

    [RelayCommand]
    private async Task DownloadAll()
    {
        IsBusy = true;
        ProgressReportMessage = "Downloading outdated/missing scripts...";
        int count = await Task.Run(async () => await _getScriptsService.DownloadAllWhereAsync(s => !s.Downloaded || s.Outdated));
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
