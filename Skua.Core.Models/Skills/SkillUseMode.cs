namespace Skua.Core.Models.Skills;
public enum SkillUseMode
{
    /// <summary>
    /// Assumes the skill is used when it should be.
    /// </summary>
    UseIfAvailable,
    /// <summary>
    /// Waits for the skill to be available before using it.
    /// </summary>
    WaitForCooldown
}
