using Skua.Core.Models.Skills;

namespace Skua.Core.Interfaces.Skills;
public interface IAdvancedSkillContainer
{
    /// <summary>
    /// The current loaded sets of advanced skills.
    /// </summary>
    List<AdvancedSkill> LoadedSkills { get; set; }

    /// <summary>
    /// Adds an advanced skill to <see cref="LoadedSkills"/>
    /// </summary>
    /// <param name="skill">Skill to save.</param>
    /// <remarks>This doesn't save the modification to the file.</remarks>
    void Add(AdvancedSkill skill);
    /// <summary>
    /// Converts a skill string to an <see cref="AdvancedSkill"/>
    /// </summary>
    /// <param name="skillString">String containing info about the skill sequence.</param>
    /// <returns>An <see cref="AdvancedSkill"/> object made from the string.</returns>
    AdvancedSkill ConvertFromString(string skillString);
    /// <summary>
    /// Reads and loads all the skills from the skills file.
    /// </summary>
    void LoadSkills();
    /// <summary>
    /// Removes an <see cref="AdvancedSkill"/> from the <see cref="LoadedSkills"/>
    /// </summary>
    /// <param name="skill">Skill to remove.</param>
    /// <remarks>This doesn't save the modification to the file.</remarks>
    void Remove(AdvancedSkill skill);
    /// <summary>
    /// Saves all modifications to the skills file.
    /// </summary>
    void Save();
}
