using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;

namespace Skua.Core.ViewModels;

public partial class BoostsViewModel : ObservableObject
{
    public BoostsViewModel(IScriptBoost boosts)
    {
        Boosts = boosts;
    }

    public IScriptBoost Boosts { get; }

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

    [RelayCommand]
    private async Task SetBoostIDs(bool searchBank)
    {
        await Task.Run(() => Boosts.SetAllBoostsIDs(searchBank));
    }
}