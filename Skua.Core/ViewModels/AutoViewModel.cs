using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Skills;

namespace Skua.Core.ViewModels;
public partial class AutoViewModel : BotControlViewModelBase
{
    public AutoViewModel(IScriptAuto auto, IScriptInventory inventory, IAdvancedSkillContainer advancedSkills)
        : base("Auto Attack")
    {
        Auto = auto;
        _inventory = inventory;
        _advancedSkills = advancedSkills;
        StartAutoAttackCommand = new AsyncRelayCommand(StartAutoAttack);
        StartAutoHuntCommand = new AsyncRelayCommand(StartAutoHunt);
        StopAutoAsyncCommand = new AsyncRelayCommand(async () => await Auto.StopAsync());
        ReloadClassesCommand = new RelayCommand(ReloadClasses);
    }

    private readonly IScriptInventory _inventory;
    private readonly IAdvancedSkillContainer _advancedSkills;
    public IScriptAuto Auto { get; }

    public List<string>? PlayerClasses => _inventory.Items?.Where(i => i.Category == ItemCategory.Class).Select(i => i.Name).ToList();
    private string? _selectedClass;
    public string? SelectedClass
    {
        get { return _selectedClass; }
        set
        {
            if(SetProperty(ref _selectedClass, value) && value is not null)
            {
                CurrentClassModes = new();
                CurrentClassModes.AddRange(_advancedSkills.LoadedSkills.Where(s => s.ClassName == _selectedClass).Select(s => s.ClassUseMode));
                OnPropertyChanged(nameof(CurrentClassModes));
            }
        }
    }

    public List<ClassUseMode>? CurrentClassModes { get; private set; }
    [ObservableProperty]
    private ClassUseMode? _selectedClassMode;

    [ObservableProperty]
    private bool _useSelectedClass;

    public IAsyncRelayCommand StartAutoAttackCommand { get; }
    public IAsyncRelayCommand StartAutoHuntCommand { get; }
    public IAsyncRelayCommand StopAutoAsyncCommand { get; }
    public IRelayCommand ReloadClassesCommand { get; }

    private void ReloadClasses()
    {
        OnPropertyChanged(nameof(PlayerClasses));

        CurrentClassModes = null;
        SelectedClass = null;
        SelectedClassMode = null;
    }

    private async Task StartAutoHunt()
    {
        if (UseSelectedClass && _selectedClass is not null && _selectedClassMode is not null)
        {
            await Task.Run(() => Auto.StartAutoHunt(_selectedClass, (ClassUseMode)_selectedClassMode));
            return;
        }

        Auto.StartAutoHunt();
    }

    private async Task StartAutoAttack()
    {
        if (UseSelectedClass && _selectedClass is not null && _selectedClassMode is not null)
        {
            await Task.Run(() => Auto.StartAutoAttack(_selectedClass, (ClassUseMode)_selectedClassMode));
            return;
        }

        Auto.StartAutoAttack();
    }
}
