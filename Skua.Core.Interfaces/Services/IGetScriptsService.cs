using System.ComponentModel;
using Skua.Core.Models.GitHub;
using Skua.Core.Utils;

namespace Skua.Core.Interfaces;
public interface IGetScriptsService : INotifyPropertyChanged
{
    RangedObservableCollection<ScriptInfo> Scripts { get; }

    ValueTask<List<ScriptInfo>> GetScriptsAsync(IProgress<string>? progress, CancellationToken token);
    public Task RefreshAsync(IProgress<string>? progress, CancellationToken token);

    public Task DownloadScriptAsync(ScriptInfo info);
    public Task<int> DownloadAllWhereAsync(Func<ScriptInfo, bool> pred);
    public Task DeleteScriptAsync(ScriptInfo info);
}
