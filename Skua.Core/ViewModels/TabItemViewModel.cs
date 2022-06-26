using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Skua.Core.ViewModels;
public class TabItemViewModel : ObservableObject
{
    public string Header { get; }
    public ObservableObject Content { get; }
    public TabItemViewModel(string header, ObservableObject content)
    {
        Header = header;
        Content = content;
    }
}
