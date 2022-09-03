using Skua.Core.Models.GitHub;
using Skua.Core.Utils;

namespace Skua.Core.Interfaces;
public interface IClientUpdateService
{
    List<UpdateInfo> Releases { get; set; }
    Task GetReleasesAsync();
    Task DownloadUpdateAsync(IProgress<string>? progress, UpdateInfo info);
}
