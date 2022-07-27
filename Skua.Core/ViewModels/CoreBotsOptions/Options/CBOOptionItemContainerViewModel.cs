using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Skua.Core.ViewModels;
public class CBOOptionItemContainerViewModel : ObservableObject
{
    public CBOOptionItemContainerViewModel(string category, List<DisplayOptionItemViewModelBase> items)
    {
        Category = category;
        Items = items;
    }
    public CBOOptionItemContainerViewModel(string category, DisplayOptionItemViewModelBase item)
    {
        Category = category;
        Items = new() { item };
    }

    public string Category { get; }
    public List<DisplayOptionItemViewModelBase> Items { get; }
}
