using System.Text;
using System.Xml.Linq;
using System.Dynamic;
using AxShockwaveFlashObjects;
using System.Security;
using Skua.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Flash;
using System.IO;
using System.Windows.Forms;
using Microsoft.Toolkit.Mvvm.Messaging;
using Skua.Core.Messaging;
using System.Threading;

namespace Skua.App.WPF.Flash;

public class FlashUtil : IFlashUtil
{
    public FlashUtil(IMessenger messenger, Lazy<IScriptManager> manager)
    {
        _messenger = messenger;
        _lazyManager = manager;
    }

    private readonly IMessenger _messenger;
    private readonly Lazy<IScriptManager> _lazyManager;
    private AxShockwaveFlash? Flash;

    public event FlashCallHandler? FlashCall;

    public void InitializeFlash()
    {
        if (!EoLHook.IsHooked)
            EoLHook.Hook();

        Flash?.Dispose();

        AxShockwaveFlash flash = new();
        flash.BeginInit();
        flash.Name = "flash";
        flash.Dock = DockStyle.Fill;
        flash.TabIndex = 0;
        flash.FlashCall += CallHandler;
        _messenger.Send<FlashChangedMessage<AxShockwaveFlash>>(new(flash));
        flash.EndInit();
        Flash = flash;
        byte[] swf = File.ReadAllBytes("skua.swf");
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

    private void CallHandler(object sender, _IShockwaveFlashEvents_FlashCallEvent e)
    {
        XElement el = XElement.Parse(e.request);
        string function = el.Attribute("name")!.Value;
        object[] args = el.Elements().Select(x => FromFlashXml(x)).ToArray();
        FlashCall?.Invoke(function, args);
    }

    public string Call(string function, params object[] args)
    {
        return Call<string>(function, args);
    }

    public T Call<T>(string function, params object[] args)
    {
        try
        {
            object o = Call(function, typeof(T), args);
            if (o is not null)
                return (T)o;
            return default;
        }
        catch
        {
            return default;
        }
    }

    public object Call(string function, Type type, params object[] args)
    {
        if (_lazyManager.Value.ShouldExit && Thread.CurrentThread.Name == "Script Thread")
            _lazyManager.Value.ScriptCTS?.Token.ThrowIfCancellationRequested();
        try
        {
            StringBuilder req = new StringBuilder().Append($"<invoke name=\"{function}\" returntype=\"xml\">");
            if (args.Length > 0)
            {
                req.Append("<arguments>");
                args.ForEach(o => req.Append(ToFlashXml(o)));
                req.Append("</arguments>");
            }
            req.Append("</invoke>");
            string result = Flash?.CallFunction(req.ToString())!;
            XElement el = XElement.Parse(result);
            return el is null || el.FirstNode is null ? default : Convert.ChangeType(el.FirstNode.ToString(), type);
        }
        catch (Exception e)
        {
            _messenger.Send<FlashErrorMessage>(new(e, function, args));
            return default;
        }
    }

    public static string ToFlashXml(object o)
    {
        switch (o)
        {
            case null:
                return "<null/>";
            case bool _:
                return $"<{o.ToString()!.ToLower()}/>";
            case double _:
            case float _:
            case long _:
            case int _:
                return $"<number>{o}</number>";
            case ExpandoObject _:
                StringBuilder sb = new StringBuilder().Append("<object>");
                foreach (KeyValuePair<string, object> kvp in (o as IDictionary<string, object>)!)
                    sb.Append($"<property id=\"{kvp.Key}\">{ToFlashXml(kvp.Value)}</property>");
                return sb.Append("</object>").ToString();
            default:
                if (o is Array)
                {
                    StringBuilder _sb = new StringBuilder().Append("<array>");
                    int k = 0;
                    foreach (object el in (o as Array)!)
                        _sb.Append($"<property id=\"{k++}\">{ToFlashXml(el)}</property>");
                    return _sb.Append("</array>").ToString();
                }
                return $"<string>{SecurityElement.Escape(o.ToString())}</string>";
        }
    }

    public object FromFlashXml(XElement el)
    {
        switch (el.Name.ToString())
        {
            case "number":
                return int.TryParse(el.Value, out int i) ? i : float.TryParse(el.Value, out float f) ? f : 0;
            case "true":
                return true;
            case "false":
                return false;
            case "null":
                return null!;
            case "array":
                return el.Elements().Select(e => FromFlashXml(e)).ToArray();
            case "object":
                dynamic d = new ExpandoObject();
                el.Elements().ForEach(e => d[e.Attribute("id")!.Value] = FromFlashXml(e.Elements().First()));
                return d;
            default:
                return el.Value;
        }
    }

    public IFlashObject<T> CreateFlashObject<T>(string path)
    {
        return new FlashObject<T>(Call<int>("lnkCreate", path), this);
    }

    public void Dispose()
    {
        EoLHook.Unhook();
        Flash?.Dispose();
    }
}
