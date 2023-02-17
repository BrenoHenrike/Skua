using System.Windows.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using MdXaml;
using Skua.Core.ViewModels;

namespace Skua.WPF.Views;
/// <summary>
/// Interaction logic for ChangeLogsView.xaml
/// </summary>
public partial class ChangeLogsView : UserControl
{
    public ChangeLogsView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<ChangeLogsViewModel>();
        Markdownview.MarkdownStyle = MarkdownStyle.SasabuneStandard;
    }
}
