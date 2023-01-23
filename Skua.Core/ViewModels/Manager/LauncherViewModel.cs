using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Utils;
using System.Diagnostics;
using Timer = System.Timers.Timer;

namespace Skua.Core.ViewModels.Manager;
public partial class LauncherViewModel : BotControlViewModelBase
{
    private readonly ISettingsService _settingsService;
    private readonly IDispatcherService _dispatcherService;
    public RangedObservableCollection<Process> SkuaProcesses { get; } = new();
    private Timer _timer;

    public LauncherViewModel(ISettingsService settingsService, IDispatcherService dispatcherService)
        : base("Launcher")
    {
        StrongReferenceMessenger.Default.Register<LauncherViewModel, UpdateStartedMessage>(this, TerminateProcesses);
        _settingsService = settingsService;
        _dispatcherService = dispatcherService;

        _timer = new Timer();
        _timer.Interval = 1000;
        _timer.Elapsed += RemoveStoppedCurrentProcess!;
        _timer.AutoReset = true;
        _timer.Start();
    }

    private async void TerminateProcesses(LauncherViewModel recipient, UpdateStartedMessage message)
    {
        foreach(var proc in recipient.SkuaProcesses.ToList())
        {
            await StopProcess(proc);
        }

        message.Reply(recipient.SkuaProcesses.Count == 0);
    }

    [RelayCommand]
    public async Task LaunchSkua()
    {
        await Task.Run(() =>
        {
            List<string> args = new();
            if(_settingsService.Get("SyncThemes", false))
            {
                args.Add("--use-theme");
                args.Add(_settingsService.Get("CurrentTheme", "no-theme"));
            }

            string token = _settingsService.Get("UserGitHubToken", string.Empty);
            if(!string.IsNullOrEmpty(token))
            {
                args.Add("--gh-token");
                args.Add(token);
            }
            
            var proc = Process.Start("./Skua.exe", args);
            if (proc != null)
                _dispatcherService.Invoke(() => SkuaProcesses.Add(proc));
        });
    }

    public void KillAllSkuaProcesses()
    {
        Task.Factory.StartNew(() =>
        {
            foreach (var proc in SkuaProcesses)
            {
                proc.Kill();
                _dispatcherService.Invoke(() => SkuaProcesses.Remove(proc));
            }
        });
    }

    private void RemoveStoppedCurrentProcess(Object source, System.Timers.ElapsedEventArgs e)
    {
        foreach (var proc in SkuaProcesses)
        {
            if (proc.HasExited)
            {
                _dispatcherService.Invoke(() => SkuaProcesses.Remove(proc));
            }
        }
    }

    [RelayCommand]
    public async Task StopProcess(object? proc)
    {
        if (proc is not Process process)
            return;

        await Task.Run(() =>
        {
            process.Kill();
            _dispatcherService.Invoke(() => SkuaProcesses.Remove(process));
        });
    }
}
