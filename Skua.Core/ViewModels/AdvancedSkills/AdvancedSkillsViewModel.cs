using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Messaging;

namespace Skua.Core.ViewModels;
public partial class AdvancedSkillsViewModel : BotControlViewModelBase
{
    public AdvancedSkillsViewModel(AdvancedSkillEditorViewModel editor, SavedAdvancedSkillsViewModel savedViewModel)
        : base("Advanced Skills", 700, 530)
    {
        Messenger.Register<AdvancedSkillsViewModel, EditAdvancedSkillMessage>(this, EditSkill);
        EditViewModel = editor;
        SavedViewModel = savedViewModel;
    }

    public AdvancedSkillEditorViewModel EditViewModel { get; }
    public SavedAdvancedSkillsViewModel SavedViewModel { get; }

    [ObservableProperty]
    private bool _editExpanded;

    private void EditSkill(AdvancedSkillsViewModel recipient, EditAdvancedSkillMessage message)
    {
        recipient.EditExpanded = true;
    }
}
