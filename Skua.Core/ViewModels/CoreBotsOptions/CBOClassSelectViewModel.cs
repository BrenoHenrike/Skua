using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Skills;
using System.Text;

namespace Skua.Core.ViewModels;
public partial class CBOClassSelectViewModel : ObservableObject, IManageCBOptions
{
    public CBOClassSelectViewModel(IScriptInventory inventory, IAdvancedSkillContainer advancedSkills)
    {
        _inventory = inventory;
        _advancedSkills = advancedSkills;
    }

    public List<string> PlayerClasses { get; private set; } = new();

    private string? _selectedSoloClass;
    public string? SelectedSoloClass
    {
        get { return _selectedSoloClass; }
        set
        {
            if (SetProperty(ref _selectedSoloClass, value) && value is not null)
            {
                SoloUseModes = new();
                SoloUseModes.AddRange(_advancedSkills.LoadedSkills.Where(s => s.ClassName == _selectedSoloClass).Select(s => s.ClassUseMode));
                OnPropertyChanged(nameof(SoloUseModes));
            }
        }
    }
    public List<ClassUseMode> SoloUseModes { get; private set; } = new();
    [ObservableProperty]
    private ClassUseMode? _selectedSoloUseMode;
    [ObservableProperty]
    private bool _useSoloEquipment;


    private string? _selectedFarmClass;
    public string? SelectedFarmClass
    {
        get { return _selectedFarmClass; }
        set
        {
            if (SetProperty(ref _selectedFarmClass, value) && value is not null)
            {
                FarmUseModes = new();
                FarmUseModes.AddRange(_advancedSkills.LoadedSkills.Where(s => s.ClassName == _selectedFarmClass).Select(s => s.ClassUseMode));
                OnPropertyChanged(nameof(FarmUseModes));
            }
        }
    }
    public List<ClassUseMode> FarmUseModes { get; private set; } = new();
    [ObservableProperty]
    private ClassUseMode? _selectedFarmUseMode;
    [ObservableProperty]
    private bool _useFarmEquipment;
    private readonly IScriptInventory _inventory;
    private readonly IAdvancedSkillContainer _advancedSkills;

    [RelayCommand]
    private void ReloadClasses()
    {
        PlayerClasses = _inventory.Items?.Where(i => 
            (i.Category == ItemCategory.Class) && (i.EnhancementLevel > 0)
        ).Select(i => i.Name).ToList() ?? new();
        
        OnPropertyChanged(nameof(PlayerClasses));

        SoloUseModes = new();
        SelectedSoloClass = null;
        SelectedSoloUseMode = null;

        FarmUseModes = new();
        SelectedFarmClass = null;
        SelectedFarmUseMode = null;
    }

    public StringBuilder Save(StringBuilder builder)
    {
        builder.AppendLine($"SoloClassSelect: {SelectedSoloClass}");
        builder.AppendLine($"SoloEquipCheck: {UseSoloEquipment}");
        builder.AppendLine($"SoloModeSelect: {SelectedSoloUseMode}");
        builder.AppendLine($"FarmClassSelect: {SelectedFarmClass}");
        builder.AppendLine($"FarmEquipCheck: {UseFarmEquipment}");
        builder.AppendLine($"FarmModeSelect: {SelectedFarmUseMode}");

        return builder;
    }

    public void SetValues(Dictionary<string, string> values)
    {
        if (values.ContainsKey("SoloClassSelect"))
        {
            PlayerClasses.Add(values["SoloClassSelect"]);
            OnPropertyChanged(nameof(PlayerClasses));
            SelectedSoloClass = values["SoloClassSelect"];
            if(values.TryGetValue("SoloEquipCheck", out string? check))
            {
                UseSoloEquipment = Convert.ToBoolean(check);
            }
            else
                UseSoloEquipment = false;
            if (values.TryGetValue("SoloModeSelect", out string? mode) && !string.IsNullOrWhiteSpace(mode))
            {
                SelectedSoloUseMode = Enum.TryParse(typeof(ClassUseMode), mode, true, out object? result) ? (ClassUseMode)result! : ClassUseMode.Base;
            }
            else
                SelectedSoloUseMode = ClassUseMode.Base;
        }
        else
        {
            SelectedSoloClass = string.Empty;
            UseSoloEquipment = false;
            SelectedSoloUseMode = ClassUseMode.Base;
        }

        if (values.ContainsKey("FarmClassSelect"))
        {
            PlayerClasses.Add(values["FarmClassSelect"]);
            OnPropertyChanged(nameof(PlayerClasses));
            SelectedFarmClass = values["FarmClassSelect"];
            UseFarmEquipment = values.TryGetValue("FarmEquipCheck", out string? check) && Convert.ToBoolean(check);
            SelectedFarmUseMode = values.TryGetValue("FarmModeSelect", out string? mode) && !string.IsNullOrWhiteSpace(mode)
                ? Enum.TryParse(typeof(ClassUseMode), mode, true, out object? result) ? (ClassUseMode)result! : ClassUseMode.Base
                : ClassUseMode.Base;
        }
        else
        {
            SelectedFarmClass = string.Empty;
            UseFarmEquipment = false;
            SelectedFarmUseMode = ClassUseMode.Base;
        }
    }
}
