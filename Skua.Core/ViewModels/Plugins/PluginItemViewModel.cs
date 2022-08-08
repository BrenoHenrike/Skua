using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skua.Core.ViewModels;
public class PluginItemViewModel : ObservableObject
{
    public PluginItemViewModel(IPluginContainer container, bool hasOptions)
    {
        Container = container;
        HasOptions = hasOptions;

        UnloadCommand = new RelayCommand(Unload);
        EditPluginOptionsCommand = new RelayCommand(EditOptions);
    }

    private void EditOptions()
    {
        Container.OptionContainer.Configure();
    }

    private void Unload()
    {
        Ioc.Default.GetRequiredService<IPluginManager>().Unload(Container.Plugin);
    }

    public IPluginContainer Container { get; }
    public bool HasOptions { get; }

    public IRelayCommand UnloadCommand { get; }
    public IRelayCommand EditPluginOptionsCommand { get; }
}
