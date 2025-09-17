using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Services;
using Skua.Core.Utils;
using System.IO;

namespace Skua.Core.ViewModels;

public class BackgroundThemeViewModel : ObservableObject
{
    private readonly BackgroundThemeService _backgroundService;
    private readonly IFileDialogService _fileDialogService;

    public BackgroundThemeViewModel(BackgroundThemeService backgroundService, IFileDialogService fileDialogService)
    {
        _backgroundService = backgroundService;
        _fileDialogService = fileDialogService;
        BrowseBackgroundCommand = new AsyncRelayCommand(BrowseBackgroundAsync);
        OpenThemesFolderCommand = new RelayCommand(OpenThemesFolder);
        RefreshBackgroundsCommand = new RelayCommand(() => OnPropertyChanged(nameof(AvailableBackgrounds)));
        GetBackgroundsCommand = new RelayCommand(GetBackgrounds);
    }

    public List<string> AvailableBackgrounds => _backgroundService.GetAvailableBackgrounds();

    public string SelectedBackground
    {
        get => _backgroundService.CurrentBackground;
        set
        {
            if (_backgroundService.CurrentBackground != value)
            {
                _backgroundService.CurrentBackground = value;
                OnPropertyChanged();
            }
        }
    }



    public IAsyncRelayCommand BrowseBackgroundCommand { get; }
    public IRelayCommand OpenThemesFolderCommand { get; }
    public IRelayCommand RefreshBackgroundsCommand { get; }
    public IRelayCommand GetBackgroundsCommand { get; }

    private async Task BrowseBackgroundAsync()
    {
        var selectedFilePath = _fileDialogService.OpenFile(
            _backgroundService.ThemesFolder,
            "SWF files (*.swf)|*.swf|All files (*.*)|*.*");

        if (!string.IsNullOrEmpty(selectedFilePath))
        {
            var fileName = Path.GetFileName(selectedFilePath);
            var destinationPath = Path.Combine(_backgroundService.ThemesFolder, fileName);

            if (selectedFilePath != destinationPath)
            {
                try
                {
                    File.Copy(selectedFilePath, destinationPath, true);
                }
                catch (Exception)
                {
                    return;
                }
            }

            OnPropertyChanged(nameof(AvailableBackgrounds));
            SelectedBackground = fileName;
        }
    }


    private void OpenThemesFolder()
    {
        try
        {
            System.Diagnostics.Process.Start("explorer.exe", _backgroundService.ThemesFolder);
        }
        catch (Exception)
        {
        }
    }

    private void GetBackgrounds()
    {
        try
        {
            Link.OpenBrowser("https://github.com/SharpTheNightmare/SkuaBackgrounds");
        }
        catch (Exception)
        {
        }
    }
}