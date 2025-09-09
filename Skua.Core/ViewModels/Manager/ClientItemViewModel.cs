using CommunityToolkit.Mvvm.ComponentModel;

namespace Skua.Core.ViewModels;

public class ClientItemViewModel : ObservableObject
{
    public ClientItemViewModel()
    {
    }

    public string Name { get; set; }

    public string Path { get; set; }

    public override string ToString()
    {
        return Name;
    }
}