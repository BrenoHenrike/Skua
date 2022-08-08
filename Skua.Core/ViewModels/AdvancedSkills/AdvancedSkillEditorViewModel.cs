using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Models.Skills;
using Skua.Core.Utils;
using Skua.Core.Messaging;

namespace Skua.Core.ViewModels;
public class AdvancedSkillEditorViewModel : ObservableRecipient
{
    public AdvancedSkillEditorViewModel(IDialogService dialogService)
    {
        Messenger.Register<AdvancedSkillEditorViewModel, EditAdvancedSkillMessage>(this, Edit);
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
        AddSkillToCurrentCommand = new RelayCommand<string>(AddSkill);
        SaveSkillsCommand = new RelayCommand(SaveSkills);
        MoveSkillDownCommand = new RelayCommand(MoveSkillDown);
        MoveSkillUpCommand = new RelayCommand(MoveSkillUp);
        SelectSkillDownCommand = new RelayCommand(SelectSkillDown);
        SelectSkillUpCommand = new RelayCommand(SelectSkillUp);
        RemoveSkillCommand = new RelayCommand(RemoveSkill);
        ClearSkillsCommand = new RelayCommand(CurrentSkillsList.Clear);
        EditSkillCommand = new RelayCommand(EditSkill);
        _dialogService = dialogService;
    }

    private void Edit(AdvancedSkillEditorViewModel recipient, EditAdvancedSkillMessage message)
    {
        recipient.CurrentSkillsList.Clear();
        recipient.CurrentSkillTimeout = message.AdvSkill.SkillTimeout;
        recipient.UseWaitModeBool = message.AdvSkill.SkillUseMode == SkillUseMode.UseIfAvailable ? false : true;
        recipient.CurrentClassName = message.AdvSkill.ClassName;
        recipient.SelectedClassUseMode = (int)message.AdvSkill.ClassUseMode;
        recipient.CurrentSkillsList.AddRange(message.AdvSkill.Skills.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(s => new SkillItemViewModel(s.Trim())));
    }

    private void EditSkill()
    {
        if (SelectedSkill is null)
            return;
        SkillRuleEditorDialogViewModel toEdit = new(new(SelectedSkill.UseRules));
        if (_dialogService.ShowDialog(toEdit) == true)
            SelectedSkill.UseRules = toEdit.UseRules;
    }

    private void SaveSkills()
    {
        string skills = string.Join(" | ", _currentSkillsList.Select(s => s.Convert()));
        AdvancedSkill advSkill = new(CurrentClassName, skills, CurrentSkillTimeout, SelectedClassUseMode, UseWaitModeBool ? SkillUseMode.WaitForCooldown : SkillUseMode.UseIfAvailable);
        Messenger.Send<SaveAdvancedSkillMessage>(new(advSkill));
    }

    private void AddSkill(string? value)
    {
        if (value is null)
            return;
        if (!int.TryParse(value, out int result))
            return;
        SkillItemViewModel info = new(result, UseRules);
        CurrentSkillsList.Add(info);
    }

    private int _currentSkillTimeout = 250;
    public int CurrentSkillTimeout
    {
        get { return _currentSkillTimeout; }
        set { SetProperty(ref _currentSkillTimeout, value); }
    }
    private bool _useWaitModeBool;
    public bool UseWaitModeBool
    {
        get { return _useWaitModeBool; }
        set { SetProperty(ref _useWaitModeBool, value); }
    }
    public SkillRulesViewModel UseRules { get; }

    private RangedObservableCollection<SkillItemViewModel> _currentSkillsList = new();
    public RangedObservableCollection<SkillItemViewModel> CurrentSkillsList
    {
        get { return _currentSkillsList; }
        set { SetProperty(ref _currentSkillsList, value); }
    }
    private SkillItemViewModel? _selectedSkill;
    public SkillItemViewModel? SelectedSkill
    {
        get { return _selectedSkill; }
        set { SetProperty(ref _selectedSkill, value); }
    }
    private int _selectedSkillIndex;
    public int SelectedSkillIndex
    {
        get { return _selectedSkillIndex; }
        set { SetProperty(ref _selectedSkillIndex, value); }
    }

    public string[] ClassUseModes { get; }
    private int _selectedClassUseMode;
    public int SelectedClassUseMode
    {
        get { return _selectedClassUseMode; }
        set { SetProperty(ref _selectedClassUseMode, value); }
    }
    private string _currentClassName = string.Empty;
    private readonly IDialogService _dialogService;

    public string CurrentClassName
    {
        get { return _currentClassName; }
        set { SetProperty(ref _currentClassName, value); }
    }

    public IRelayCommand AddSkillToCurrentCommand { get; }
    public IRelayCommand SaveSkillsCommand { get; }
    public IRelayCommand MoveSkillUpCommand { get; }
    public IRelayCommand MoveSkillDownCommand { get; }
    public IRelayCommand SelectSkillDownCommand { get; }
    public IRelayCommand SelectSkillUpCommand { get; }
    public IRelayCommand RemoveSkillCommand { get; }
    public IRelayCommand ClearSkillsCommand { get; }
    public IRelayCommand EditSkillCommand { get; }

    private void MoveSkillDown()
    {
        MoveSkill(1);
    }

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

    private void SelectSkillDown()
    {
        SelectSkill(1);
    }

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

    private void RemoveSkill()
    {
        if (SelectedSkill is null)
            return;
        int index = SelectedSkillIndex;
        CurrentSkillsList.RemoveAt(SelectedSkillIndex);
        SelectedSkillIndex = index - 1;
    }
}
