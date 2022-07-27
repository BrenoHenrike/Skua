using Microsoft.Extensions.DependencyInjection;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.ViewModels;
using System;
using System.Collections.Generic;

namespace Skua.App.WPF.AppStartup;
internal static class LogTabs
{
    internal static IEnumerable<LogTabViewModel> CreateViewModels(IServiceProvider s)
    {
        ILogService logService = s.GetService<ILogService>()!;
        IClipboardService clipBoard = s.GetService<IClipboardService>()!;
        IFileDialogService fileDialog = s.GetService<IFileDialogService>()!;
        return new[]
        {
            Create("Debug", LogType.Debug),
            Create("Script", LogType.Script),
            Create("Flash", LogType.Flash)
        };

        LogTabViewModel Create(string title, LogType logType) => new(title, logService, clipBoard, fileDialog, logType);
    }
}
