using Newtonsoft.Json;

namespace Skua.Core.Models.GitHub;
public class ScriptInfo
{
    public ScriptRepo Parent { get; set; }
    [JsonProperty("name")]
    public string FileName { get; set; }
    [JsonProperty("sha")]
    public string Hash { get; set; }
    [JsonProperty("size")]
    public int Size { get; set; }
    [JsonProperty("download_url")]
    public string DownloadUrl { get; set; }
    [JsonProperty("path")]
    public string FilePath { get; set; }
    public string RelativePath => FilePath == FileName ? "Scripts/" : $"Scripts/{FilePath.Replace(FileName, "")}";
    public string LocalFile => Path.Combine(AppContext.BaseDirectory, "Scripts", FilePath);
    public string LocalShaFile => Path.Combine(AppContext.BaseDirectory, "Scripts", ".shacache", $"{FilePath}.sha");

    public string ManagerLocalFile => Path.Combine(AppContext.BaseDirectory, "Scripts", FilePath);
    public string ManagerLocalShaFile => Path.Combine(AppContext.BaseDirectory, "Scripts", ".shacache", $"{FilePath}.sha");
    
    public string? LocalSha => File.Exists(LocalShaFile) ? File.ReadAllText(LocalShaFile) : null;
    public bool Downloaded => File.Exists(LocalFile);
    public bool Outdated => Downloaded && LocalSha != Hash;

    public override string ToString()
    {
        return FileName;
    }
}
