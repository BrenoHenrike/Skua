using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Messaging;
using System.Windows;
using System.Windows.Controls;

namespace Skua.WPF.UserControls;
/// <summary>
/// Interaction logic for BalloonTipUserControl.xaml
/// </summary>
public partial class BalloonTipUserControl : UserControl
{
    public string Title { get; }
    public string Message { get; }

    public BalloonTipUserControl(string title, string message)
    {
        Title = title;
        Message = message;
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        StrongReferenceMessenger.Default.Send<HideBalloonTipMessage>();
    }
}
