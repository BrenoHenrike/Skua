using CommunityToolkit.Mvvm.DependencyInjection;
using Grpc.Net.Client;
using Skua.Core.Interfaces;
using Skua.Core.Models.Servers;
using Skua.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Skua.App.WPF.Follower;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : CustomWindow
{
    //private readonly GrpcChannel _channel;
    //private readonly MainAccount.MainAccountClient _client;
    //private readonly GetScripts.GetScriptsClient _client2;
    private IFlashUtil _flash;
    private IScriptServers _server;
    public MainWindow()
    {
        InitializeComponent();
        _flash = Ioc.Default.GetRequiredService<IFlashUtil>();
        _server = Ioc.Default.GetRequiredService<IScriptServers>();
        //_channel = GrpcChannel.ForAddress("http://localhost:5074");
        //_client = new MainAccount.MainAccountClient(_channel);
        //_client2 = new GetScripts.GetScriptsClient(_channel);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Task.Run(() => _server.Relogin("Espada"));
        
        //_server.Logout();
        //await Task.Delay(3000);
        //_server.Login();
        //await Task.Delay(2000);
        //_flash.Call("clickServer", "espada");
        //bool test = await Ioc.Default.GetRequiredService<IScriptWait>().ForTrueAsync(() => Ioc.Default.GetRequiredService<IScriptPlayer>().Playing, 30, 1000);
        //Trace.WriteLine(test);
        //_client2.SetupGHClient(new TokenRequest()
        //{
        //    Token = "suco"
        //});

        //_client.SendCommand(new CommandRequest()
        //{
        //    Command = "suco"
        //});
    }
}
