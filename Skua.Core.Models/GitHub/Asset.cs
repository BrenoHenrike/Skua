using Newtonsoft.Json;

namespace Skua.Core.Models.GitHub;
public class Asset
{
    [JsonProperty("browser_download_url")]
    public string? BrowserUrl { get; set; }
}
