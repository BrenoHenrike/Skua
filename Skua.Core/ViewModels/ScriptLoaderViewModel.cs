using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;

namespace Skua.Core.ViewModels;
public partial class ScriptLoaderViewModel : BotControlViewModelBase
{
    private readonly string _scriptPath;
    public ScriptLoaderViewModel(
        IProcessStartService processService,
        IFileDialogService fileDialog,
        IScriptManager scriptManager,
        IWindowService windowService,
        IDialogService dialogService,
        IEnumerable<LogTabViewModel> logs) 
        : base("Load Script", 350, 450)
    {
        Messenger.Register<ScriptLoaderViewModel, LoadScriptMessage>(this, ReceiveLoadScript);
        Messenger.Register<ScriptLoaderViewModel, EditScriptMessage>(this, ReceiveEditScript);
        Messenger.Register<ScriptLoaderViewModel, StartScriptMessage>(this, ReceiveToggleScript);
        Messenger.Register<ScriptLoaderViewModel, ScriptStartedMessage>(this, ScriptStarted);
        Messenger.Register<ScriptLoaderViewModel, ScriptStoppedMessage>(this, ScriptStopped);
        Messenger.Register<ScriptLoaderViewModel, ScriptStoppingMessage>(this, ScriptStopping);

        _scriptPath = Path.Combine(AppContext.BaseDirectory, "Scripts");
        ScriptLogs = logs.ToArray()[1];
        ScriptManager = scriptManager;
        _windowService = windowService;
        _processService = processService;
        _dialogService = dialogService;
        _fileDialog = fileDialog;
        LoadScriptCommand = new RelayCommand(() => LoadScript());
        OpenVSCodeCommand = new RelayCommand(_processService.OpenVSC);
        EditScriptCommand = new RelayCommand(() => EditScript());
        EditScriptConfigCommand = new RelayCommand(EditScriptConfig);
        OpenScriptRepoCommand = new RelayCommand(_windowService.ShowWindow<ScriptRepoViewModel>);
        OpenBrowserFormCommand = new RelayCommand(() => _processService.OpenLink(@"https://forms.gle/sbp57LBQP5WvCH2B9"));
        ToggleScriptAsyncCommand = new AsyncRelayCommand(ToggleScriptAsync);
    }

    private void ScriptStopping(ScriptLoaderViewModel recipient, ScriptStoppingMessage message)
    {
        recipient.ToggleScriptEnabled = false;
        recipient.ScriptStatus = "Stopping...";
    }

    private void ScriptStopped(ScriptLoaderViewModel recipient, ScriptStoppedMessage message)
    {
        recipient.ToggleScriptEnabled = true;
        recipient.ScriptStatus = "[Stopped]";
    }

    private void ScriptStarted(ScriptLoaderViewModel recipient, ScriptStartedMessage message)
    {
        recipient.ToggleScriptEnabled = true;
    }

    public IScriptManager ScriptManager { get; }

    private readonly IWindowService _windowService;
    private readonly IProcessStartService _processService;
    private readonly IDialogService _dialogService;
    private readonly IFileDialogService _fileDialog;
    public LogTabViewModel ScriptLogs { get; }

    [ObservableProperty]
    private string _ScriptErrorToolTip = string.Empty;
    [ObservableProperty]
    private bool _ToggleScriptEnabled = true;
    [ObservableProperty]
    private string _scriptStatus = "[No Script Loaded]";
    [ObservableProperty]
    private string _loadedScript = string.Empty;

    public IRelayCommand LoadScriptCommand { get; }
    public IRelayCommand EditScriptCommand { get; }
    public IRelayCommand EditScriptConfigCommand { get; }
    public IRelayCommand OpenScriptRepoCommand { get; }
    public IRelayCommand OpenBrowserFormCommand { get; }
    public IRelayCommand OpenVSCodeCommand { get; }
    public IAsyncRelayCommand ToggleScriptAsyncCommand { get; }

    private async Task StartScriptAsync(string? path = null)
    {
        if (string.IsNullOrWhiteSpace(path))
            return;

        ScriptManager.SetLoadedScript(path);

        if (ScriptManager.ScriptRunning)
            await ScriptManager.StopScriptAsync();
        await StartScript();
    }

    private async Task ToggleScriptAsync()
    {
        ToggleScriptEnabled = false;
        if (string.IsNullOrWhiteSpace(ScriptManager.LoadedScript))
        {
            _dialogService.ShowMessageBox("No script loaded.", "Scripts");
            ToggleScriptEnabled = true;
            return;
        }

        if (ScriptManager.ScriptRunning)
        {
            ScriptManager.StopScript();
            ToggleScriptEnabled = true;
            return;
        }

        await StartScript();
    }

    private async Task StartScript()
    {
        ScriptStatus = "Compiling...";
        await Task.Run(async () =>
        {
            Exception? ex = await ScriptManager.StartScriptAsync();
            if (ex is not null)
            {
                _dialogService.ShowMessageBox($"Error while starting script:\r\n{ex.Message}", "Script Error");
                ScriptStatus = "[Error]";
                ScriptErrorToolTip = $"Error while starting script:\r\n{ex}";
                ToggleScriptEnabled = true;
            }
            ScriptStatus = "[Running]";
        });
    }

    private void LoadScript(string? path = null)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            path = _fileDialog.Open(_scriptPath, "Skua Scripts (*.cs)|*.cs");
            if (path is null)
                return;
        }

        ScriptManager.SetLoadedScript(path);
        LoadedScript = Path.GetFileName(path) ?? string.Empty;
        ScriptStatus = "[Script loaded]";
    }

    private void EditScript(string? path = null)
    {
        if (path is null && string.IsNullOrEmpty(ScriptManager.LoadedScript))
            return;

        if(!string.IsNullOrWhiteSpace(path))
        {
            _processService.OpenVSC(path);
            return;
        }

        _processService.OpenVSC(ScriptManager.LoadedScript);
    }

    private void EditScriptConfig()
    {
        if (string.IsNullOrWhiteSpace(ScriptManager.LoadedScript))
        {
            _dialogService.ShowMessageBox("No script is currently loaded. Please load a script to edit its options.", "No Script Loaded");
            return;
        }

        try
        {
            object compiled = ScriptManager.Compile(File.ReadAllText(ScriptManager.LoadedScript))!;
            ScriptManager.LoadScriptConfig(compiled);
            if (ScriptManager.Config!.Options.Count > 0 || ScriptManager.Config.MultipleOptions.Count > 0)
                ScriptManager.Config.Configure();
            else
                _dialogService.ShowMessageBox("The loaded script has no options to configure.", "No Options");
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessageBox($"Script cannot be configured as it has compilation errors:\r\n{ex}", "Script Error");
        }
    }

    private async void ReceiveToggleScript(ScriptLoaderViewModel recipient, StartScriptMessage message)
    {
        await recipient.StartScriptAsync(message.Path);
    }

    private void ReceiveEditScript(ScriptLoaderViewModel recipient, EditScriptMessage message)
    {
        recipient.EditScript(message.Path);
    }

    private void ReceiveLoadScript(ScriptLoaderViewModel recipient, LoadScriptMessage message)
    {
        recipient.LoadScript(message.Path);
    }
}
