using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Messaging;

namespace Skua.Core.ViewModels;
public class AdvancedSkillsViewModel : BotControlViewModelBase
{
    public AdvancedSkillsViewModel(AdvancedSkillEditorViewModel editor, SavedAdvancedSkillsViewModel savedViewModel)
        : base("Advanced Skills", 700, 530)
    {
        Messenger.Register<AdvancedSkillsViewModel, EditAdvancedSkillMessage>(this, Receive);
        EditViewModel = editor;
        SavedViewModel = savedViewModel;
    }

    private void Receive(AdvancedSkillsViewModel recipient, EditAdvancedSkillMessage message)
    {
        recipient.EditExpanded = true;
    }

    private bool _editExpanded;
    public bool EditExpanded
    {
        get { return _editExpanded; }
        set { SetProperty(ref _editExpanded, value); }
    }

    public AdvancedSkillEditorViewModel EditViewModel { get; }
    public SavedAdvancedSkillsViewModel SavedViewModel { get; }
}
