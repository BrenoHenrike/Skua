using Skua.Core.Models;

namespace Skua.Core.Interfaces;
public interface IDialogService
{
    bool? ShowDialog<TViewModel>(TViewModel viewModel)
        where TViewModel : class;
    bool? ShowDialog<TViewModel>(TViewModel viewModel, string Title)
        where TViewModel : class;
    bool? ShowDialog<TViewModel>(TViewModel viewModel, Action<TViewModel> callback)
        where TViewModel : class;

    void ShowMessageBox(string message, string caption);
    bool? ShowMessageBox(string message, string caption, bool yesAndNo);
    DialogResult ShowMessageBox(string message, string caption, params string[] buttons);
}
