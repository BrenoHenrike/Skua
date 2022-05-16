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
    public bool FixedSize { get { return (bool)GetValue(FixedSizeProperty); } set { SetValue(FixedSizeProperty, value); } }
    public static readonly DependencyProperty FixedSizeProperty = DependencyProperty.Register("FixedSize", typeof(bool), typeof(CustomWindow), new PropertyMetadata(false));
    
    public CustomWindow() : base()
    {
        SourceInitialized += (s, e) =>
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowChromePatch.WindowProc));
        };
        
        Loaded += (s, e) =>
        {
            btnClose = (Button)Template.FindName("btnClose", this);
            btnMaximize = (Button)Template.FindName("btnMaximize", this);
            btnMinimize = (Button)Template.FindName("btnMinimize", this);

            btnClose.Click += (s, e) => Close();
            btnMinimize.Click += (s, e) => WindowState = WindowState.Minimized;
            if(!FixedSize)
            {
                btnMaximize.Click += (s, e) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                return;
            }

            btnMaximize.IsEnabled = false;
            ResizeMode = ResizeMode.NoResize;
        };
    }
}
