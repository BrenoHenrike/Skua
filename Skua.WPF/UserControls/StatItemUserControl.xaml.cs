using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace Skua.WPF.UserControls;
/// <summary>
/// Interaction logic for StatItemUserControl.xaml
/// </summary>
public partial class StatItemUserControl : UserControl
{
    public string Label
    {
        get { return (string)GetValue(LabelProperty); }
        set { SetValue(LabelProperty, value); }
    }

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(StatItemUserControl), new PropertyMetadata(string.Empty));

    public PackIconKind Icon
    {
        get { return (PackIconKind)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(PackIconKind), typeof(StatItemUserControl), new PropertyMetadata(PackIconKind.Abacus));

    public string Value
    {
        get { return (string)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(string), typeof(StatItemUserControl), new PropertyMetadata("0"));

    public StatItemUserControl()
    {
        InitializeComponent();
        Root.DataContext = this;
    }
}
