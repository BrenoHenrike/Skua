using System.ComponentModel;
using Skua.Core.Models.Skills;
using Skua.Core.Utils;

namespace Skua.Core.Interfaces;
public interface IAdvancedSkillContainer : INotifyPropertyChanged
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
    /// Tries to replace a given skill, if not found, adds it to the list.
    /// </summary>
    /// <param name="skill">Skill to save.</param>
    void TryOverride(AdvancedSkill skill);
    /// <summary>
    /// Reads and loads all the skills from the skills file.
    /// </summary>
    void LoadSkills();
    /// <summary>
    /// Resets the skills to the default ones.
    /// </summary>
    void ResetSkillsSets();
    /// <summary>
    /// Saves the current skills to the skills file.
    /// </summary>
    void SyncSkills();
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
