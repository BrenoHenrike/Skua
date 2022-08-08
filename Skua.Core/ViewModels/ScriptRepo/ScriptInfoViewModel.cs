using CommunityToolkit.Mvvm.ComponentModel;
using Skua.Core.Models.GitHub;

namespace Skua.Core.ViewModels;
[ObservableObject]
public partial class ScriptInfoViewModel
{
    public ScriptInfoViewModel(ScriptInfo info)
    {
        Info = info;
        _downloaded = Info.Downloaded;
    }

    public ScriptInfo Info { get; }
    public string FileName => Info.FileName;
    public int Size => Info.Size;
    public string LocalFile => Info.LocalFile;
    public string FilePath => Info.FilePath;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Outdated))]
    private bool _downloaded;

    public bool Outdated => Downloaded && Info.LocalSha != Info.Hash;

    public override string ToString()
    {
        return FileName;
    }
}
