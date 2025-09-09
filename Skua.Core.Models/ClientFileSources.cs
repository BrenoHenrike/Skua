namespace Skua.Core.Models;

public static class ClientFileSources
{
    public static string SkuaDIR { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Skua");
    public static string SkuaScriptsDIR { get; } = Path.Combine(SkuaDIR, "Scripts");
    public static string SkuaThemesDIR { get; } = Path.Combine(SkuaDIR, "themes");
    public static string SkuaOptionsDIR { get; } = Path.Combine(SkuaDIR, "options");
    public static string SkuaPluginsDIR { get; } = Path.Combine(SkuaDIR, "plugins");
    public static string SkuaAdvancedSkillsFile { get; } = Path.Combine(SkuaDIR, "AdvancedSkills.txt");
    public static string SkuaQuestsFile { get; } = Path.Combine(SkuaDIR, "Quests.txt");
}