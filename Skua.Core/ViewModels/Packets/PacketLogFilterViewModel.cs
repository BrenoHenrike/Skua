using CommunityToolkit.Mvvm.ComponentModel;

namespace Skua.Core.ViewModels;
public class PacketLogFilterViewModel : ObservableRecipient
{
    public PacketLogFilterViewModel(string content, Predicate<string[]> filter)
    {
        Content = content;
        Filter = filter;
    }
    public string Content { get; }
    public Predicate<string[]> Filter { get; }
    private bool _isChecked = true;
    public bool IsChecked
    {
        get { return _isChecked; }
        set { SetProperty(ref _isChecked, value); }
    }

}
