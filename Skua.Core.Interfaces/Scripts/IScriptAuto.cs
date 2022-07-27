using Skua.Core.Models.Skills;
using System.ComponentModel;

namespace Skua.Core.Interfaces;
public interface IScriptAuto : INotifyPropertyChanged
{
    bool IsRunning { get; }

    void StartAutoAttack(string? className = null, ClassUseMode classUseMode = ClassUseMode.Base);
    void StartAutoHunt(string? className = null, ClassUseMode classUseMode = ClassUseMode.Base);
    void Stop();
    ValueTask StopAsync();
}
