namespace Skua.Core.Interfaces;

public interface IScriptLite
{
    bool CharacterSelectScreen { get; set; }
    bool CustomDropsUI { get; set; }
    bool Debugger { get; set; }
    bool DisableDamageStrobe { get; set; }
    bool DisableMonsterAnimation { get; set; }
    bool DisableRedWarning { get; set; }
    bool DisableSelfAnimation { get; set; }
    bool DisableSkillAnimation { get; set; }
    bool DisableSkillAnimations { get; set; }
    bool DisableWeaponAnimation { get; set; }
    bool FreezeMonsterPosition { get; set; }
    bool HidePlayers { get; set; }
    bool HideUI { get; set; }
    bool InvisibleMonsters { get; set; }
    bool ReacceptQuest { get; set; }
    bool ShowMonsterType { get; set; }
    bool SmoothBackground { get; set; }
    bool UntargetDead { get; set; }
    bool UntargetSelf { get; set; }

    /// <summary>
    /// Gets the current value of an AQLite option (Advanced Options panel).
    /// </summary>
    /// <typeparam name="T">Type of the value to be retrieved.</typeparam>
    /// <param name="optionName">Name of the option to be retrieved.</param>
    /// <returns>The value <typeparamref name="T"/> of the specified option.</returns>
    T? Get<T>(string optionName);

    /// <summary>
    /// Sets the value of an AQLite option (Advanced Options panel) to the specified value.
    /// </summary>
    /// <typeparam name="T">Type of the value that will be setted.</typeparam>
    /// <param name="optionName">Name of the options to be setted.</param>
    /// <param name="value">Value that will be setted to the specified option.</param>
    void Set<T>(string optionName, T value);
}