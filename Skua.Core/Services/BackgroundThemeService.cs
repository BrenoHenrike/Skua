using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using System.IO;

namespace Skua.Core.Services;

public class BackgroundThemeService : ObservableObject
{
    private readonly ISettingsService _settingsService;
    private string _themesFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Skua", "themes");
    private string _configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Skua", "background-config.json");
    public string[] defaultBackgrounds = new[]
    {
        "Generic2.swf", "Skyguard.swf", "Kezeroth.swf", "Mirror.swf", "DageScorn.swf", "ravenloss2.swf"
    };

    public BackgroundThemeService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        EnsureThemesFolderExists();
        GenerateBackgroundConfig();
    }

    private void EnsureThemesFolderExists()
    {
        if (!Directory.Exists(_themesFolder))
        {
            Directory.CreateDirectory(_themesFolder);
        }
    }

    public List<string> GetAvailableBackgrounds()
    {
        var backgrounds = defaultBackgrounds.ToList();

        if (Directory.Exists(_themesFolder))
        {
            var swfFiles = Directory.GetFiles(_themesFolder, "*.swf")
                .Select(Path.GetFileName)
                .Where(f => f != null)
                .Cast<string>()
                .ToList();

            backgrounds.AddRange(swfFiles);
        }

        return backgrounds.Distinct().ToList();
    }

    public string CurrentBackground
    {
        get => _settingsService.Get<string>("DefaultBackground") ?? "Generic2.swf";
        set
        {
            _settingsService.Set("DefaultBackground", value);
            GenerateBackgroundConfig();
            OnPropertyChanged();
        }
    }

    public bool IsLocalBackgroundFile(string backgroundName)
    {
        var localPath = Path.Combine(_themesFolder, backgroundName);
        return File.Exists(localPath);
    }

    public string ThemesFolder => _themesFolder;

    private bool IsDefaultAQWBackground(string background)
    {
        return defaultBackgrounds.Contains(background);
    }

    private void GenerateBackgroundConfig()
    {
        try
        {
            var config = new BackgroundConfig();
            var currentBg = CurrentBackground;
            if (IsLocalBackgroundFile(currentBg) && !IsDefaultAQWBackground(currentBg))
            {
                config.sBG = "hideme.swf";
                var localPath = Path.Combine(_themesFolder, currentBg);
                config.customBackground = $"file:///{localPath.Replace('\\', '/')}"; 
            }
            else
            {
                config.sBG = currentBg;
                config.customBackground = null;
            }

            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(_configFilePath, json);
        }
        catch (Exception)
        {
        }
    }
}
