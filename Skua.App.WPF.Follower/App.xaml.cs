using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Skua.Core.AppStartup;
using Skua.Core.Interfaces;
using Skua.WPF.Services;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace Skua.App.WPF.Follower;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private string username;
    private string password;

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
            }
        }

        Services = ConfigureServices();

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            Ioc.Default.GetRequiredService<IScriptServers>().SetLoginInfo(username, password);
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var userPrefFile = Path.Combine(path, "Macromedia\\Flash Player\\#SharedObjects\\HFK9B8XK\\game.aq.com\\AQWUserPref.sol");

            var quality = "MEDIUM";

            var userLenghtHex = Convert.ToByte((username.Length * 2) + 1).ToString("X2");
            var passwordLenghtHex = Convert.ToByte((password.Length * 2) + 1).ToString("X2");
            var qualityLenghtHex = Convert.ToByte((quality.Length * 2) + 1).ToString("X2");
            var totalLenghtHex = Convert.ToByte(username.Length + password.Length + quality.Length + 132).ToString("X2");

            var passwordHex = Convert.ToHexString(Encoding.UTF8.GetBytes(password));
            var usernameHex = Convert.ToHexString(Encoding.UTF8.GetBytes(username));
            var qualityHex = Convert.ToHexString(Encoding.UTF8.GetBytes(quality));

            var AQWUserPref = $"00BF000000{totalLenghtHex}5443534F000400000000000B4151575573657250726566000000030F7175616C69747906{qualityLenghtHex}{qualityHex}0025626974436865636B6564557365726E616D6503001162536F756E644F6E030025626974436865636B656450617373776F7264030017737472557365726E616D6506{userLenghtHex}{usernameHex}0011624465617468416403001773747250617373776F726406{passwordLenghtHex}{passwordHex}00";

            File.WriteAllBytes(userPrefFile, Convert.FromHexString(AQWUserPref));
        }

        Services.GetRequiredService<IScriptInterface>().Flash.FlashCall += Flash_FlashCall;
    }

    private void Flash_FlashCall(string function, params object[] args)
    {
        switch (function)
        {
            case "requestLoadGame":
                Services.GetRequiredService<IFlashUtil>().Call("loadClient");
                break;

            case "loaded":
                Services.GetRequiredService<IFlashUtil>().FlashCall -= Flash_FlashCall;
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

        ServiceProvider provider = services.BuildServiceProvider();
        Ioc.Default.ConfigureServices(provider);

        return provider;
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Exception ex = (Exception)e.ExceptionObject;
        MessageBox.Show($"Application Crash.\r\nVersion: 0.0.0.0\r\nMessage: {ex.Message}\r\nStackTrace: {ex.StackTrace}", "Application");
    }

    private static Assembly? ResolveAssemblies(object? sender, ResolveEventArgs args)
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