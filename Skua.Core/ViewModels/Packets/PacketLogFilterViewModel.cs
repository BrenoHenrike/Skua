using CommunityToolkit.Mvvm.ComponentModel;

namespace Skua.Core.ViewModels;
public partial class PacketLogFilterViewModel : ObservableObject
{
    public PacketLogFilterViewModel(string content, Predicate<string[]> filter)
    {
        Content = content;
        Filter = filter;
    }

    [ObservableProperty]
    private bool _isChecked = true;

    public string Content { get; }
    public Predicate<string[]> Filter { get; }
}
