using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models.GitHub;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels;
public partial class ScriptRepoViewModel : BotControlViewModelBase
{
    public ScriptRepoViewModel(IGetScriptsService getScripts, IProcessStartService processService)
        : base("Get Scripts")
    {
        _getScriptsService = getScripts;
        _processService = processService;
        RefreshScriptsCommand = new AsyncRelayCommand(Refresh);
        DownloadAllCommand = new AsyncRelayCommand(DownloadAll);
        UpdateAllCommand = new AsyncRelayCommand(UpdateAll);
        DownloadCommand = new AsyncRelayCommand(Download);
        DeleteCommand = new AsyncRelayCommand(Delete);
        CancelTaskCommand = new RelayCommand(CancelTask);
        LoadScriptCommand = new RelayCommand(LoadScript);
        OpenScriptCommand = new RelayCommand(OpenScript);
        StartScriptCommand = new RelayCommand(StartScriptAsync);
        OpenScriptFolderCommand = new RelayCommand(_processService.OpenVSC);
    }

    private readonly IGetScriptsService _getScriptsService;
    private readonly IProcessStartService _processService;

    public int DownloadedQuantity => _getScriptsService.Downloaded;
    public int OutdatedQuantity => _getScriptsService.Outdated;
    public int ScriptQuantity => _scripts.Count;
    [ObservableProperty]
    [AlsoNotifyChangeFor(nameof(DownloadedQuantity), nameof(OutdatedQuantity), nameof(ScriptQuantity))]
    private RangedObservableCollection<ScriptInfoViewModel> _scripts = new();
    [ObservableProperty]
    [AlsoNotifyChangeFor(nameof(DownloadedQuantity), nameof(OutdatedQuantity), nameof(ScriptQuantity))]
    private ScriptInfoViewModel? _selectedItem;
    [ObservableProperty]
    private bool _showSnackBar;
    [ObservableProperty]
    private bool _isBusy;
    [ObservableProperty]
    private string _progressReportMessage = string.Empty;

    public IAsyncRelayCommand UpdateAllCommand { get; }
    public IAsyncRelayCommand DownloadAllCommand { get; }
    public IAsyncRelayCommand DownloadCommand { get; }
    public IAsyncRelayCommand DeleteCommand { get; }
    public IAsyncRelayCommand RefreshScriptsCommand { get; }
    public IRelayCommand CancelTaskCommand { get; }
    public IRelayCommand LoadScriptCommand { get; }
    public IRelayCommand OpenScriptCommand { get; }
    public IRelayCommand StartScriptCommand { get; }
    public IRelayCommand OpenScriptFolderCommand { get; }

    private void StartScriptAsync()
    {
        if (SelectedItem is null)
            return;

        Messenger.Send<StartScriptMessage>(new(SelectedItem.LocalFile));
    }

    private void OpenScript()
    {
        if (SelectedItem is null)
            return;

        Messenger.Send<EditScriptMessage>(new(SelectedItem.LocalFile));
    }

    private void LoadScript()
    {
        if (SelectedItem is null)
            return;

        Messenger.Send<LoadScriptMessage>(new(SelectedItem.LocalFile));
    }

    private async Task Refresh(CancellationToken token)
    {
        IsBusy = true;
        try
        {
            await Task.Run(async () =>
            {
                Progress<string> progress = new(ProgressHandler);
                await _getScriptsService.RefreshAsync(progress, token);
            }, token);
        }
        catch
        {
            RefreshScriptsList();
        }
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

    private async Task UpdateAll()
    {
        IsBusy = true;
        ProgressReportMessage = "Updating scripts...";
        int count = await _getScriptsService.DownloadAllWhereAsync(s => s.Outdated);
        ProgressReportMessage = $"Updated {count} scripts.";
        RefreshScriptsList();
    }

    private async Task DownloadAll()
    {
        IsBusy = true;
        ProgressReportMessage = "Downloading outdated/missing scripts...";
        int count = await Task.Run(async () => await _getScriptsService.DownloadAllWhereAsync(s => !s.Downloaded || s.Outdated));
        ProgressReportMessage = $"Downloaded {count} scripts.";
        RefreshScriptsList();
    }

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
