﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models.Skills;
using System.Collections.ObjectModel;

namespace Skua.Core.ViewModels;
public partial class SavedAdvancedSkillsViewModel : ObservableRecipient
{
    public SavedAdvancedSkillsViewModel(IAdvancedSkillContainer advancedSkillContainer)
    {
        _advancedSkillContainer = advancedSkillContainer;
        RefreshSkillsCommand = new RelayCommand(_advancedSkillContainer.LoadSkills);
        ResetSkillsSetCommand = new RelayCommand(_advancedSkillContainer.ResetSkillsSets);
    }

    protected override void OnActivated()
    {
        Messenger.Register<SavedAdvancedSkillsViewModel, SaveAdvancedSkillMessage>(this, SaveSkill);
        Messenger.Register<SavedAdvancedSkillsViewModel, PropertyChangedMessage<List<AdvancedSkill>>>(this, AdvancedSkillsChanged);
    }

    private readonly IAdvancedSkillContainer _advancedSkillContainer;
    [ObservableProperty]
    private AdvancedSkill? _selectedSkill;

    private ObservableCollection<AdvancedSkill>? _loadedSkills;
    public ObservableCollection<AdvancedSkill> LoadedSkills
    {
        get
        {
            // Only create new collection if it doesn't exist or needs refresh
            if (_loadedSkills == null)
            {
                _loadedSkills = new ObservableCollection<AdvancedSkill>(_advancedSkillContainer.LoadedSkills);
            }
            return _loadedSkills;
        }
        set
        {
            _loadedSkills = value;
            OnPropertyChanged(nameof(LoadedSkills));
        }
    }

    public IRelayCommand RefreshSkillsCommand { get; }
    public IRelayCommand ResetSkillsSetCommand { get; }

    [RelayCommand]
    private void RemoveSelected()
    {
        if (SelectedSkill is null)
            return;

        _advancedSkillContainer.Remove(SelectedSkill);
    }

    [RelayCommand]
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
        {
            // Force refresh of the collection
            recipient._loadedSkills = null;
            recipient.OnPropertyChanged(nameof(recipient.LoadedSkills));
        }
    }
}
