using AxShockwaveFlashObjects;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Skua.App.Flash;
using Skua.Core.Interfaces;
using Skua.Core.ViewModels;
using Skua.WPF;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Skua.App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : CustomWindow
{
    public IScriptInterface Bot { get; }
    public IFlashUtil FlashUtil { get; }

    public MainWindow(IScriptInterface bot, IFlashUtil flashUtil)
    {
        Bot = bot;
        FlashUtil = flashUtil;
        DataContext = Ioc.Default.GetService(typeof(MainViewModel));
        InitializeComponent();
        InitFlash();

        Bot.Schedule(2000, b => Bot.Initialize());
    }

    private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        InitFlash();
    }

    public void InitFlash()
    {
        if (!EoLHook.IsHooked)
            EoLHook.Hook();

        Flash.FlashUtil.Flash?.Dispose();

        AxShockwaveFlash flash = new();
        flash.BeginInit();
        flash.Name = "flash";
        flash.Dock = DockStyle.Fill;
        flash.TabIndex = 0;
        flash.FlashCall += CallHandler;
        gameContainer.Controls.Add(flash);
        flash.EndInit();
        Flash.FlashUtil.Flash = flash;
        // TODO swf from setting manager
        byte[] swf = File.ReadAllBytes("rbot.swf");
        using (MemoryStream stream = new())
        using (BinaryWriter writer = new(stream))
        {
            writer.Write(8 + swf.Length);
            writer.Write(1432769894);
            writer.Write(swf.Length);
            writer.Write(swf);
            writer.Seek(0, SeekOrigin.Begin);
            flash.OcxState = new AxHost.State(stream, 1, false, null);
        }

        EoLHook.Unhook();
    }

    public void CallHandler(object sender, _IShockwaveFlashEvents_FlashCallEvent e)
    {
        XElement el = XElement.Parse(e.request);
        string name = el.Attribute("name")!.Value;
        object[] args = el.Elements().Select(x => FlashUtil.FromFlashXml(x)).ToArray();
        FlashUtil.OnFlashCall(name, args);
    }
}
