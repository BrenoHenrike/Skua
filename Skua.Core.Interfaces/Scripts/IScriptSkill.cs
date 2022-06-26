using Skua.Core.Models.Skills;

namespace Skua.Core.Interfaces;

public interface IScriptSkill
{
    /// <summary>
    /// The default provider used if no override is set.
    /// </summary>
    ISkillProvider BaseProvider { get; }
    /// <summary>
    /// This provider is always used over any set through SetProvider.
    /// </summary>
    ISkillProvider? OverrideProvider { get; set; }
    /// <summary>
    /// The timeout in multiples of <see cref="SkillInterval"/> milliseconds before skipping the current unavailable skill when using <see cref="SkillUseMode.WaitForCooldown"/>.
    /// </summary>
    int SkillTimeout { get; set; }
    /// <summary>
    /// The interval, in milliseconds, at which to use skills.
    /// </summary>
    int SkillInterval { get; set; }
    /// <summary>
    /// Determines whether the skill thread is currently running.
    /// </summary>
    bool TimerRunning { get; }
    /// <summary>
    /// The way the bot will use skils:
    /// <list type="bullet">
    /// <item><see cref="SkillUseMode.WaitForCooldown"/>
    /// <description>If not set to skip, will wait for the skill to be available and then use it.</description>
    /// </item>
    /// <item><see cref="SkillUseMode.UseIfAvailable"/>
    /// <description>Whenever a skill is available it will use the skill, if not will skip it.</description>
    /// </item>
    /// </list>
    /// </summary>
    SkillUseMode SkillUseMode { get; set; }

    /// <summary>
    /// Checks if the skill with specified <paramref name="index"/> has cooled down.
    /// </summary>
    /// <param name="index">Index of the skill to check.</param>
    /// <returns><see langword="true"/> if the skill is ready to use.</returns>
    bool CanUseSkill(int index);
    /// <summary>
    /// Uses the skill with the specified <paramref name="index"/> (1-4).
    /// </summary>
    /// <param name="index">Index of the skill.</param>
    void UseSkill(int index);
    /// <summary>
    /// Loads the skills from the specified <paramref name="skills"/> string.
    /// </summary>
    /// <param name="skills">String of the skills</param>
    /// <param name="skillTimeout">Timeout in multiples of <see cref="SkillInterval"/> milliseconds before skipping the current unavailable skill when using <see cref="SkillUseMode.WaitForCooldown"/>.</param>
    void LoadAdvanced(string skills, int skillTimeout = -1, SkillUseMode skillMode = SkillUseMode.UseIfAvailable);
    /// <summary>
    /// Loads the skills of the specified <paramref name="className"/> from AdvancedSkills.txt.
    /// </summary>
    /// <param name="className">Name of the class to use</param>
    /// <param name="autoEquip">Whether to equip the class, useful if you want to use multiple skill sets for 1 class</param>
    /// <param name="useMode">Some classes can have different use modes:
    /// <list type="bullet">
    /// <item>
    /// <see cref="ClassUseMode.Base"/>
    /// <description>Default combo</description>
    /// </item>
    /// <item>
    /// <see cref="ClassUseMode.Atk"/>
    /// <description>Full damage combo</description>
    /// </item>
    /// <item>
    /// <see cref="ClassUseMode.Def"/>
    /// <description>Defensive combo</description>
    /// </item>
    /// <item>
    /// <see cref="ClassUseMode.Farm"/>
    /// <description>Farming combo</description>
    /// </item>
    /// <item>
    /// <see cref="ClassUseMode.Solo"/>
    /// <description>Soloing combo</description>
    /// </item>
    /// <item>
    /// <see cref="ClassUseMode.Supp"/>
    /// <description>Support combo</description>
    /// </item>
    /// </list>
    /// </param>
    /// <remarks>If skills from the desired class doesn't exist, generic skills will be used instead.</remarks>
    void LoadAdvanced(string className, bool autoEquip, ClassUseMode useMode = ClassUseMode.Base);
    /// <summary>
    /// Sets the current skill provider (<see cref="OverrideProvider"/>).
    /// </summary>
    /// <param name="provider">Desired provider that implements <see cref="ISkillProvider"/></param>
    void SetProvider(ISkillProvider provider);
    /// <summary>
    /// Loads the skills from the specified <paramref name="skills"/> string and starts the skill thread.
    /// </summary>
    /// <param name="skills">String of the skills</param>
    /// <param name="skillTimeout">Timeout in multiples of <see cref="SkillInterval"/> milliseconds before skipping the current unavailable skill when using <see cref="SkillUseMode.WaitForCooldown"/>.</param>
    void StartAdvanced(string skills, int skillTimeout = -1, SkillUseMode skillMode = SkillUseMode.UseIfAvailable)
    {
        LoadAdvanced(skills, skillTimeout, skillMode);
        Start();
    }
    /// <summary>
    /// Loads the skills of the specified <paramref name="className"/> from AdvancedSkills.txt and starts the skill thread.
    /// </summary>
    /// <param name="className">Name of the class to use</param>
    /// <param name="autoEquip">Whether to equip the class, useful if you want to use multiple skill sets for 1 class</param>
    /// <param name="useMode">Some classes can have different use modes:
    /// <list type="bullet">
    /// <item>
    /// <see cref="ClassUseMode.Base"/>
    /// <description>Default combo</description>
    /// </item>
    /// <item>
    /// <see cref="ClassUseMode.Atk"/>
    /// <description>Full damage combo</description>
    /// </item>
    /// <item>
    /// <see cref="ClassUseMode.Def"/>
    /// <description>Defensive combo</description>
    /// </item>
    /// <item>
    /// <see cref="ClassUseMode.Farm"/>
    /// <description>Farming combo</description>
    /// </item>
    /// <item>
    /// <see cref="ClassUseMode.Solo"/>
    /// <description>Soloing combo</description>
    /// </item>
    /// <item>
    /// <see cref="ClassUseMode.Supp"/>
    /// <description>Support combo</description>
    /// </item>
    /// </list>
    /// </param>
    /// <remarks>If skills from the desired class doesn't exist, generic skills will be used instead.</remarks>
    void StartAdvanced(string className, bool autoEquip, ClassUseMode useMode = ClassUseMode.Base)
    {
        LoadAdvanced(className, autoEquip, useMode);
        Start();
    }
    /// <summary>
    /// Starts the skill thread which uses the registered skills at the set <see cref="SkillInterval"/>.
    /// </summary>
    /// <remarks>The skill thread is automatically stopped when the bot is stopped.</remarks>
    void Start();
    /// <summary>
    /// Stops the skill thread.
    /// </summary>
    void Stop();
    /// <summary>
    /// Sets the current skill provider to the base provider.
    /// </summary>
    void UseBaseProvider();
}
