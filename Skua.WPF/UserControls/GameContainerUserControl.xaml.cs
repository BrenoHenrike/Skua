using AxShockwaveFlashObjects;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using System.Windows;
using System.Windows.Controls;

namespace Skua.WPF.UserControls;

/// <summary>
/// Interaction logic for GameContainerUserControl.xaml
/// </summary>
public partial class GameContainerUserControl : UserControl
{
    private IScriptInterface _bot;

    public GameContainerUserControl()
    {
        InitializeComponent();
        _bot = Ioc.Default.GetRequiredService<IScriptInterface>();
        gameContainer.Visibility = Visibility.Hidden;
        WeakReferenceMessenger.Default.Register<GameContainerUserControl, FlashChangedMessage<AxShockwaveFlash>>(this, FlashChanged);
        Loaded += GameContainer_Loaded;
    }

    private void FlashChanged(GameContainerUserControl recipient, FlashChangedMessage<AxShockwaveFlash> message)
    {
        recipient.gameContainer.Child = message.Flash;
    }

    private void GameContainer_Loaded(object sender, RoutedEventArgs e)
    {
        _bot.Flash.FlashCall += LoadingFlash;
        _bot.Flash.InitializeFlash();
        Loaded -= GameContainer_Loaded;
    }

    private void LoadingFlash(string function, params object[] args)
    {
        if (function == "loaded")
        {
            LoadingBar.Visibility = Visibility.Hidden;
            gameContainer.Visibility = Visibility.Visible;
            _bot.Flash.FlashCall -= LoadingFlash;
        }
    }
}