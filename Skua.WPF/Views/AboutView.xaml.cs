using CommunityToolkit.Mvvm.DependencyInjection;
using MdXaml;
using Skua.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;

namespace Skua.WPF.Views;
/// <summary>
/// Interaction logic for AboutView.xaml
/// </summary>
public partial class AboutView : UserControl
{
    public AboutView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<AboutViewModel>();
        Markdownview.MarkdownStyle = MarkdownStyle.SasabuneStandard;
    }

    private void Markdownview_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        Dispatcher.BeginInvoke(new Action(() =>
        {
            SubscribeToAllHyperlinks(Markdownview.Document);
        }), DispatcherPriority.Loaded);
    }

    void SubscribeToAllHyperlinks(FlowDocument flowDocument)
    {
        var hyperlinks = GetVisuals(flowDocument).OfType<Hyperlink>();
        foreach (var link in hyperlinks)
            link.Command = ((AboutViewModel)DataContext).NavigateCommand;
    }

    IEnumerable<DependencyObject> GetVisuals(DependencyObject root)
    {
        foreach (var child in LogicalTreeHelper.GetChildren(root).OfType<DependencyObject>())
        {
            yield return child;
            foreach (var descendants in GetVisuals(child))
                yield return descendants;
        }
    }

    void link_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
        e.Handled = true;
    }
}
