namespace Skua.Core.ViewModels;

public class ApplicationOptionsViewModel : BotControlViewModelBase
{
    public ApplicationOptionsViewModel(List<DisplayOptionItemViewModelBase> appOptions)
        : base("Application Options", 420, 0)
    {
        ApplicationOptions = appOptions;
    }

    public List<DisplayOptionItemViewModelBase> ApplicationOptions { get; }
}