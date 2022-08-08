using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models.Skills;

namespace Skua.Core.ViewModels;
public partial class SavedAdvancedSkillsViewModel : ObservableRecipient
{
    public SavedAdvancedSkillsViewModel(IAdvancedSkillContainer advancedSkillContainer)
    {
        Messenger.Register<SavedAdvancedSkillsViewModel, SaveAdvancedSkillMessage>(this, SaveSkill);
        Messenger.Register<SavedAdvancedSkillsViewModel, PropertyChangedMessage<List<AdvancedSkill>>>(this, AdvancedSkillsChanged);
        _advancedSkillContainer = advancedSkillContainer;
        RefreshSkillsCommand = new RelayCommand(_advancedSkillContainer.LoadSkills);
        EditSelectedCommand = new RelayCommand(EditSelected);
        RemoveSelectedCommand = new RelayCommand(RemoveSelected);
    }

    private readonly IAdvancedSkillContainer _advancedSkillContainer;

    public List<AdvancedSkill> LoadedSkills => _advancedSkillContainer.LoadedSkills;
    [ObservableProperty]
    private AdvancedSkill? _selectedSkill;

    public IRelayCommand RefreshSkillsCommand { get; }
    public IRelayCommand EditSelectedCommand { get; }
    public IRelayCommand RemoveSelectedCommand { get; }

    private void RemoveSelected()
    {
        if (SelectedSkill is null)
            return;

        _advancedSkillContainer.Remove(SelectedSkill);
    }

    private void EditSelected()
    {
        if (SelectedSkill is null)
            return;

        Messenger.Send<EditAdvancedSkillMessage>(new(SelectedSkill));
    }

    private void SaveSkill(SavedAdvancedSkillsViewModel recipient, SaveAdvancedSkillMessage message)
    {
        recipient._advancedSkillContainer.TryOverride(message.AdvSkill);
    }

    private void AdvancedSkillsChanged(SavedAdvancedSkillsViewModel recipient, PropertyChangedMessage<List<AdvancedSkill>> message)
    {
        if (message.PropertyName == nameof(IAdvancedSkillContainer.LoadedSkills))
            recipient.OnPropertyChanged(nameof(LoadedSkills));
    }
}
