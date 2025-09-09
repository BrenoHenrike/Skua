namespace Skua.Core.Models;

public class HotKey
{
    public HotKey(string key, bool ctrl, bool alt, bool shift)
    {
        Key = key;
        Ctrl = ctrl;
        Alt = alt;
        Shift = shift;
    }

    public string Key { get; set; }
    public bool Ctrl { get; set; }
    public bool Alt { get; set; }
    public bool Shift { get; set; }
}