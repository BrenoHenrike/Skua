namespace Skua.Core.ViewModels;
public partial class RuntimeHelpersViewModel : BotControlViewModelBase
{
    public RuntimeHelpersViewModel(ToPickupDropsViewModel toPickupDropsViewModel, RegisteredQuestsViewModel registeredQuestsViewModel, BoostsViewModel boostsViewModel)
        : base("Runtime")
    {
        ToPickupDropsViewModel = toPickupDropsViewModel;
        RegisteredQuestsViewModel = registeredQuestsViewModel;
        BoostsViewModel = boostsViewModel;
    }

    public ToPickupDropsViewModel ToPickupDropsViewModel { get; }
    public RegisteredQuestsViewModel RegisteredQuestsViewModel { get; }
    public BoostsViewModel BoostsViewModel { get; }
}
