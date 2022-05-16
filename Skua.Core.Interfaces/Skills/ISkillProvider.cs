namespace Skua.Core.Interfaces;

public interface ISkillProvider
{
    bool ResetOnTarget { get; set; }
    /// <summary>
    /// This method should return true if the bot should attempt to use a skill at the given time.
    /// </summary>
    /// <returns>Whether or not the bot should attempt to use a skill.</returns>
    bool? ShouldUseSkill();
    /// <summary>
    /// This method should return the index of the next skill the bot should try and use. The mode parameter should be set to indicate how the skill should be used.
    /// </summary>
    /// <param name="mode">The mode that the skill should be used in.</param>
    /// <returns>The index of the skill to be used.</returns>
    int GetNextSkill();
    /// <summary>
    /// This method is called when the target is reset/changed.
    /// </summary>
    void OnTargetReset();
    /// <summary>
    /// This method is called when the skill timer is stopped.
    /// </summary>
    void Stop();
    /// <summary>
    /// Loads this skill provider from the given file.
    /// </summary>
    /// <param name="file">The file to load this provider from.</param>
    void Load(string file);
    /// <summary>
    /// Saves this skill provider to the given file.
    /// </summary>
    /// <param name="file">The file to save this provider to.</param>
    void Save(string file);
}
