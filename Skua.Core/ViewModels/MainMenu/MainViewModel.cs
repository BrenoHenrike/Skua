
using CommunityToolkit.Mvvm.ComponentModel;

namespace Skua.Core.ViewModels;
public sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = "Skua";

    public MainViewModel() { }
}
