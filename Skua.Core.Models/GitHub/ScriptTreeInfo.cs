using Newtonsoft.Json;

namespace Skua.Core.Models.GitHub;

public  class ScriptTree
{
    [JsonProperty("sha")]
    public string Sha { get; set; }
    [JsonProperty("url")]
    public string Url { get; set; }
    [JsonProperty("tree")]
    public List<ScriptTreeInfo> TreeInfo { get; set; }
}

public class ScriptTreeInfo
{
    [JsonProperty("path")]
    public string Path { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; }
}
