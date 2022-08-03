using System;
using System.Windows;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.ViewModels;

namespace Skua.WPF.Services;
public class DialogService : IDialogService
{
    public bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : class
    {
        return Application.Current.Dispatcher.Invoke(() =>
        {
            HostDialog dialog = new()
            {
                DataContext = viewModel
            };
            return dialog.ShowDialog();
        });
    }
    public bool? ShowDialog<TViewModel>(TViewModel viewModel, string title) where TViewModel : class
    {
        return Application.Current.Dispatcher.Invoke(() =>
        {
            HostDialog dialog = new()
            {
                DataContext = viewModel
            };
            dialog.Title = title;
            return dialog.ShowDialog();
        });
    }

    public bool? ShowDialog<TViewModel>(TViewModel viewModel, Action<TViewModel> callback) where TViewModel : class
    {
        return Application.Current.Dispatcher.Invoke(() =>
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
        });
    }

    public void ShowMessageBox(string message, string caption)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            MessageBoxDialogViewModel viewModel = new(message, caption);
            HostDialog dialog = new()
            {
                DataContext = viewModel
            };

            dialog.ShowDialog();
        });
    }

    public bool? ShowMessageBox(string message, string caption, bool yesAndNo)
    {
        return Application.Current.Dispatcher.Invoke(() =>
        {
            MessageBoxDialogViewModel viewModel = new(message, caption, yesAndNo);
            HostDialog dialog = new()
            {
                DataContext = viewModel
            };

            return dialog.ShowDialog();
        });
    }

    public DialogResult ShowMessageBox(string message, string caption, params string[] buttons)
    {
        return Application.Current.Dispatcher.Invoke(() =>
        {
            CustomDialogViewModel viewModel = new(message, caption, buttons);
            HostDialog dialog = new()
            {
                DataContext = viewModel
            };

            dialog.ShowDialog();

            return viewModel.Result ?? DialogResult.Cancelled;
        });
    }
}
