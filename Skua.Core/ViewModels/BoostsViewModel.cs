using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Skua.Core.Interfaces;

namespace Skua.Core.ViewModels;
public class BoostsViewModel : ObservableObject
{
    public BoostsViewModel(IScriptBoost boosts)
    {
        Boosts = boosts;
        ToggleBoostsCommand = new AsyncRelayCommand(ToggleBoosts);
        SetBoostIDsCommand = new RelayCommand<bool>(Boosts.SetAllBoostsIDs);
    }
    public IScriptBoost Boosts { get; }
    public IAsyncRelayCommand ToggleBoostsCommand { get; }
    public IRelayCommand SetBoostIDsCommand { get; }

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
