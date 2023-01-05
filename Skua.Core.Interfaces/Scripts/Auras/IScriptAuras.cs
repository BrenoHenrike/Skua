using Skua.Core.Models.Auras;

namespace Skua.Core.Interfaces;
public interface IScriptAuras
{
    IEnumerable<Aura>? GetAuras();
    bool HasActiveAura(string auraName);
    Aura? GetAura(string auraName);
}
