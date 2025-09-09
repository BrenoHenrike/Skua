using System.Windows;
using System.Windows.Controls;

namespace Skua.WPF.UserControls;

/// <summary>
/// Interaction logic for ThemeItemUserControl.xaml
/// </summary>
public partial class ThemeItemUserControl : UserControl
{
    public bool CanRemove
    {
        get { return (bool)GetValue(CanRemoveProperty); }
        set { SetValue(CanRemoveProperty, value); }
    }

    public static readonly DependencyProperty CanRemoveProperty =
        DependencyProperty.Register("CanRemove", typeof(bool), typeof(ThemeItemUserControl), new PropertyMetadata(false));

    public ThemeItemUserControl()
    {
        InitializeComponent();
    }
}