using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Models.GitHub;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Utils;
using Skua.Core.Models;

namespace Skua.Core.ViewModels.Manager;
public partial class ClientUpdatesViewModel : BotControlViewModelBase
{
    public ClientUpdatesViewModel(IClientUpdateService updateService, ISettingsService settingsService, IGetScriptsService scriptsService, IDialogService dialogService)
        : base("Client Updates")
    {
        StrongReferenceMessenger.Default.Register<ClientUpdatesViewModel, DownloadClientUpdateMessage>(this, DownloadUpdate);
        StrongReferenceMessenger.Default.Register<ClientUpdatesViewModel, UpdateScriptsMessage>(this, ReceiveUpdateScriptsMessage);
        StrongReferenceMessenger.Default.Register<ClientUpdatesViewModel, CheckClientUpdateMessage>(this, CheckUpdate);

        _updateService = updateService;
        _settingsService = settingsService;
        _scriptsService = scriptsService;
        _dialogService = dialogService;
        Current = _settingsService.Get("ApplicationVersion", "0.0.0.0");
        _appVersion = Version.Parse(Current);
        _progress = new Progress<string>(p => ProgressStatus = p);
    }

    private async void CheckUpdate(ClientUpdatesViewModel recipient, CheckClientUpdateMessage message)
    {
        await recipient.Refresh();

        if (recipient.UpdateVisible && recipient._dialogService.ShowMessageBox($"New update available: {recipient.Latest?.Name}\r\nDo you want to download it?", "Update Available", true) == true)
            await recipient.Update();
    }

    private async void ReceiveUpdateScriptsMessage(ClientUpdatesViewModel recipient, UpdateScriptsMessage message)
    {
        if(message.Reset)
        {
            await recipient.ResetScripts(default);
            return;
        }

        await recipient.UpdateScripts(default);
    }

    protected override void OnActivated()
    {
        if (Releases.Count == 0)
            Refresh();
    }

    protected override void OnDeactivated()
    {
        StrongReferenceMessenger.Default.UnregisterAll(this);
        base.OnDeactivated();
    }

    private readonly ISettingsService _settingsService;
    private readonly IGetScriptsService _scriptsService;
    private readonly IDialogService _dialogService;
    private readonly IClientUpdateService _updateService;
    private readonly Version _appVersion;
    private readonly IProgress<string> _progress;

    public RangedObservableCollection<ClientUpdateItemViewModel> Releases { get; } = new();

    public string Current { get; }

    [ObservableProperty]
    private string _status = "Loading...";
    [ObservableProperty]
    private string? _progressStatus = null;
    [ObservableProperty]
    private bool _isBusy;
    [ObservableProperty]
    private bool _updateVisible;
    [ObservableProperty]
    private UpdateInfo? _latest;

    [RelayCommand]
    public async Task Refresh()
    {
        IsBusy = true;
        Status = "Loading...";
        try
        {
            await _updateService.GetReleasesAsync();
            
            bool checkPrereleases = _settingsService.Get("CheckClientPrereleases", false);
            UpdateInfo? latest = _updateService.Releases.FirstOrDefault(r => checkPrereleases || !r.Prerelease);
            UpdateVisible = latest?.ParsedVersion.CompareTo(_appVersion) > 0;
            
            if (UpdateVisible)
                Latest = latest;

            Releases.Clear();
            foreach(var release in _updateService.Releases)
            {
                if (checkPrereleases || !release.Prerelease)
                    Releases.Add(new(release));
            }
                
            Status = UpdateVisible ? "Update available" : "You have the latest version";
        }
        catch
        {
            Status = "Error while getting releases";
        }
        finally
        {
            IsBusy = false;
        }

    }

    [RelayCommand]
    public async Task Update()
    {
        if(Latest is null)
            return;

        IsBusy = true;
        await _updateService.DownloadUpdateAsync(_progress, Latest);
        await Task.Delay(1000);
        ProgressStatus = null;
        IsBusy = false;
    }

    private async void DownloadUpdate(ClientUpdatesViewModel recipient, DownloadClientUpdateMessage message)
    {
        recipient.IsBusy = true;
        await recipient._updateService.DownloadUpdateAsync(recipient._progress, message.UpdateInfo);
        await Task.Delay(1000);
        recipient.ProgressStatus = null;
        recipient.IsBusy = false;
    }


    [RelayCommand]
    public async Task ResetScripts(CancellationToken token)
    {
        IsBusy = true;
        var skuaPath = ClientFileSources.SkuaScriptsDIR;
        if (Directory.Exists(skuaPath))
            Directory.Delete(skuaPath, true);

        if (!Directory.Exists(skuaPath))
            Directory.CreateDirectory(skuaPath);

        await UpdateScripts(token);
    }

    [RelayCommand]
    public async Task UpdateScripts(CancellationToken token)
    {
        IsBusy = true;
        try
        {
            await _scriptsService.RefreshScriptsAsync(_progress, token);

            int count = await Task.Run(async () => await _scriptsService.ManagerDownloadAllWhereAsync(s => !s.Downloaded || s.Outdated));
            ProgressStatus = $"Downloaded {count} scripts.";

        }
        catch (OperationCanceledException)
        {
            ProgressStatus = "Task cancelled.";
        }
        catch { }
        finally
        {
            await Task.Delay(1000);
            IsBusy = false;
            ProgressStatus = null;
        }
    }
}
