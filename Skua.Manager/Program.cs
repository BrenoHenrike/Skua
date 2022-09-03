using Microsoft.Extensions.DependencyInjection;
using Skua.Manager.Properties;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Skua.Manager;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        AppDomain currentDomain = AppDomain.CurrentDomain;
        currentDomain.AssemblyResolve += new ResolveEventHandler(ResolveAssemblies);
        currentDomain.UnhandledException += CurrentDomain_UnhandledException;

        App app = new();
        app.InitializeComponent();
        app.Run();
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Exception ex = (Exception)e.ExceptionObject;
        MessageBox.Show($"Manager 'Crash.\r\nVersion: {Settings.Default.ApplicationVersion}\r\nMessage: {ex.Message}\r\nInner Exception Message: {ex.InnerException?.Message}\r\nStackTrace: {ex.StackTrace}", "Application");
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
