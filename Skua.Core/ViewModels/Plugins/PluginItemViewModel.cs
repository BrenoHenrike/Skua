using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;

namespace Skua.Core.ViewModels;
public partial class PluginItemViewModel : ObservableObject
{
    public PluginItemViewModel(IPluginContainer container, bool hasOptions)
    {
        Container = container;
        HasOptions = hasOptions;
    }

    public IPluginContainer Container { get; }
    public bool HasOptions { get; }

    [RelayCommand]
    private void EditPluginOptions()
    {
        Container.OptionContainer.Configure();
    }

    [RelayCommand]
    private void Unload()
    {
        Ioc.Default.GetRequiredService<IPluginManager>().Unload(Container.Plugin);
    }
}
