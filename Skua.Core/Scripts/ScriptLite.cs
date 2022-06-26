using Skua.Core.Interfaces;
using Skua.Core.Flash;

namespace Skua.Core.Scripts;

public partial class ScriptLite : IScriptLite
{
    private readonly Lazy<IFlashUtil> _lazyFlash;
    private IFlashUtil Flash => _lazyFlash.Value;
    public ScriptLite(Lazy<IFlashUtil> flash)
    {
        _lazyFlash = flash;
    }

    [ObjectBinding("litePreference.data.bDebugger", HasSetter = true)]
    private bool _debugger;

    [ObjectBinding("litePreference.data.bHideUI", HasSetter = true)]
    private bool _hideUI;

    [ObjectBinding("litePreference.data.bMonsType", HasSetter = true)]
    private bool _showMonsterType;

    [ObjectBinding("litePreference.data.bSmoothBG", HasSetter = true)]
    private bool _smoothBackground;

    [ObjectBinding("litePreference.data.bUntargetSelf", HasSetter = true)]
    private bool _untargetSelf;

    [ObjectBinding("litePreference.data.bUntargetDead", HasSetter = true)]
    private bool _untargetDead;

    [ObjectBinding("litePreference.data.bDisSkillAnim", HasSetter = true)]
    private bool _disableSkillAnimations;

    [ObjectBinding("litePreference.data.bCustomDrops", HasSetter = true)]
    private bool _customDropsUI;

    [ObjectBinding("litePreference.data.bDisDmgStrobe", HasSetter = true)]
    private bool _disableDamageStrobe;

    [ObjectBinding("litePreference.data.bDisMonAnim", HasSetter = true)]
    private bool _disableMonsterAnimation;

    [ObjectBinding("litePreference.data.bDisSelfMAnim", HasSetter = true)]
    private bool _disableSelfAnimation;

    [ObjectBinding("litePreference.data.bDisSkillAnim", HasSetter = true)]
    private bool _disableSkillAnimation;

    [ObjectBinding("litePreference.data.bDisWepAnim", HasSetter = true)]
    private bool _disableWeaponAnimation;

    [ObjectBinding("litePreference.data.bFreezeMons", HasSetter = true)]
    private bool _freezeMonsterPosition;

    [ObjectBinding("litePreference.data.bHideMons", HasSetter = true)]
    private bool _invisibleMonsters;

    [ObjectBinding("litePreference.data.bHidePlayers", HasSetter = true)]
    private bool _hidePlayers;

    [ObjectBinding("litePreference.data.bReaccept", HasSetter = true)]
    private bool _reacceptQuest;

    [ObjectBinding("litePreference.data.bCharSelect", HasSetter = true)]
    private bool _characterSelectScreen;

    [ObjectBinding("litePreference.data.dOptions[\"disRed\"]", HasSetter = true)]
    private bool _disableRedWarning;

    public T? Get<T>(string optionName)
    {
        return Flash.GetGameObject<T>($"litePreference.data.{optionName}");
    }

    public void Set<T>(string optionName, T value)
    {
        Flash.SetGameObject($"litePreference.data.{optionName}", value!);
    }
}
