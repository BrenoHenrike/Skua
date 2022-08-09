using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;

namespace Skua.Core.ViewModels;
public partial class BoostsViewModel : ObservableObject
{
    public BoostsViewModel(IScriptBoost boosts)
    {
        Boosts = boosts;
        SetBoostIDsCommand = new AsyncRelayCommand<bool>(async b => await Task.Run(() => Boosts.SetAllBoostsIDs(b)));
    }
    public IScriptBoost Boosts { get; }
    public IAsyncRelayCommand SetBoostIDsCommand { get; }

    [RelayCommand]
    private async Task ToggleBoosts()
    {
        if (!Boosts.Enabled)
        {
            Boosts.Start();
            return;
        }
        await Boosts.StopAsync();
    }
}
