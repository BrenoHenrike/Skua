using Skua.Core.Models;

namespace Skua.Core.ViewModels;
public class CustomDialogViewModel : DialogViewModelBase
{
    public CustomDialogViewModel(string message, string caption, IEnumerable<string> buttons) : base(caption)
    {
        Buttons = buttons.ToList();
        Message = message;
    }

    public List<string> Buttons { get; }

    public DialogResult? Result { get; set; }
    public string Message { get; }
}
