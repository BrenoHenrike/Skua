namespace Skua.Core.ViewModels;
public class FastTravelEditorDialogViewModel : DialogViewModelBase
{
    public FastTravelEditorDialogViewModel(FastTravelEditorViewModel fastTravelEditor)
        : base("Edit Fast Travel")
    {
        Editor = fastTravelEditor;
    }
    public FastTravelEditorViewModel Editor { get; }
}
