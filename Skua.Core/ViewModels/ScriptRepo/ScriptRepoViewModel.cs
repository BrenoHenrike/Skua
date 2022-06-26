using Microsoft.Toolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Models.GitHub;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels;
// TODO loading scripts
public class ScriptRepoViewModel : BotControlViewModelBase
{
    public ScriptRepoViewModel(IGetScriptsService getScripts)
        : base("Get Scripts")
    {
        _getScriptsService = getScripts;

        RefreshScriptsCommand = new AsyncRelayCommand(Refresh);
        DownloadAllCommand = new AsyncRelayCommand(DownloadAll);
        UpdateAllCommand = new AsyncRelayCommand(UpdateAll);
        DownloadCommand = new AsyncRelayCommand(Download);
        DeleteCommand = new AsyncRelayCommand(Delete);
        CancelTaskCommand = new RelayCommand(CancelTask);
    }

    private readonly IGetScriptsService _getScriptsService;
    //public RangedObservableCollection<ScriptInfo> Scripts => _getScriptsService.Scripts;
    public int DownloadedQuantity => _scripts.Count(s => s.Downloaded);
    public int OutdatedQuantity => _scripts.Count(s => s.Outdated);
    public int ScriptQuantity => _scripts.Count;
    private RangedObservableCollection<ScriptInfoViewModel> _scripts = new();
    public RangedObservableCollection<ScriptInfoViewModel> Scripts
    {
        get { return _scripts; }
        set
        {
            SetProperty(ref _scripts, value);
            OnPropertyChanged(nameof(DownloadedQuantity));
            OnPropertyChanged(nameof(OutdatedQuantity));
            OnPropertyChanged(nameof(ScriptQuantity));
        }
    }

    private ScriptInfoViewModel? _selectedItem;
    public ScriptInfoViewModel? SeletedItem
    {
        get { return _selectedItem; }
        set
        {
            SetProperty(ref _selectedItem, value);
            OnPropertyChanged(nameof(DownloadedQuantity));
            OnPropertyChanged(nameof(OutdatedQuantity));
            OnPropertyChanged(nameof(ScriptQuantity));
        }
    }

    private bool _showSnackBar;
    public bool ShowSnackBar
    {
        get { return _showSnackBar; }
        set { SetProperty(ref _showSnackBar, value); }
    }
    private bool _isBusy;
    public bool IsBusy
    {
        get { return _isBusy; }
        set { SetProperty(ref _isBusy, value); }
    }

    private string _progressReportMessage = string.Empty;

    public string ProgressReportMessage
    {
        get { return _progressReportMessage; }
        set { SetProperty(ref _progressReportMessage, value); }
    }

    public IAsyncRelayCommand UpdateAllCommand { get; }
    public IAsyncRelayCommand DownloadAllCommand { get; }
    public IAsyncRelayCommand DownloadCommand { get; }
    public IAsyncRelayCommand DeleteCommand { get; }
    public IAsyncRelayCommand RefreshScriptsCommand { get; }
    public IRelayCommand CancelTaskCommand { get; }
    public IRelayCommand LoadScriptCommand { get; }
    // Use message?
    public IRelayCommand OpenScriptCommand { get; }
    public IRelayCommand OpenScriptFolderCommand { get; }

    private async Task Refresh(CancellationToken token)
    {
        IsBusy = true;
        await Task.Run(async () =>
        {
            Progress<string> progress = new(ProgressHandler);
            await _getScriptsService.RefreshAsync(progress, token);
        }, token);
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
