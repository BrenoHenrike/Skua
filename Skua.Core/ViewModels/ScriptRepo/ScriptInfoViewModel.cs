using Microsoft.Toolkit.Mvvm.ComponentModel;
using Skua.Core.Models.GitHub;

namespace Skua.Core.ViewModels;
[INotifyPropertyChanged]
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
    private bool _downloaded;
    public bool Downloaded
    {
        get { return _downloaded; }
        set 
        {
            SetProperty(ref _downloaded, value);
            OnPropertyChanged(nameof(Outdated));
        }
    }

    public bool Outdated => Downloaded && Info.LocalSha != Info.Hash;

    public override string ToString()
    {
        return FileName;
    }
}
