using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace Skua.Core.ViewModels;
public partial class SkillRulesViewModel : ObservableRecipient
{
    public SkillRulesViewModel()
    {
        ResetUseRulesCommand = new RelayCommand(ResetUseRules);
    }
    public SkillRulesViewModel(SkillRulesViewModel rules)
    {
        _useRuleBool = rules.UseRuleBool;
        _waitUseValue = rules.WaitUseValue;
        _healthGreaterThanBool = rules.HealthGreaterThanBool;
        _healthUseValue = rules.HealthUseValue;
        _manaGreaterThanBool = rules.ManaGreaterThanBool;
        _manaUseValue = rules.ManaUseValue;
        _skipUseBool = rules.SkipUseBool;

        ResetUseRulesCommand = new RelayCommand(ResetUseRules);
    }
    [ObservableProperty]
    private bool _useRuleBool;
    [ObservableProperty]
    private bool _healthGreaterThanBool = true;
    private int _healthUseValue;
    public int HealthUseValue
    {
        get { return _healthUseValue; }
        set
        {
            if (value is < 0 or > 100)
                return;
            SetProperty(ref _healthUseValue, value);
        }
    }
    [ObservableProperty]
    private bool _manaGreaterThanBool = true;
    private int _manaUseValue;
    public int ManaUseValue
    {
        get { return _manaUseValue; }
        set
        {
            if (value is < 0 or > 100)
                return;
            SetProperty(ref _manaUseValue, value);
        }
    }
    [ObservableProperty]
    private int _waitUseValue;
    [ObservableProperty]
    private bool _skipUseBool;

    public IRelayCommand ResetUseRulesCommand { get; }

    private void ResetUseRules()
    {
        UseRuleBool = false;
        HealthGreaterThanBool = true;
        HealthUseValue = 0;
        ManaGreaterThanBool = true;
        ManaUseValue = 0;
        WaitUseValue = 0;
        SkipUseBool = false;
    }
}
