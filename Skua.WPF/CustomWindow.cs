using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Skua.WPF;
[TemplatePart(Name = "btnClose", Type = typeof(Button))]
[TemplatePart(Name = "btnMaximize", Type = typeof(Button))]
[TemplatePart(Name = "btnMinimize", Type = typeof(Button))]
public partial class CustomWindow : Window
{

    private Button? btnClose;
    private Button? btnMaximize;
    private Button? btnMinimize;

    /// <summary>
    /// Dependency property to set if the custom window should be of Fixed Size.
    /// </summary>
    public bool FixedSize
    { 
        get { return (bool)GetValue(FixedSizeProperty); } 
        set { SetValue(FixedSizeProperty, value); }
    }
    public static readonly DependencyProperty FixedSizeProperty =
        DependencyProperty.Register("FixedSize", typeof(bool), typeof(CustomWindow), new PropertyMetadata(false));

    /// <summary>
    /// Dependency property to set if the custom window should hide instead of closing.
    /// </summary>
    public bool HideWindow
    {
        get { return (bool)GetValue(HideWindowProperty); }
        set { SetValue(HideWindowProperty, value); }
    }

    public static readonly DependencyProperty HideWindowProperty =
        DependencyProperty.Register("HideWindow", typeof(bool), typeof(CustomWindow), new PropertyMetadata(false));



    public CustomWindow() : base()
    {
        SourceInitialized += (s, e) =>
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowChromePatch.WindowProc));
        };

        MenuItem topMost = new();
        topMost.IsCheckable = true;
        topMost.Header = "Top Most";
        topMost.ToolTip = "Makes this window stick at the top of the others.";
        topMost.Click += Topmost_Click;

        ContextMenu = new();
        ContextMenu.Items.Add(topMost);

        Loaded += (s, e) =>
        {
            btnClose = (Button)Template.FindName("btnClose", this);
            btnMaximize = (Button)Template.FindName("btnMaximize", this);
            btnMinimize = (Button)Template.FindName("btnMinimize", this);

            if(!HideWindow)
                btnClose.Click += (s, e) => Close();
            else
            {
                Closing += (s, e) => e.Cancel = true;
                btnClose.Click += (s, e) =>
                {
                    Hide();
                };
            }
            btnMinimize.Click += BtnMinimize_Click;
            if(!FixedSize)
            {
                btnMaximize.Click += BtnMaximize_Click;
                return;
            }

            btnMaximize.IsEnabled = false;
            ResizeMode = ResizeMode.NoResize;
        };
    }

    private void BtnMaximize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void BtnMinimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void Topmost_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem)
            return;
        Topmost = menuItem.IsChecked;
    }
}
