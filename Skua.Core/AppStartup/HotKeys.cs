using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Skua.Core.AppStartup;
internal class HotKeys
{
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
        return Ioc.Default.GetRequiredService<IFlashUtil>().GetGameObject("stage.focus") != "null";
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
