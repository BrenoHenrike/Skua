using Skua.Core.Models;

namespace Skua.Core.ViewModels;
public class CustomDialogViewModel : DialogViewModelBase
{
    public CustomDialogViewModel(string message, string caption, IEnumerable<string> buttons) : base(caption)
    {
        Buttons = buttons.ToList();
        Message = message;
    }

    public string Message { get; }
    public List<string> Buttons { get; }
    public DialogResult? Result { get; set; }
}
