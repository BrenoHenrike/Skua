namespace Skua.Core.Interfaces;
public interface IClipboardService
{
    public void SetText(string text);
    public void SetData(string format, object data);
    public object GetData(string format);
    public string GetText();
}
