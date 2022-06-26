using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using Skua.Core.Interfaces;

namespace Skua.Core.ViewModels;
public class ScriptLoaderViewModel : BotControlViewModelBase
{
    private readonly string _scriptPath;
    public ScriptLoaderViewModel(IScriptManager scriptManager, IFileDialogService fileDialog, IVSCodeService vscService, IEnumerable<LogTabItemViewModel> logs) 
        : base("Load Script")
    {
        _scriptPath = Path.Combine(AppContext.BaseDirectory, "Scripts");
        ScriptLogs = logs.ToArray()[1];
        _scriptManager = scriptManager;
        _fileDialog = fileDialog;
        _vscService = vscService;
        LoadScriptCommand = new RelayCommand(LoadScript);
        OpenVSCodeCommand = new RelayCommand(_vscService.Open);
        EditScriptCommand = new RelayCommand(EditScript);
    }

    private string _scriptStatus = "[No Script Loaded]";
    public string ScriptStatus
    {
        get { return _scriptStatus; }
        set { SetProperty(ref _scriptStatus, value); }
    }
    private readonly IScriptManager _scriptManager;
    private readonly IFileDialogService _fileDialog;
    private readonly IVSCodeService _vscService;

    private string _loadedScript = string.Empty;
    public string LoadedScript
    {
        get { return _loadedScript; }
        set { SetProperty(ref _loadedScript, value); }
    }

    public IRelayCommand LoadScriptCommand { get; }
    public IRelayCommand EditScriptCommand { get; }
    public IRelayCommand OpenScriptRepoCommand { get; }
    public IRelayCommand OpenBrowserFormCommand { get; }
    public IRelayCommand OpenVSCodeCommand { get; }
    public IAsyncRelayCommand StartScriptAsyncCommand { get; }

    public LogTabItemViewModel ScriptLogs { get; }

    private void LoadScript()
    {
        string? file = _fileDialog.Open(_scriptPath, "Skua Scripts (*.cs)|*.cs");
        if (file is null)
            return;

        _scriptManager.LoadedScript = file;
        LoadedScript = $"{Path.GetFileName(file)}";
        ScriptStatus = "[Script loaded]";
    }

    private void EditScript()
    {
        if (string.IsNullOrEmpty(_scriptManager.LoadedScript))
            return;
        _vscService.Open(_scriptManager.LoadedScript);
    }
}
