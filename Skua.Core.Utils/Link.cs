using System.Diagnostics;

namespace Skua.Core.Utils;

public class Link
{
    public static void OpenBrowser(string link)
    {
        var ps = new ProcessStartInfo("explorer", link)
        {
            UseShellExecute = true,
            Verb = "open"
        };
        Process.Start(ps);
    }
}