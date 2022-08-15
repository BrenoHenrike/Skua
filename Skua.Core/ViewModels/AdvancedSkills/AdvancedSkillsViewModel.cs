using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Messaging;

namespace Skua.Core.ViewModels;
public partial class AdvancedSkillsViewModel : BotControlViewModelBase
{
    public AdvancedSkillsViewModel(AdvancedSkillEditorViewModel editor, SavedAdvancedSkillsViewModel savedViewModel)
        : base("Advanced Skills", 700, 530)
    {
        EditViewModel = editor;
        SavedViewModel = savedViewModel;
    }

    protected override void OnActivated()
    {
        SavedViewModel.IsActive = true;
        EditViewModel.IsActive = true;
        Messenger.Register<AdvancedSkillsViewModel, EditAdvancedSkillMessage>(this, EditSkill);
    }

    protected override void OnDeactivated()
    {
        SavedViewModel.IsActive = false;
        EditViewModel.IsActive = false;
        base.OnDeactivated();
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
