using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace Skua.Core.ViewModels;
public class SkillRulesViewModel : ObservableRecipient
{
    public SkillRulesViewModel()
    {
        ResetUseRulesCommand = new RelayCommand(ResetUseRules);
    }
    private bool _useRuleBool;
    public bool UseRuleBool
    {
        get { return _useRuleBool; }
        set { SetProperty(ref _useRuleBool, value); }
    }
    private bool _healthGreaterThanBool = true;
    public bool HealthGreaterThanBool
    {
        get { return _healthGreaterThanBool; }
        set { SetProperty(ref _healthGreaterThanBool, value); }
    }
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
    private bool _manaGreaterThanBool = true;
    public bool ManaGreaterThanBool
    {
        get { return _manaGreaterThanBool; }
        set { SetProperty(ref _manaGreaterThanBool, value); }
    }
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
    private int _waitUseValue;
    public int WaitUseValue
    {
        get { return _waitUseValue; }
        set { SetProperty(ref _waitUseValue, value); }
    }
    private bool _skipUseBool;
    public bool SkipUseBool
    {
        get { return _skipUseBool; }
        set { SetProperty(ref _skipUseBool, value); }
    }

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
