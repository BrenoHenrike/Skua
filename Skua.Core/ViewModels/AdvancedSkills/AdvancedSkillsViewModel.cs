namespace Skua.Core.ViewModels;
public class AdvancedSkillsViewModel : BotControlViewModelBase
{
    public AdvancedSkillsViewModel(AdvancedSkillEditorViewModel editor)
        : base("Advanced Skills")
    {
        EditViewModel = editor;
    }

    public AdvancedSkillEditorViewModel EditViewModel { get; }
}
