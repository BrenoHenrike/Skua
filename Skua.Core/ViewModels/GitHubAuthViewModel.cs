using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models.GitHub;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels;
public partial class GitHubAuthViewModel : BotControlViewModelBase
{
    public GitHubAuthViewModel(IClipboardService clipboard, IProcessStartService processService, ISettingsService settingsService)
        : base("GitHub Authentication")
    {
        _clipboard = clipboard;
        _processService = processService;
        _settingsService = settingsService;
        OpenBrowserCommand = new RelayCommand(OpenBrowser);
        GetUserCodeCommand = new AsyncRelayCommand(GetUserCode);
        AuthorizeGHCommand = new AsyncRelayCommand(AuthorizeGH);
        if(HttpClients.UserGitHubClient is not null)
            HintStatus = "GitHub Authentication already done.";
    }

    private readonly IProcessStartService _processService;
    private readonly ISettingsService _settingsService;
    private readonly IClipboardService _clipboard;
    private DeviceCodeResponse? _deviceCode;
    private TokenResponse? _token;

    [ObservableProperty]
    private string _userCode = string.Empty;
    [ObservableProperty]
    private string _hintStatus = string.Empty;
    [ObservableProperty]
    private bool _isBusy;

    private async Task GetUserCode()
    {
        IsBusy = true;
        await GetDeviceCode();
        if (_deviceCode is not null)
        {
            UserCode = _deviceCode.UserCode;
            _clipboard.SetText(_deviceCode.UserCode);
            HintStatus = "Copied. Click Open Browser.";
            IsBusy = false;
            return;
        }
        for(int i = 10; i >= 0; i--)
        {
            HintStatus = $"Error. Please, wait {i} seconds.";
            await Task.Delay(1000);
        }
        IsBusy = false;
        HintStatus = "Try again";
    }
    private async Task<DeviceCodeResponse?> GetDeviceCode()
    {
        Dictionary<string, string>? content = new()
        {
            { "client_id", "Iv1.c670c450985a5363" },
            { "scope", "public_repo" }
        };
        FormUrlEncodedContent encodedContent = new(content);
        HttpResponseMessage? response = await HttpClients.GetGHClient().PostAsync("https://github.com/login/device/code", encodedContent);
        return response?.IsSuccessStatusCode ?? false
            ? _deviceCode = JsonConvert.DeserializeObject<DeviceCodeResponse>(await response.Content.ReadAsStringAsync())
            : null;
    }

    private async Task<TokenResponse?> GetAccessToken()
    {
        if (_deviceCode is null)
            return null;
        Dictionary<string, string> content = new()
        {
            { "client_id", "Iv1.c670c450985a5363" },
            { "device_code", _deviceCode.DeviceCode },
            { "grant_type", "urn:ietf:params:oauth:grant-type:device_code" }
        };
        FormUrlEncodedContent? encodedContent = new(content);
        HttpResponseMessage? response = await HttpClients.GetGHClient().PostAsync("https://github.com/login/oauth/access_token", encodedContent);
        return response?.IsSuccessStatusCode ?? false
            ? _token = JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync())
            : null;
    }

    private async Task AuthorizeGH()
    {
        IsBusy = true;
        await GetAccessToken();
        if (_token is not null)
        {
            HttpClients.UserGitHubClient = new(_token.AccessToken);
            _settingsService.Set("UserGitHubToken", _token.AccessToken);
            IsBusy = false;
            HintStatus = "All good to go!";
            return;
        }
        for(int i = 10; i >= 0; i--)
        {
            HintStatus = $"Error. Please, wait {i} seconds";
            await Task.Delay(1000);
        }
    }

    public void OpenBrowser()
    {
        _processService.OpenLink("https://github.com/login/device");
        HintStatus = "Do the site verification.\r\nOnly click \"Authorize\" after seeing the All set page.";
    }

    public IAsyncRelayCommand GetUserCodeCommand { get; }
    public IRelayCommand OpenBrowserCommand { get; }
    public IAsyncRelayCommand AuthorizeGHCommand { get; }
}
