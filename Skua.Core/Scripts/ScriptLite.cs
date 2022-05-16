using Skua.Core.Interfaces;
using Skua.Core.PostSharp;

namespace Skua.Core.Scripts;

public class ScriptLite : ScriptableObject, IScriptLite
{
    [ObjectBinding("litePreference.data.bDebugger")]
    public bool Debugger { get; set; }

    [ObjectBinding("litePreference.data.bHideUI")]
    public bool HideUI { get; set; }

    [ObjectBinding("litePreference.data.bMonsType")]
    public bool ShowMonsterType { get; set; }

    [ObjectBinding("litePreference.data.bSmoothBG")]
    public bool SmoothBackground { get; set; }

    [ObjectBinding("litePreference.data.bUntargetSelf")]
    public bool UntargetSelf { get; set; }

    [ObjectBinding("litePreference.data.bUntargetDead")]
    public bool UntargetDead { get; set; }

    [ObjectBinding("litePreference.data.bDisSkillAnim")]
    public bool DisableSkillAnimations { get; set; }

    [ObjectBinding("litePreference.data.bCustomDrops")]
    public bool CustomDropsUI { get; set; }

    [ObjectBinding("litePreference.data.bDisDmgStrobe")]
    public bool DisableDamageStrobe { get; set; }

    [ObjectBinding("litePreference.data.bDisMonAnim")]
    public bool DisableMonsterAnimation { get; set; }

    [ObjectBinding("litePreference.data.bDisSelfMAnim")]
    public bool DisableSelfAnimation { get; set; }

    [ObjectBinding("litePreference.data.bDisSkillAnim")]
    public bool DisableSkillAnimation { get; set; }

    [ObjectBinding("litePreference.data.bDisWepAnim")]
    public bool DisableWeaponAnimation { get; set; }

    [ObjectBinding("litePreference.data.bFreezeMons")]
    public bool FreezeMonsterPosition { get; set; }

    [ObjectBinding("litePreference.data.bHideMons")]
    public bool InvisibleMonsters { get; set; }

    [ObjectBinding("litePreference.data.bHidePlayers")]
    public bool HidePlayers { get; set; }

    [ObjectBinding("litePreference.data.bReaccept")]
    public bool ReacceptQuest { get; set; }

    [ObjectBinding("litePreference.data.bCharSelect")]
    public bool CharacterSelectScreen { get; set; }

    [ObjectBinding("litePreference.data.dOptions[\"disRed\"]")]
    public bool DisableRedWarning { get; set; }

    public T? Get<T>(string optionName)
    {
        return Bot.Flash.GetGameObject<T>($"litePreference.data.{optionName}");
    }

    public void Set<T>(string optionName, T value)
    {
        Bot.Flash.SetGameObject($"litePreference.data.{optionName}", value!);
    }
}
