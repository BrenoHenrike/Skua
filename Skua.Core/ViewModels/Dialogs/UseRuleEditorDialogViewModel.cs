namespace Skua.Core.ViewModels;
public class UseRuleEditorDialogViewModel : DialogViewModelBase
{
    public UseRuleEditorDialogViewModel(SkillRulesViewModel useRules)
        : base("Edit Rules")
    {
        UseRules = useRules;
    }

    public SkillRulesViewModel UseRules { get; }
}
