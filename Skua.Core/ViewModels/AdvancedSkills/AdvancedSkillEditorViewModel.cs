using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Models.Skills;
using Skua.Core.Utils;
using Skua.Core.Messaging;

namespace Skua.Core.ViewModels;
public partial class AdvancedSkillEditorViewModel : ObservableRecipient
{
    public AdvancedSkillEditorViewModel(IDialogService dialogService)
    {
        ClassUseModes = new[]
        {
            "Base",
            "Attack",
            "Defense",
            "Farm",
            "Solo",
            "Support"
        };
        UseRules = new();
        ClearSkillsCommand = new RelayCommand(CurrentSkillsList.Clear);
        _dialogService = dialogService;
    }

    protected override void OnActivated()
    {
        Messenger.Register<AdvancedSkillEditorViewModel, EditAdvancedSkillMessage>(this, EditSkill);
    }

    private readonly IDialogService _dialogService;
    [ObservableProperty]
    private int _currentSkillTimeout = 250;
    [ObservableProperty]
    private bool _useWaitModeBool;
    [ObservableProperty]
    private RangedObservableCollection<SkillItemViewModel> _currentSkillsList = new();
    [ObservableProperty]
    private SkillItemViewModel? _selectedSkill;
    [ObservableProperty]
    private int _selectedSkillIndex;
    [ObservableProperty]
    private int _selectedClassUseMode;
    [ObservableProperty]
    private string _currentClassName = string.Empty;

    public string[] ClassUseModes { get; }
    public SkillRulesViewModel UseRules { get; }
    public IRelayCommand ClearSkillsCommand { get; }

    [RelayCommand]
    private void EditSkill()
    {
        if (SelectedSkill is null)
            return;
        SkillRuleEditorDialogViewModel toEdit = new(new(SelectedSkill.UseRules));
        if (_dialogService.ShowDialog(toEdit) == true)
            SelectedSkill.UseRules = toEdit.UseRules;
    }

    [RelayCommand]
    private void SaveSkills()
    {
        string skills = string.Join(" | ", _currentSkillsList.Select(s => s.Convert()));
        AdvancedSkill advSkill = new(CurrentClassName, skills, CurrentSkillTimeout, SelectedClassUseMode, UseWaitModeBool ? SkillUseMode.WaitForCooldown : SkillUseMode.UseIfAvailable);
        Messenger.Send<SaveAdvancedSkillMessage>(new(advSkill));
        OnPropertyChanged(nameof(CurrentSkillsList));
    }

    [RelayCommand]
    private void AddSkillToCurrent(string? value)
    {
        if (value is null)
            return;
        if (!int.TryParse(value, out int result))
            return;
        SkillItemViewModel info = new(result, UseRules);
        CurrentSkillsList.Add(info);
    }

    [RelayCommand]
    private void MoveSkillDown()
    {
        MoveSkill(1);
    }
    [RelayCommand]
    private void MoveSkillUp()
    {
        MoveSkill(-1);
    }
    private void MoveSkill(int direction)
    {
        if (SelectedSkill is null)
        {
            SelectedSkillIndex = 0;
            return;
        }

        int newIndex = SelectedSkillIndex + direction;

        if (newIndex < 0 || newIndex >= _currentSkillsList.Count)
            return;

        CurrentSkillsList.Swap(newIndex, SelectedSkillIndex);
        SelectedSkillIndex = newIndex;
    }

    [RelayCommand]
    private void SelectSkillDown()
    {
        SelectSkill(1);
    }
    [RelayCommand]
    private void SelectSkillUp()
    {
        SelectSkill(-1);
    }
    private void SelectSkill(int direction)
    {
        if (SelectedSkill is null)
        {
            SelectedSkillIndex = 0;
            return;
        }

        int newIndex = SelectedSkillIndex + direction;
        if (newIndex < 0 || newIndex >= _currentSkillsList.Count)
            return;

        SelectedSkillIndex = newIndex;
    }

    [RelayCommand]
    private void RemoveSkill()
    {
        if (SelectedSkill is null)
            return;
        int index = SelectedSkillIndex;
        CurrentSkillsList.RemoveAt(SelectedSkillIndex);
        SelectedSkillIndex = index - 1;
    }

    private void EditSkill(AdvancedSkillEditorViewModel recipient, EditAdvancedSkillMessage message)
    {
        recipient.CurrentSkillsList.Clear();
        recipient.CurrentSkillTimeout = message.AdvSkill.SkillTimeout;
        recipient.UseWaitModeBool = message.AdvSkill.SkillUseMode == SkillUseMode.UseIfAvailable ? false : true;
        recipient.CurrentClassName = message.AdvSkill.ClassName;
        recipient.SelectedClassUseMode = (int)message.AdvSkill.ClassUseMode;
        recipient.CurrentSkillsList.AddRange(message.AdvSkill.Skills.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(s => new SkillItemViewModel(s.Trim())));
    }
}
