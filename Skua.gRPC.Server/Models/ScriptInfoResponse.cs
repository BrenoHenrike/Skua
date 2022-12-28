using Newtonsoft.Json;

namespace Skua.gRPC.Server.Models;

public class ScriptInfoResponse
{
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
}
