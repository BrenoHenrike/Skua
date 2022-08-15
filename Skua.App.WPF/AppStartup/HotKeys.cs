using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Skua.App.WPF.AppStartup;
internal class HotKeys
{
    internal static Dictionary<string, ICommand> CreateHotKeys(IServiceProvider s)
    {
        Dictionary<string, ICommand> hotKeys = new()
        {
            { "ToggleScript", new RelayCommand(ToggleScript) },
            { "LoadScript", new RelayCommand(LoadScript) },
            { "OpenBank", new RelayCommand(Ioc.Default.GetRequiredService<IScriptBank>().Open) },
            { "OpenConsole", new RelayCommand(OpenConsole) },
            { "ToggleAutoAttack", new RelayCommand(ToggleAutoAttack) },
            { "ToggleAutoHunt", new RelayCommand(ToggleAutoHunt) }
        };

        return hotKeys;
    }

    private static void ToggleAutoHunt()
    {
        if(Ioc.Default.GetRequiredService<IScriptAuto>().IsRunning)
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
