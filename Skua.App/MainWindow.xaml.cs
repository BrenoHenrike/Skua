using AxShockwaveFlashObjects;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Skua.App.Flash;
using Skua.Core.Interfaces;
using Skua.Core.ViewModels;
using Skua.WPF;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Skua.App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : CustomWindow
{
    public IScriptInterface Bot { get; }

    public MainWindow(IScriptInterface bot)
    {
        Bot = bot;
        InitializeComponent();
        DataContext = Ioc.Default.GetService<MainViewModel>();
        gameContainer.Visibility = Visibility.Hidden;
        Bot.Flash.FlashChanged += Flash_FlashChanged;
        Loaded += MainWindow_Loaded;
    }

    private void Flash_FlashChanged(System.ComponentModel.IComponent flash)
    {
        gameContainer.Child = (AxShockwaveFlash)flash;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        Bot.Flash.FlashCall += LoadingFlash;
        Bot.Flash.InitializeFlash();
        Loaded -= MainWindow_Loaded;
    }

    private void LoadingFlash(string function, params object[] args)
    {
        if(function == "loaded")
        {
            LoadingBar.Visibility = Visibility.Hidden;
            gameContainer.Visibility = Visibility.Visible;
            Bot.Flash.FlashCall -= LoadingFlash;
        }
    }

    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
        Bot.Flash.InitializeFlash();
    }

    private void MenuItem_Click_1(object sender, RoutedEventArgs e)
    {
        Title = "Suco";
    }
}
