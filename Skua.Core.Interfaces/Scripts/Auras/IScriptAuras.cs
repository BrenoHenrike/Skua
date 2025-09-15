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

    /// <summary>
    /// Tried to get the aura of a subject type with the specified aura name.
    /// </summary>
    /// <param name="auraName">The aura name.</param>
    /// <param name="aura">Here it returns the aura object if the aura is found.</param>
    /// <returns>
    /// <see cref="bool"/> true if subject has the aura; otherwise false
    /// </returns>
    bool TryGetAura(string auraName, out Aura? aura)
    {
        if (HasActiveAura(auraName))
        {
            aura = GetAura(auraName);
            return true;
        }
        aura = null;
        return false;
    }

    /// <summary>
    /// Gets the stack value/count of a specific aura.
    /// </summary>
    /// <param name="auraName">The aura name.</param>
    /// <returns>
    /// <see cref="int"/> the aura stack value, or 0 if aura is not active
    /// </returns>
    int GetAuraValue(string auraName);

    /// <summary>
    /// Checks if the subject has an aura with at least the specified stack count.
    /// </summary>
    /// <param name="auraName">The aura name.</param>
    /// <param name="minStacks">The minimum stack count.</param>
    /// <returns>
    /// <see cref="bool"/> true if subject has the aura with at least minStacks; otherwise false
    /// </returns>
    bool HasAuraWithMinStacks(string auraName, int minStacks);

    /// <summary>
    /// Gets the remaining seconds until the aura expires.
    /// </summary>
    /// <param name="auraName">The aura name.</param>
    /// <returns>
    /// <see cref="int"/> seconds remaining, or 0 if aura is not active
    /// </returns>
    int GetAuraSecondsRemaining(string auraName);

    /// <summary>
    /// Checks if the subject has any of the specified auras active.
    /// </summary>
    /// <param name="auraNames">Array of aura names to check.</param>
    /// <returns>
    /// <see cref="bool"/> true if subject has any of the auras; otherwise false
    /// </returns>
    bool HasAnyActiveAura(params string[] auraNames);

    /// <summary>
    /// Checks if the subject has all of the specified auras active.
    /// </summary>
    /// <param name="auraNames">Array of aura names to check.</param>
    /// <returns>
    /// <see cref="bool"/> true if subject has all the auras; otherwise false
    /// </returns>
    bool HasAllActiveAuras(params string[] auraNames);

    /// <summary>
    /// Gets the total stack count of all matching auras (useful for similar named auras).
    /// </summary>
    /// <param name="auraNamePattern">The aura name or pattern to match.</param>
    /// <returns>
    /// <see cref="int"/> total stack count of matching auras
    /// </returns>
    int GetTotalAuraStacks(string auraNamePattern);
}