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

    private async void Markdownview_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        await Task.Run(async () =>
        {
            await Dispatcher.BeginInvoke(new Action(() =>
            {
                while (!((AboutViewModel)DataContext).MarkdownDoc.Contains("About"))
                {
                    Trace.WriteLine("Null");
                    Task.Delay(1500).Wait();
                }
                SubscribeToAllHyperlinks(Markdownview.Document);
            }));
        });
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
