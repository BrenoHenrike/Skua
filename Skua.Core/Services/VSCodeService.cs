using System.Diagnostics;
using Skua.Core.Interfaces;
// TODO might change to another combined service
namespace Skua.Core.Services;
public class VSCodeService : IVSCodeService
{
    private readonly string vscPath = Path.Combine(AppContext.BaseDirectory, "VSCode", "code");
    private readonly string scriptsPath = Path.Combine(AppContext.BaseDirectory, "Scripts");
    public void Open()
    {
        Process.Start(vscPath, scriptsPath);
    }

    public void Open(string path)
    {
        Process.Start(vscPath, new[] { scriptsPath, path, "--reuse-window" });
    }
}
