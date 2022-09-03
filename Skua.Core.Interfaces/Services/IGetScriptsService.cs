using System.ComponentModel;
using Skua.Core.Models.GitHub;
using Skua.Core.Utils;

namespace Skua.Core.Interfaces;
public interface IGetScriptsService : INotifyPropertyChanged
{
    int Downloaded => Scripts.Count(s => s.Downloaded);
    int Outdated => Scripts.Count(s => s.Outdated);
    int Total => Scripts.Count;
    int Missing => Total - Downloaded;
    RangedObservableCollection<ScriptInfo> Scripts { get; }

    ValueTask<List<ScriptInfo>> GetScriptsAsync(IProgress<string>? progress, CancellationToken token);
    public Task RefreshScriptsAsync(IProgress<string>? progress, CancellationToken token);

    public Task DownloadScriptAsync(ScriptInfo info);
    public Task<int> DownloadAllWhereAsync(Func<ScriptInfo, bool> pred);
    public Task DeleteScriptAsync(ScriptInfo info);
}
