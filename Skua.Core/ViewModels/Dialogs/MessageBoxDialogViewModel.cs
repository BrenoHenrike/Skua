namespace Skua.Core.ViewModels;

public class MessageBoxDialogViewModel : DialogViewModelBase
{
    public MessageBoxDialogViewModel(string message, string caption)
        : base(caption)
    {
        Message = message;
        YesAndNo = false;
    }

    public MessageBoxDialogViewModel(string message, string caption, bool yesAndNo)
        : base(caption)
    {
        Message = message;
        YesAndNo = yesAndNo;
    }

    public string Message { get; }
    public bool YesAndNo { get; }
}