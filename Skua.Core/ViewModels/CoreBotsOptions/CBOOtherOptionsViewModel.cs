using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Text;

namespace Skua.Core.ViewModels;
public class CBOOtherOptionsViewModel : ObservableObject, IManageCBOptions
{
    public CBOOtherOptionsViewModel(List<CBOOptionItemContainerViewModel> options)
    {
        Options = options;
        DefaultValues = new();
        foreach (CBOOptionItemContainerViewModel container in Options)
            foreach (DisplayOptionItemViewModelBase option in container.Items)
                DefaultValues.Add(option.Tag, option.Value!);
    }

    public Dictionary<string, object> DefaultValues { get; }
    public List<CBOOptionItemContainerViewModel> Options { get; }

    public StringBuilder Save(StringBuilder builder)
    {
        foreach (CBOOptionItemContainerViewModel container in Options)
        {
            foreach (DisplayOptionItemViewModelBase option in container.Items)
                builder.AppendLine($"{option.Tag}: {option.Value}");
        }

        return builder;
    }

    public void SetValues(Dictionary<string, string> values)
    {
        foreach (CBOOptionItemContainerViewModel container in Options)
        {
            foreach (DisplayOptionItemViewModelBase option in container.Items)
            {
                if (values.TryGetValue(option.Tag, out string? value) && !string.IsNullOrWhiteSpace(value))
                {
                    option.Value = Convert.ChangeType(value, option.DisplayType);
                    continue;
                }
                option.Value = DefaultValues[option.Tag];
            }
        }
    }
}
