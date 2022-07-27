using System.Diagnostics;
using Skua.Core.Interfaces;

namespace Skua.App.WPF.Services;
public class BrowserService : IBrowserService
{
    public void Open(string link)
    {
        var ps = new ProcessStartInfo("explorer", link)
        {
            UseShellExecute = true,
            Verb = "open"
        };
        Process.Start(ps);
    }
}
