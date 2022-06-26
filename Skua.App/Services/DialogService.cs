using System;
using Skua.Core.Interfaces;
using Skua.Core.ViewModels;

namespace Skua.App.Services;
public class DialogService : IDialogService
{
    public bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : class
    {
        HostDialog dialog = new()
        {
            DataContext = viewModel
        };
        return dialog.ShowDialog();
    }
    public bool? ShowDialog<TViewModel>(TViewModel viewModel, string title) where TViewModel : class
    {
        HostDialog dialog = new()
        {
            DataContext = viewModel
        };
        dialog.Title = title;
        return dialog.ShowDialog();
    }

    public bool? ShowDialog<TViewModel>(TViewModel viewModel, Action<TViewModel> callback) where TViewModel : class
    {
        HostDialog dialog = new()
        {
            DataContext = viewModel
        };

        void closeHandler(object? s, EventArgs e)
        {
            callback(viewModel);
            dialog.Closed -= closeHandler;
        }
        dialog.Closed += closeHandler;

        return dialog.ShowDialog();
    }

    public void ShowMessageBox(string message, string caption)
    {
        MessageBoxDialogViewModel viewModel = new(message, caption);
        HostDialog dialog = new()
        {
            DataContext = viewModel
        };

        dialog.ShowDialog();
    }

    public bool? ShowMessageBox(string message, string caption, bool yesAndNo)
    {
        MessageBoxDialogViewModel viewModel = new(message, caption, true);
        HostDialog dialog = new()
        {
            DataContext = viewModel
        };

        return dialog.ShowDialog();
    }
}
