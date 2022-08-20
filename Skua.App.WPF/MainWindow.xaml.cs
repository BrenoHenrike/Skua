using AxShockwaveFlashObjects;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.ViewModels;
using Skua.WPF;
using System.Windows;

namespace Skua.App.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : CustomWindow
{
    //public IScriptInterface Bot { get; }

    public MainWindow(/*IScriptInterface bot*/)
    {
        //Bot = bot;
        InitializeComponent();
        DataContext = Ioc.Default.GetService<MainViewModel>();
        //gameContainer.Visibility = Visibility.Hidden;
        //WeakReferenceMessenger.Default.Register<MainWindow, FlashChangedMessage<AxShockwaveFlash>>(this, FlashChanged);
        //Loaded += MainWindow_Loaded;
    }

    //private void FlashChanged(MainWindow recipient, FlashChangedMessage<AxShockwaveFlash> message)
    //{
    //    recipient.gameContainer.Child = message.Flash;
    //}

    //private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    //{
    //    Bot.Flash.FlashCall += LoadingFlash;
    //    Bot.Flash.InitializeFlash();
    //    Loaded -= MainWindow_Loaded;
    //}

    //private void LoadingFlash(string function, params object[] args)
    //{
    //    if(function == "loaded")
    //    {
    //        LoadingBar.Visibility = Visibility.Hidden;
    //        gameContainer.Visibility = Visibility.Visible;
    //        Bot.Flash.FlashCall -= LoadingFlash;
    //    }
    //}
}
