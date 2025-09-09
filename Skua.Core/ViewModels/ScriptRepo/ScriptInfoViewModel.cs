using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Messaging;
using Skua.Core.Models;
using Skua.Core.Models.GitHub;
using System.Collections.ObjectModel;

namespace Skua.Core.ViewModels;

public partial class ScriptInfoViewModel : ObservableObject
{
    public ScriptInfoViewModel(ScriptInfo info)
    {
        Info = info;
        _downloaded = Info.Downloaded;
    }

    public ScriptInfo Info { get; }
    public string FileName => Info.Name;
    public int Size => Info.Size;
    public string LocalFile => Info.LocalFile;
    public string FilePath => Info.FilePath;
    public string ScriptPath => Path.Combine(ClientFileSources.SkuaScriptsDIR, FilePath.Replace("/", "\\"));

    public ObservableCollection<string> InfoTags
    {
        get => new ObservableCollection<string>(Info.Tags);
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Outdated))]
    private bool _downloaded;

    public bool Outdated => Downloaded && Info.LocalSize != Info.Size;

    [RelayCommand]
    private void LoadScript(ScriptInfoViewModel selectedScript)
    {
        if (selectedScript is null || !selectedScript.Downloaded)
            return;

        StrongReferenceMessenger.Default.Send<LoadScriptMessage, int>(new(selectedScript.LocalFile), (int)MessageChannels.ScriptStatus);
    }

    [RelayCommand]
    private void StartScript(ScriptInfoViewModel selectedScript)
    {
        if (selectedScript is null || !selectedScript.Downloaded)
            return;

        StrongReferenceMessenger.Default.Send<StartScriptMessage, int>(new(selectedScript.LocalFile), (int)MessageChannels.ScriptStatus);
    }

    public override string ToString()
    {
        return FileName;
    }
}