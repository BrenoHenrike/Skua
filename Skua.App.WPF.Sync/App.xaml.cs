using CommunityToolkit.Mvvm.DependencyInjection;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Skua.App.WPF.Follower;
using Skua.Core.AppStartup;
using Skua.Core.Interfaces;
using Skua.gRPC.Server;
using Skua.WPF.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Skua.App.WPF.Sync;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    string username;
    string password;
    int id;

    private Task _syncTask;
    public App()
    {
        AppDomain currentDomain = AppDomain.CurrentDomain;
        currentDomain.AssemblyResolve += new ResolveEventHandler(ResolveAssemblies);
        currentDomain.UnhandledException += CurrentDomain_UnhandledException;

        var args = Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--usr":
                    username = args[++i];
                    break;
                case "--psw":
                    password = args[++i];
                    break;
                case "--id":
                    id = int.Parse(args[++i]);
                    break;
            }
        }

        Services = ConfigureServices();

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            Ioc.Default.GetRequiredService<IScriptServers>().SetLoginInfo(username, password);
            //var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            //var userPrefFile = Path.Combine(path, "Macromedia\\Flash Player\\#SharedObjects\\HFK9B8XK\\game.aq.com\\AQWUserPref.sol");

            //var quality = "LOW";

            //var userLenghtHex = Convert.ToByte((username.Length * 2) + 1).ToString("X2");
            //var passwordLenghtHex = Convert.ToByte((password.Length * 2) + 1).ToString("X2");
            //var qualityLenghtHex = Convert.ToByte((quality.Length * 2) + 1).ToString("X2");
            //var totalLenghtHex = Convert.ToByte(username.Length + password.Length + quality.Length + 132).ToString("X2");

            //var passwordHex = Convert.ToHexString(Encoding.UTF8.GetBytes(password));
            //var usernameHex = Convert.ToHexString(Encoding.UTF8.GetBytes(username));
            //var qualityHex = Convert.ToHexString(Encoding.UTF8.GetBytes(quality));

            //var AQWUserPref = $"00BF000000{totalLenghtHex}5443534F000400000000000B4151575573657250726566000000030F7175616C69747906{qualityLenghtHex}{qualityHex}0025626974436865636B6564557365726E616D6503001162536F756E644F6E030025626974436865636B656450617373776F7264030017737472557365726E616D6506{userLenghtHex}{usernameHex}0011624465617468416403001773747250617373776F726406{passwordLenghtHex}{passwordHex}00";

            //File.WriteAllBytes(userPrefFile, Convert.FromHexString(AQWUserPref));
        }

        Services.GetRequiredService<IScriptInterface>().Flash.FlashCall += Flash_FlashCall;
        _ = Services.GetRequiredService<IScriptSync>();
        //_appCTS = new();
        //_syncTask = StartSync(_appCTS.Token);
    }

    //private async Task StartSync(CancellationToken token)
    //{
    //    var channel = GrpcChannel.ForAddress("http://localhost:5074");
    //    var client = new Synchronizer.SynchronizerClient(channel);

    //    using var call = client.StartReceiving(new SyncRequest()
    //    {
    //        Id = id
    //    }, cancellationToken: token);
    //    var bot = Ioc.Default.GetRequiredService<IScriptInterface>();
    //    while(await call.ResponseStream.MoveNext(token))
    //    {
    //        var current  = call.ResponseStream.Current;
    //        var arguments = current.Arguments.Split(' ');
    //        //if (current.Id != "0" && current.Id != id)
    //        //    continue;
    //        switch(current.Command)
    //        {
    //            case "join":
    //                if (arguments.Length == 1)
    //                    bot.Map.Join(arguments[0]);
    //                else if (arguments.Length == 3)
    //                    bot.Map.Join(arguments[0], arguments[1], arguments[2]);
    //                break;
    //            case "jump":
    //                bot.Map.Jump(arguments[0], arguments[1]);
    //                break;
    //            case "kill":
    //                bot.Kill.Monster(arguments[0]);
    //                break;
    //            case "hunt":
    //                bot.Hunt.Monster(arguments[0]);
    //                break;
    //            case "move":
    //                bot.Player.WalkTo(int.Parse(arguments[0]), int.Parse(arguments[1]));
    //                break;
    //            case "goto":
    //                bot.Player.Goto(arguments[0]);
    //                break;
    //            case "shop":
    //                bot.Shops.Load(int.Parse(arguments[0]));
    //                break;
    //        }
    //    }
    //}

    private void Flash_FlashCall(string function, params object[] args)
    {
        switch (function)
        {
            case "requestLoadGame":
                Services.GetRequiredService<IFlashUtil>().Call("loadClient");
                break;
            case "loaded":
                Services.GetRequiredService<IFlashUtil>().FlashCall -= Flash_FlashCall;
                if(!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                    Task.Run(async () =>
                    {
                        await Task.Delay(2500);
                        Ioc.Default.GetRequiredService<IScriptServers>().Relogin("twilly");
                    });
                break;
        }
    }

    public new static App Current = (App)Application.Current;

    public IServiceProvider Services { get; }

    private IServiceProvider ConfigureServices()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddSingleton<ISettingsService, SettingsService>();

        services.AddWindowsServices();

        services.AddCommonServices();

        services.AddScriptableObjects();

        //services.AddSync(1);

        ServiceProvider provider = services.BuildServiceProvider();
        Ioc.Default.ConfigureServices(provider);

        return provider;
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Exception ex = (Exception)e.ExceptionObject;
        MessageBox.Show($"Application Crash.\r\nVersion: 0.0.0.0\r\nMessage: {ex.Message}\r\nStackTrace: {ex.StackTrace}", "Application");
    }

    static Assembly? ResolveAssemblies(object? sender, ResolveEventArgs args)
    {
        if (args.Name.Contains(".resources"))
            return null;

        Assembly? assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
        if (assembly != null)
            return assembly;

        string assemblyName = new AssemblyName(args.Name).Name + ".dll";
        string assemblyPath = Path.Combine(AppContext.BaseDirectory, "Assemblies", assemblyName);
        if (!File.Exists(assemblyPath))
        {
            assemblyPath = Path.Combine(AppContext.BaseDirectory, assemblyName);
            return File.Exists(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null;
        }
        return Assembly.LoadFrom(assemblyPath);
    }
}
