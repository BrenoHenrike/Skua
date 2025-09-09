using CommunityToolkit.Mvvm.ComponentModel;

namespace Skua.Core.ViewModels;

public class TabItemViewModel : ObservableObject
{
    public TabItemViewModel(string header, ObservableObject content)
    {
        Header = header;
        Content = content;
    }

    public string Header { get; }
    public ObservableObject Content { get; }
}