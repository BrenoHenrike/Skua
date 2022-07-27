using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models.GitHub;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels;
public partial class GitHubAuthViewModel : BotControlViewModelBase
{
    public GitHubAuthViewModel(IClipboardService clipboard, IBrowserService browserService, ISettingsService settingsService)
        : base("GitHub Authentication")
    {
        _clipboard = clipboard;
        _browserService = browserService;
        _settingsService = settingsService;
        OpenBrowserCommand = new RelayCommand(OpenBrowser);
        GetUserCodeCommand = new AsyncRelayCommand(GetUserCode);
        AuthorizeGHCommand = new AsyncRelayCommand(AuthorizeGH);
        if(HttpClients.UserGitHubClient is not null)
            HintStatus = "GitHub Authentication already done.";
    }

    private readonly IClipboardService _clipboard;
    private readonly IBrowserService _browserService;
    private readonly ISettingsService _settingsService;
    private DeviceCodeResponse? deviceCode;
    private TokenResponse? token;

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
        if (deviceCode is not null)
        {
            UserCode = deviceCode.UserCode;
            _clipboard.SetText(deviceCode.UserCode);
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
            { "client_id", "449f889db3d655d2ef4a" },
            { "scope", "public_repo" }
        };
        FormUrlEncodedContent encodedContent = new(content);
        HttpResponseMessage? response = await HttpClients.GetGHClient().PostAsync("https://github.com/login/device/code", encodedContent);
        return response?.IsSuccessStatusCode ?? false
            ? deviceCode = JsonConvert.DeserializeObject<DeviceCodeResponse>(await response.Content.ReadAsStringAsync())
            : null;
    }

    private async Task<TokenResponse?> GetAccessToken()
    {
        if (deviceCode is null)
            return null;
        Dictionary<string, string> content = new()
        {
            { "client_id", "449f889db3d655d2ef4a" },
            { "device_code", deviceCode.DeviceCode },
            { "grant_type", "urn:ietf:params:oauth:grant-type:device_code" }
        };
        FormUrlEncodedContent? encodedContent = new FormUrlEncodedContent(content);
        HttpResponseMessage? response = await HttpClients.GetGHClient().PostAsync("https://github.com/login/oauth/access_token", encodedContent);
        return response?.IsSuccessStatusCode ?? false
            ? token = JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync())
            : null;
    }

    private async Task AuthorizeGH()
    {
        IsBusy = true;
        await GetAccessToken();
        if (token is not null)
        {
            HttpClients.UserGitHubClient = new(token.AccessToken);
            _settingsService.Set("UserGitHubToken", token.AccessToken);
            string access = token.AccessToken;
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
        _browserService.Open("https://github.com/login/device");
        HintStatus = "Do the site verification.\r\nOnly click \"Authorize\" after seeing the All set page.";
    }

    public IAsyncRelayCommand GetUserCodeCommand { get; }
    public IRelayCommand OpenBrowserCommand { get; }
    public IAsyncRelayCommand AuthorizeGHCommand { get; }
}
