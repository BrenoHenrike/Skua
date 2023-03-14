using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Shell;

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


    public string TitleText
    {
        get { return (string)GetValue(TitleTextProperty); }
        set { SetValue(TitleTextProperty, value); }
    }

    public static readonly DependencyProperty TitleTextProperty =
        DependencyProperty.Register("TitleText", typeof(string), typeof(CustomWindow), new PropertyMetadata("Skua"));

    static CustomWindow()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomWindow), new FrameworkPropertyMetadata(typeof(CustomWindow)));
    }

    public CustomWindow() : base()
    {
        ApplyTemplate();

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

            if (!HideWindow && !TitleText.Contains("Skua Manager"))
            {
                btnClose.Click += (s, e) =>
                {
                    Close();
                    WindowChrome.SetWindowChrome(this, null);

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                };
            }
            else
            {
                Closing += (s, e) =>
                {
                    e.Cancel = true;
                    Hide();
                    if (DataContext is ObservableRecipient recipient)
                        recipient.IsActive = false;
                };
                btnClose.Click += (s, e) =>
                {
                    Hide();
                    if (DataContext is ObservableRecipient recipient)
                        recipient.IsActive = false;
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
