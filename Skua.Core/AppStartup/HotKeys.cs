using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using System.Runtime.InteropServices;

namespace Skua.Core.AppStartup;

internal class HotKeys
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

    internal static Dictionary<string, IRelayCommand> CreateHotKeys(IServiceProvider s)
    {
        Dictionary<string, IRelayCommand> hotKeys = new()
        {
            { "ToggleScript", new RelayCommand(ToggleScript, CanExecuteHotKey) },
            { "LoadScript", new RelayCommand(LoadScript, CanExecuteHotKey) },
            { "OpenBank", new RelayCommand(Ioc.Default.GetRequiredService<IScriptBank>().Open, CanExecuteHotKey) },
            { "OpenConsole", new RelayCommand(OpenConsole, CanExecuteHotKey) },
            { "ToggleAutoAttack", new RelayCommand(ToggleAutoAttack, CanExecuteHotKey) },
            { "ToggleAutoHunt", new RelayCommand(ToggleAutoHunt, CanExecuteHotKey) }
        };

        return hotKeys;
    }

    private static bool CanExecuteHotKey()
    {
        try
        {
            IntPtr foregroundWindow = GetForegroundWindow();
            if (foregroundWindow == IntPtr.Zero)
                return false;

            GetWindowThreadProcessId(foregroundWindow, out uint foregroundProcessId);
            return foregroundProcessId == (uint)Environment.ProcessId;
        }
        catch
        {
            return false;
        }
    }

    private static void ToggleAutoHunt()
    {
        if (Ioc.Default.GetRequiredService<IScriptAuto>().IsRunning)
        {
            StrongReferenceMessenger.Default.Send<StopAutoMessage>();
            return;
        }

        StrongReferenceMessenger.Default.Send<StartAutoHuntMessage>();
    }

    private static void ToggleAutoAttack()
    {
        if (Ioc.Default.GetRequiredService<IScriptAuto>().IsRunning)
        {
            StrongReferenceMessenger.Default.Send<StopAutoMessage>();
            return;
        }

        StrongReferenceMessenger.Default.Send<StartAutoAttackMessage>();
    }

    private static void OpenConsole()
    {
        Ioc.Default.GetRequiredService<IWindowService>().ShowManagedWindow("Console");
    }

    private static void ToggleScript()
    {
        StrongReferenceMessenger.Default.Send<ToggleScriptMessage, int>((int)MessageChannels.ScriptStatus);
    }

    private static void LoadScript()
    {
        StrongReferenceMessenger.Default.Send<LoadScriptMessage, int>(new(null), (int)MessageChannels.ScriptStatus);
    }
}