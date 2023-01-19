namespace Skua.Core.Interfaces;
public interface IClientDirectoriesService
{
    string SkuaDIR { get; }
    string SkuaOptionsDIR { get; }
    string SkuaScriptsDIR { get; }
    string SkuaPluginsDIR { get; }
    string SkuaThemesDIR { get; }

    void CreateDirectories();
}
