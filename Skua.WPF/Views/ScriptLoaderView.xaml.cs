using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.ViewModels;
using System.Windows.Controls;

namespace Skua.WPF.Views;

/// <summary>
/// Interaction logic for ScriptLoaderView.xaml
/// </summary>
public partial class ScriptLoaderView : UserControl
{
    public ScriptLoaderView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<ScriptLoaderViewModel>();
    }
}