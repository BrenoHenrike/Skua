using CommunityToolkit.Mvvm.DependencyInjection;
using MdXaml;
using Skua.Core.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Skua.WPF.Views;

/// <summary>
/// Interaction logic for ChangeLogsView.xaml
/// </summary>
public partial class ChangeLogsView : UserControl
{
    private readonly ChangeLogsViewModel _viewModel;

    public ChangeLogsView()
    {
        InitializeComponent();
        _viewModel = Ioc.Default.GetRequiredService<ChangeLogsViewModel>();
        DataContext = _viewModel;
        Markdownview.MarkdownStyle = MarkdownStyle.SasabuneStandard;
        this.IsVisibleChanged += OnVisibilityChanged;
    }

    private void OnVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is bool isVisible && isVisible && _viewModel != null)
        {
            _viewModel.OnActivated();
        }
    }
}