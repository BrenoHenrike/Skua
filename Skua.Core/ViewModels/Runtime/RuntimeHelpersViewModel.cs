namespace Skua.Core.ViewModels;

public class RuntimeHelpersViewModel : BotControlViewModelBase
{
    public RuntimeHelpersViewModel(ToPickupDropsViewModel toPickupDropsViewModel, RegisteredQuestsViewModel registeredQuestsViewModel, BoostsViewModel boostsViewModel)
        : base("Runtime")
    {
        ToPickupDropsViewModel = toPickupDropsViewModel;
        RegisteredQuestsViewModel = registeredQuestsViewModel;
        BoostsViewModel = boostsViewModel;
    }

    protected override void OnActivated()
    {
        RegisteredQuestsViewModel.IsActive = true;
        ToPickupDropsViewModel.IsActive = true;
    }

    protected override void OnDeactivated()
    {
        RegisteredQuestsViewModel.IsActive = false;
        ToPickupDropsViewModel.IsActive = false;
    }

    public ToPickupDropsViewModel ToPickupDropsViewModel { get; }
    public RegisteredQuestsViewModel RegisteredQuestsViewModel { get; }
    public BoostsViewModel BoostsViewModel { get; }
}