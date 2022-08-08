using System.Windows.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
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
