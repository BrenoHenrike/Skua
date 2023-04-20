using CommandLine;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using System.Threading.Tasks;
using System.Windows;

namespace Skua.WPF;

public sealed class SkuaStartupHandler
{
    private readonly IScriptInterface _bot;
    private readonly ISettingsService _settingsService;
    private readonly IThemeService _themeService;
    private ParserResult<SkuaOptions> _result;

    public SkuaStartupHandler(string[] args, IScriptInterface bot, ISettingsService settingsService, IThemeService themeService)
    {
        _bot = bot;
        _bot.Flash.FlashCall += LoadGame;
        _result = Parser.Default.ParseArguments<SkuaOptions>(args);
        _settingsService = settingsService;
        _themeService = themeService;
    }

    private void LoadGame(string function, params object[] args)
    {
        if (function == "requestLoadGame")
        {
            _bot.Flash.Call("loadClient");
            _bot.Flash.FlashCall -= LoadGame;
        }
    }

    private void Login(string function, params object[] args)
    {
        if (function == "loaded")
        {
            _bot.Flash.FlashCall -= Login;
            Task.Factory.StartNew(() =>
            {
                _bot.Servers.Relogin(_result.Value.Server);

                if (!string.IsNullOrEmpty(_result.Value.Script))
                    StrongReferenceMessenger.Default.Send<StartScriptMessage, int>(new(_result.Value.Script), (int)MessageChannels.ScriptStatus);
            });
        }
    }

    public bool Execute()
    {
        if (_result is null)
            return false;

        if (_result.Tag == ParserResultType.NotParsed)
        {
            MessageBox.Show($"Error while parsing command line arguments", "Cmd Args Error");
            return false;
        }

        var options = _result.Value;

        if (!string.IsNullOrEmpty(options.Username) && !string.IsNullOrEmpty(options.Password))
        {
            _bot.Flash.FlashCall += Login;
            _bot.Servers.SetLoginInfo(options.Username, options.Password);
        }

        if (!string.IsNullOrEmpty(options.GitHubToken))
            _settingsService.Set("UserGitHubToken", options.GitHubToken);

        if (!string.IsNullOrEmpty(options.UseTheme) && options.UseTheme != "no-theme")
            _themeService.SetCurrentTheme(ThemeItem.FromString(options.UseTheme));

        return true;
    }
}

public sealed class SkuaOptions
{
    [Option('u', "user", HelpText = "Username of the account to login when starting the app.")]
    public string Username { get; set; }
    [Option('p', "password", HelpText = "Password of the account to login when starting the app.")]
    public string Password { get; set; }
    [Option('s', "server", HelpText = "Server to login, will default to 'Twilly'", Default = "Twilly")]
    public string Server { get; set; }
    [Option("run-script", HelpText = "File path of a script to be started up after starting the app.")]
    public string Script { get; set; }
    [Option("use-theme")]
    public string UseTheme { get; set; }
    [Option("gh-token")]
    public string GitHubToken { get; set; }
}
