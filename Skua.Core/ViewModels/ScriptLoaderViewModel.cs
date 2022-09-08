using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;

namespace Skua.Core.ViewModels;
public partial class ScriptLoaderViewModel : BotControlViewModelBase
{
    private readonly string _scriptPath;
    public ScriptLoaderViewModel(
        IProcessService processService,
        IFileDialogService fileDialog,
        IScriptManager scriptManager,
        IWindowService windowService,
        IDialogService dialogService,
        IEnumerable<LogTabViewModel> logs) 
        : base("Load Script", 350, 450)
    {
        StrongReferenceMessenger.Default.Register<ScriptLoaderViewModel, LoadScriptMessage, int>(this, (int)MessageChannels.ScriptStatus, LoadScript);
        StrongReferenceMessenger.Default.Register<ScriptLoaderViewModel, EditScriptMessage, int>(this, (int)MessageChannels.ScriptStatus, EditScript);
        StrongReferenceMessenger.Default.Register<ScriptLoaderViewModel, StartScriptMessage, int>(this, (int)MessageChannels.ScriptStatus, StartScript);
        StrongReferenceMessenger.Default.Register<ScriptLoaderViewModel, ToggleScriptMessage, int>(this, (int)MessageChannels.ScriptStatus, ToggleScript);
        StrongReferenceMessenger.Default.Register<ScriptLoaderViewModel, ScriptStartedMessage, int>(this, (int)MessageChannels.ScriptStatus, ScriptStarted);
        StrongReferenceMessenger.Default.Register<ScriptLoaderViewModel, ScriptStoppedMessage, int>(this, (int)MessageChannels.ScriptStatus, ScriptStopped);
        StrongReferenceMessenger.Default.Register<ScriptLoaderViewModel, ScriptStoppingMessage, int>(this, (int)MessageChannels.ScriptStatus, ScriptStopping);

        _scriptPath = Path.Combine(AppContext.BaseDirectory, "Scripts");
        ScriptLogs = logs.ToArray()[1];
        ScriptManager = scriptManager;
        _windowService = windowService;
        _processService = processService;
        _dialogService = dialogService;
        _fileDialog = fileDialog;
    }

    public IScriptManager ScriptManager { get; }

    private readonly IWindowService _windowService;
    private readonly IProcessService _processService;
    private readonly IDialogService _dialogService;
    private readonly IFileDialogService _fileDialog;
    public LogTabViewModel ScriptLogs { get; }

    [ObservableProperty]
    private string _scriptErrorToolTip = string.Empty;
    [ObservableProperty]
    private bool _toggleScriptEnabled = true;
    [ObservableProperty]
    private string _scriptStatus = "[No Script Loaded]";
    [ObservableProperty]
    private string _loadedScript = string.Empty;

    [RelayCommand]
    private void OpenBrowserForm()
    {
        _processService.OpenLink(@"https://forms.gle/sbp57LBQP5WvCH2B9");
    }

    [RelayCommand]
    private void OpenScriptRepo()
    {
        _windowService.ShowManagedWindow("Script Repo");
    }

    [RelayCommand]
    private void OpenVSCode()
    {
        _processService.OpenVSC();
    }

    private async Task StartScriptAsync(string? path = null)
    {
        if (string.IsNullOrWhiteSpace(path))
            return;

        ScriptManager.SetLoadedScript(path);

        if (ScriptManager.ScriptRunning)
            await ScriptManager.StopScriptAsync();
        await StartScript();
    }

    [RelayCommand]
    private async Task ToggleScript()
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

    [RelayCommand]
    private void LoadScript(string? path = null)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            path = _fileDialog.OpenFile(_scriptPath, "Skua Scripts (*.cs)|*.cs");
            if (path is null)
                return;
        }

        ScriptManager.SetLoadedScript(path);
        LoadedScript = Path.GetFileName(path) ?? string.Empty;
        ScriptStatus = "[Script loaded]";
    }

    [RelayCommand]
    private void EditScript(string? path = null)
    {
        if (path is null && string.IsNullOrEmpty(ScriptManager.LoadedScript))
            return;

        _processService.OpenVSC(path ?? ScriptManager.LoadedScript);
    }

    [RelayCommand]
    private async Task EditScriptConfig()
    {
        if (string.IsNullOrWhiteSpace(ScriptManager.LoadedScript))
        {
            _dialogService.ShowMessageBox("No script is currently loaded. Please load a script to edit its options.", "No Script Loaded");
            return;
        }

        try
        {
            object compiled = await Task.Run(() => ScriptManager.Compile(File.ReadAllText(ScriptManager.LoadedScript))!);
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

    private async void ToggleScript(ScriptLoaderViewModel recipient, ToggleScriptMessage message)
    {
        await recipient.ToggleScript();
    }

    private async void StartScript(ScriptLoaderViewModel recipient, StartScriptMessage message)
    {
        await recipient.StartScriptAsync(message.Path);
    }

    private void EditScript(ScriptLoaderViewModel recipient, EditScriptMessage message)
    {
        recipient.EditScript(message.Path);
    }

    private void LoadScript(ScriptLoaderViewModel recipient, LoadScriptMessage message)
    {
        recipient.LoadScript(message.Path);
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
}
