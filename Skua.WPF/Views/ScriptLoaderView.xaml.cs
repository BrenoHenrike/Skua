using System.Windows.Controls;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Skua.Core.ViewModels;

namespace Skua.WPF.Views;
/// <summary>
/// Interaction logic for ScriptLoaderView.xaml
/// </summary>
public partial class ScriptLoaderView : UserControl
{
    public ScriptLoaderView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<ScriptLoaderViewModel>()!;
    }
}
