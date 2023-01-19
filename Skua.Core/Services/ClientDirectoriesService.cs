using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skua.Core.Interfaces;

namespace Skua.Core.Services;
public class ClientDirectoriesService : IClientDirectoriesService
{
    public string SkuaDIR { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Skua");
    public string SkuaScriptsDIR { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Skua", "Scripts");
    public string SkuaThemesDIR { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Skua", "themes");
    public string SkuaOptionsDIR { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Skua", "options");
    public string SkuaPluginsDIR { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Skua", "plugins");


    public void CreateDirectories()
    {
        if (!Directory.Exists(SkuaDIR))
        {
            Directory.CreateDirectory(SkuaDIR);

            if (!Directory.Exists(SkuaOptionsDIR))
                Directory.CreateDirectory(SkuaOptionsDIR);
            if (!Directory.Exists(SkuaScriptsDIR))
                Directory.CreateDirectory(SkuaScriptsDIR);
            if (!Directory.Exists(SkuaPluginsDIR))
                Directory.CreateDirectory(SkuaPluginsDIR);
            if (!Directory.Exists(SkuaThemesDIR))
                Directory.CreateDirectory(SkuaThemesDIR);
        }
        else
        {
            if (!Directory.Exists(SkuaOptionsDIR))
                Directory.CreateDirectory(SkuaOptionsDIR);
            if (!Directory.Exists(SkuaScriptsDIR))
                Directory.CreateDirectory(SkuaScriptsDIR);
            if (!Directory.Exists(SkuaPluginsDIR))
                Directory.CreateDirectory(SkuaPluginsDIR);
            if (!Directory.Exists(SkuaThemesDIR))
                Directory.CreateDirectory(SkuaThemesDIR);
        }
    }
}
