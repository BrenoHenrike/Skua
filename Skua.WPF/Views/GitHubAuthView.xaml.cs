using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace Skua.WPF.Views;
/// <summary>
/// Interaction logic for GitHubAuthView.xaml
/// </summary>
public partial class GitHubAuthView : UserControl
{
    public GitHubAuthView()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var window = Window.GetWindow(this);
        window.DialogResult = false;
        window.Close();
    }
}
