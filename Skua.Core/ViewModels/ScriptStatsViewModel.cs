using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;

namespace Skua.Core.ViewModels;
public class ScriptStatsViewModel : ObservableObject, IManagedWindow
{
    public ScriptStatsViewModel(IScriptBotStats scriptStats)
    {
        ScriptStats = scriptStats;
        ResetStatsCommand = new RelayCommand(ScriptStats.Reset);
    }

    public IScriptBotStats ScriptStats { get; }
    public IRelayCommand ResetStatsCommand { get; }
    public string Title => "Stats";
    public int Width => 250;
    public int Height => 235;
    public bool CanResize => false;
}
