using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models;
using Skua.Core.Utils;
using System.Diagnostics;
using Timer = System.Timers.Timer;

namespace Skua.Core.ViewModels.Manager;

public partial class LauncherViewModel : BotControlViewModelBase, IDisposable
{
    private readonly ISettingsService _settingsService;
    private readonly IDispatcherService _dispatcherService;
    public RangedObservableCollection<ProcessInfo> SkuaProcesses { get; } = new();
    private Timer _timer;
    private bool _disposed;

    public LauncherViewModel(ISettingsService settingsService, IDispatcherService dispatcherService)
        : base("Launcher")
    {
        StrongReferenceMessenger.Default.Register<LauncherViewModel, UpdateStartedMessage>(this, TerminateProcesses);
        StrongReferenceMessenger.Default.Register<LauncherViewModel, AddProcessMessage>(this, AddProcessWithName);
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
        foreach (var procInfo in recipient.SkuaProcesses.ToList())
        {
            await StopProcess(procInfo);
        }

        message.Reply(recipient.SkuaProcesses.Count == 0);
    }

    private void AddProcessWithName(LauncherViewModel recipient, AddProcessMessage message)
    {
        if (message.Process != null)
        {
            var procInfo = new ProcessInfo(message.Process, message.AccountName ?? $"Skua #{message.Process.Id}");
            _dispatcherService.Invoke(() => recipient.SkuaProcesses.Add(procInfo));
        }
    }

    [RelayCommand]
    public async Task LaunchSkua()
    {
        await Task.Run(() =>
        {
            List<string> args = new();
            if (_settingsService.Get("SyncThemes", false))
            {
                args.Add("--use-theme");
                args.Add(_settingsService.Get("CurrentTheme", "no-theme"));
            }

            string token = _settingsService.Get("UserGitHubToken", string.Empty);
            if (!string.IsNullOrEmpty(token))
            {
                args.Add("--gh-token");
                args.Add(token);
            }

            try
            {
                var proc = Process.Start("./Skua.exe", args);
                if (proc != null)
                {
                    var procInfo = new ProcessInfo(proc, $"Skua #{proc.Id}");
                    _dispatcherService.Invoke(() => SkuaProcesses.Add(procInfo));
                }
            }
            catch
            {
                var dialogService = Ioc.Default.GetService<IDialogService>()!;

                dialogService.ShowMessageBox("Failed to launch Skua. Make sure Skua.exe is in the same folder as Skua.Manager.exe.", "Error");
            }
        });
    }

    public void KillAllSkuaProcesses()
    {
        Task.Factory.StartNew(() =>
        {
            foreach (var procInfo in SkuaProcesses)
            {
                procInfo.Process.Kill();
                _dispatcherService.Invoke(() => SkuaProcesses.Remove(procInfo));
            }
        });
    }

    private void RemoveStoppedCurrentProcess(Object source, System.Timers.ElapsedEventArgs e)
    {
        foreach (var procInfo in SkuaProcesses.ToList())
        {
            if (procInfo.HasExited)
            {
                _dispatcherService.Invoke(() => SkuaProcesses.Remove(procInfo));
            }
        }
    }

    [RelayCommand]
    public async Task StopProcess(object? proc)
    {
        ProcessInfo? procInfo = proc as ProcessInfo;
        if (procInfo == null && proc is Process process)
        {
            // For backwards compatibility, find ProcessInfo by Process
            procInfo = SkuaProcesses.FirstOrDefault(p => p.Process == process);
        }

        if (procInfo == null)
            return;

        await Task.Run(() =>
        {
            procInfo.Process.Kill();
            _dispatcherService.Invoke(() => SkuaProcesses.Remove(procInfo));
        });
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Stop and dispose timer
                _timer?.Stop();
                _timer.Elapsed -= RemoveStoppedCurrentProcess!;
                _timer?.Dispose();

                // Unregister from messenger
                StrongReferenceMessenger.Default.Unregister<UpdateStartedMessage>(this);
                StrongReferenceMessenger.Default.Unregister<AddProcessMessage>(this);

                // Clear and kill any remaining processes
                foreach (var procInfo in SkuaProcesses)
                {
                    try
                    {
                        if (!procInfo.HasExited)
                            procInfo.Process.Kill();
                    }
                    catch { }
                }
                SkuaProcesses?.Clear();
            }
            _disposed = true;
        }
    }
}