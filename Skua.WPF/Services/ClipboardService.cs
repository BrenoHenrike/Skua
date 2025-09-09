using Skua.Core.Interfaces;
using System.Windows;

namespace Skua.WPF.Services;

public class ClipboardService : IClipboardService
{
    public void SetText(string text)
    {
        Clipboard.SetDataObject(text);
    }

    public void SetData(string format, object data)
    {
        Clipboard.SetData(format, data);
    }

    public object GetData(string format)
    {
        return Clipboard.GetData(format);
    }

    public string GetText()
    {
        return Clipboard.GetText();
    }
}