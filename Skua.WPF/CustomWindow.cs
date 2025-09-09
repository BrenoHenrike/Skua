using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Shell;

namespace Skua.WPF;

[TemplatePart(Name = "PART_Close", Type = typeof(Button))]
[TemplatePart(Name = "PART_Maximize", Type = typeof(Button))]
[TemplatePart(Name = "PART_Minimize", Type = typeof(Button))]
public partial class CustomWindow : Window
{
    private Button? _btnClose;
    private Button? _btnMaximize;
    private Button? _btnMinimize;
    private readonly MenuItem _topMostMenu;

    private HwndSource _handle;
    private HwndSourceHook _hook;

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
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(CustomWindow),
            new FrameworkPropertyMetadata(typeof(CustomWindow)));
    }

    public CustomWindow() : base()
    {
        ApplyTemplate();

        SourceInitialized += CustomWindow_SourceInitialized;

        _topMostMenu = new()
        {
            IsCheckable = true,
            Header = "Top Most",
            ToolTip = "Makes this window stick at the top of the others."
        };
        _topMostMenu.Click += Topmost_Click;

        ContextMenu = new();
        ContextMenu.Items.Add(_topMostMenu);

        Loaded += CustomWindow_Loaded;
    }

    private void CustomWindow_SourceInitialized(object? sender, EventArgs e)
    {
        _handle = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
        _hook = new HwndSourceHook(WindowChromePatch.WindowProc);
        _handle.AddHook(_hook);
        SourceInitialized -= CustomWindow_SourceInitialized;
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _btnClose = GetTemplateChild("PART_Close") as Button;
        _btnMaximize = GetTemplateChild("PART_Maximize") as Button;
        _btnMinimize = GetTemplateChild("PART_Minimize") as Button;
    }

    private void CustomWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (!HideWindow && !TitleText.Contains("Skua Manager"))
        {
            _btnClose!.Click += WindowClose_Click;
        }
        else
        {
            Closing += HideWindow_Closing;
            _btnClose!.Click += HideWindow_Close;
        }

        _btnMinimize!.Click += WindowMinimize_Click;

        if (!FixedSize)
        {
            _btnMaximize!.Click += WindowMaximize_Click;
        }
        else
        {
            _btnMaximize!.IsEnabled = false;
            ResizeMode = ResizeMode.NoResize;
        }

        Loaded -= CustomWindow_Loaded;
    }

    private void HideWindow_Close(object sender, RoutedEventArgs e)
    {
        Hide();
        if (DataContext is ObservableRecipient recipient)
            recipient.IsActive = false;
    }

    private void HideWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
        if (DataContext is ObservableRecipient recipient)
            recipient.IsActive = false;
    }

    private void WindowMaximize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void WindowMinimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void Topmost_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem)
            return;
        Topmost = menuItem.IsChecked;
    }

    private void WindowClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
        if (_btnMaximize is not null && _btnMinimize is not null && _btnClose is not null)
        {
            _btnMaximize.Click -= WindowClose_Click;
            _btnMaximize.Click -= WindowMaximize_Click;
            _btnMinimize.Click -= WindowMinimize_Click;
        }
        _topMostMenu.Click -= Topmost_Click;
        _handle.RemoveHook(_hook);
        _handle.Dispose();
        WindowChrome.SetWindowChrome(this, null);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
}