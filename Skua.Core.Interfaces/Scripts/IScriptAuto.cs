using Skua.Core.Models.Skills;
using System.ComponentModel;

namespace Skua.Core.Interfaces;

public interface IScriptAuto : INotifyPropertyChanged
{
    /// <summary>
    /// Whether the Auto Attack/Hunt is running.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Starts the Auto Attack function.
    /// </summary>
    /// <param name="className">The name of the class to use.</param>
    /// <param name="classUseMode">The <see cref="ClassUseMode"/> to use the class.</param>
    void StartAutoAttack(string? className = null, ClassUseMode classUseMode = ClassUseMode.Base);

    /// <summary>
    /// Starts the Auto Hunt function. The player will hunt the current target or all the monsters in the current room throughout the map.
    /// </summary>
    /// <param name="className">The name of the class to use.</param>
    /// <param name="classUseMode">The <see cref="ClassUseMode"/> to use the class.</param>
    void StartAutoHunt(string? className = null, ClassUseMode classUseMode = ClassUseMode.Base);

    /// <summary>
    /// Stops the Auto Attack/Hunt.
    /// </summary>
    void Stop();

    /// <summary>
    /// Stops the Auto Attack/Hunt asynchronously, use for UI elements.
    /// </summary>
    ValueTask StopAsync();
}