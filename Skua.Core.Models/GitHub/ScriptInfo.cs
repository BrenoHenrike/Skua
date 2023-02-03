using Newtonsoft.Json;

namespace Skua.Core.Models.GitHub;
public class ScriptInfo
{
    
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("tags")]
    public string[] Tags { get; set; }

    [JsonProperty("path")]
    public string FilePath { get; set; }

    [JsonProperty("size")]
    public int Size { get; set; }

    [JsonProperty("fileName")]
    public string FileName { get; set; }

    [JsonProperty("downloadUrl")]
    public string DownloadUrl { get; set; }
    
    
    public string RelativePath => FilePath == FileName ? "Scripts/" : $"Scripts/{FilePath.Replace(FileName, "")}";
    
    public string LocalFile => Path.Combine(ClientFileSources.SkuaScriptsDIR, FilePath);

    public string ManagerLocalFile => Path.Combine(ClientFileSources.SkuaScriptsDIR, FilePath);

    public bool Downloaded => File.Exists(LocalFile);
    
    public int LocalSize => Downloaded ? (int)new FileInfo(LocalFile).Length : 0;
    public bool Outdated => Downloaded && LocalSize != Size;

    public override string ToString()
    {
        return FileName;
    }
}
