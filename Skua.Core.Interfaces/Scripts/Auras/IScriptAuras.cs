using Skua.Core.Models.Auras;

namespace Skua.Core.Interfaces;
public interface IScriptAuras
{
    /// <summary>
    /// The list of auras.
    /// </summary>
    List<Aura> Auras { get; }

    /// <summary>
    /// Checks if the subtype has active <paramref name="auraName"/>.
    /// </summary>
    /// <param name="auraName">The aura name.</param>
    /// <returns>
    /// <see cref="bool"/> true if subject has the aura; otherwise false
    /// </returns>
    bool HasActiveAura(string auraName);

    /// <summary>
    /// Gets the aura of a subject type with the specified aura name.
    /// </summary>
    /// <param name="auraName">The aura name.</param>
    /// <returns>
    /// <see cref="Aura"/> object
    /// </returns>
    Aura? GetAura(string auraName);
}
