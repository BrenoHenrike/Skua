namespace Skua.Core.Interfaces;
public interface ISoundService
{
    /// <summary>
    /// Plays the sound of a beep.
    /// </summary>
    void Beep();
    /// <summary>
    /// Plays the sound of a beep with the desired <paramref name="frequency"/> and <paramref name="duration"/>.
    /// </summary>
    /// <param name="frequency">The frequency of the beep.</param>
    /// <param name="duration">The duration of the beep in milliseconds.</param>
    void Beep(int frequency, int duration);
}
