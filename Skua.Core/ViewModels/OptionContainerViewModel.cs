using CommunityToolkit.Mvvm.ComponentModel;
using Skua.Core.Interfaces;

namespace Skua.Core.ViewModels;

public partial class OptionContainerViewModel : ObservableObject
{
    public OptionContainerViewModel(IOptionContainer container)
    {
        Container = container;
        Options = new();
        foreach (IOption option in container.Options)
            Options.Add(new(container, option));

        if (container.MultipleOptions.Count > 0)
        {
            foreach (IOption option in container.MultipleOptions.Values.SelectMany(x => x))
                Options.Add(new(container, option));
        }
    }

    public string Title { get; } = "Options";
    public IOptionContainer Container { get; set; }

    public List<OptionContainerItemViewModel> Options { get; }

    [ObservableProperty]
    private OptionContainerItemViewModel? _selectedOption;
}