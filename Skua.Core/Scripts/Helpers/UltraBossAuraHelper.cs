using Skua.Core.Interfaces.Auras;
using Skua.Core.Models.Auras;
using Skua.Core.Models.Monsters;

namespace Skua.Core.Scripts.Helpers;

/// <summary>
/// Helper class for flexible aura analysis and boss mechanics.
/// </summary>
public static class UltraBossAuraHelper
{
    /// <summary>
    /// Analyzes charge mechanics based on specified charge aura names.
    /// </summary>
    /// <param name="auras">Auras to check (self or target)</param>
    /// <param name="positiveChargeName">Name of positive charge aura</param>
    /// <param name="negativeChargeName">Name of negative charge aura</param>
    /// <param name="reversedSuffix">Suffix that indicates reversed charges (e.g., "?")</param>
    /// <returns>Tuple indicating charge states</returns>
    public static (bool hasPositive, bool hasNegative, bool hasReversed) AnalyzeChargeMechanics(
        IScriptSelfAuras auras,
        string positiveChargeName,
        string negativeChargeName,
        string? reversedSuffix = null)
    {
        bool positiveCharge = auras.HasActiveAura(positiveChargeName);
        bool negativeCharge = auras.HasActiveAura(negativeChargeName);
        bool hasReversed = false;

        if (!string.IsNullOrEmpty(reversedSuffix))
        {
            hasReversed = auras.HasAnyActiveAura(
                positiveChargeName + reversedSuffix,
                negativeChargeName + reversedSuffix);
        }

        return (positiveCharge, negativeCharge, hasReversed);
    }

    /// <summary>
    /// Checks if a specific aura value meets a threshold condition.
    /// </summary>
    /// <param name="auras">Auras interface to check</param>
    /// <param name="auraName">Name of the aura to check</param>
    /// <param name="threshold">Threshold value</param>
    /// <param name="comparison">Comparison type ("&lt;", "&gt;", "&lt;=", "&gt;=", "==", "!=")</param>
    /// <returns>True if condition is met</returns>
    public static bool CheckAuraThreshold(IScriptSelfAuras auras, string auraName, int threshold, string comparison = ">=")
    {
        int value = auras.GetAuraValue(auraName);
        return comparison switch
        {
            "<" => value < threshold,
            ">" => value > threshold,
            "<=" => value <= threshold,
            ">=" => value >= threshold,
            "==" => value == threshold,
            "!=" => value != threshold,
            _ => throw new ArgumentException($"Invalid comparison operator: {comparison}")
        };
    }

    /// <summary>
    /// Gets all auras matching a pattern from a list of auras (case-insensitive).
    /// </summary>
    /// <param name="auras">List of auras to search</param>
    /// <param name="namePattern">Pattern to match (supports wildcards)</param>
    /// <returns>List of matching auras</returns>
    public static List<Aura> GetAurasMatchingPattern(List<Aura> auras, string namePattern)
    {
        if (namePattern.Contains('*'))
        {
            var regex = new System.Text.RegularExpressions.Regex(
                "^" + namePattern.Replace("*", ".*") + "$",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return auras.Where(a => regex.IsMatch(a.Name)).ToList();
        }
        else
        {
            return auras.Where(a => a.Name.Equals(namePattern, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }

    /// <summary>
    /// Gets monsters with specific aura from a list of monsters (case-insensitive).
    /// </summary>
    /// <param name="monsters">Monsters to search through</param>
    /// <param name="auraName">Name of aura to search for</param>
    /// <returns>List of monsters that have the specified aura</returns>
    public static List<Monster> GetMonstersWithAura(IEnumerable<Monster> monsters, string auraName)
    {
        return monsters.Where(m => m.HasAura(auraName)).ToList();
    }

    /// <summary>
    /// Gets monsters with any of the specified auras (case-insensitive).
    /// </summary>
    /// <param name="monsters">Monsters to search through</param>
    /// <param name="auraNames">Array of aura names to search for</param>
    /// <returns>List of monsters that have any of the specified auras</returns>
    public static List<Monster> GetMonstersWithAnyAura(IEnumerable<Monster> monsters, params string[] auraNames)
    {
        return monsters.Where(m => auraNames.Any(aura => m.HasAura(aura))).ToList();
    }

    /// <summary>
    /// Gets monsters with all of the specified auras (case-insensitive).
    /// </summary>
    /// <param name="monsters">Monsters to search through</param>
    /// <param name="auraNames">Array of aura names that must all be present</param>
    /// <returns>List of monsters that have all specified auras</returns>
    public static List<Monster> GetMonstersWithAllAuras(IEnumerable<Monster> monsters, params string[] auraNames)
    {
        return monsters.Where(m => auraNames.All(aura => m.HasAura(aura))).ToList();
    }

    /// <summary>
    /// Checks if all aura conditions are met for skill usage (case-insensitive).
    /// </summary>
    /// <param name="selfAuras">Self auras interface to check</param>
    /// <param name="conditions">Dictionary of aura names and their required conditions</param>
    /// <returns>True if all conditions are met</returns>
    public static bool ShouldUseSkill(IScriptSelfAuras selfAuras, Dictionary<string, Func<int, bool>> conditions)
    {
        return conditions.All(condition =>
        {
            int auraValue = selfAuras.GetAuraValue(condition.Key);
            return condition.Value(auraValue);
        });
    }

    /// <summary>
    /// Checks if all aura conditions are met for skill usage using target auras (case-insensitive).
    /// </summary>
    /// <param name="targetAuras">Target auras interface to check</param>
    /// <param name="conditions">Dictionary of aura names and their required conditions</param>
    /// <returns>True if all conditions are met</returns>
    public static bool ShouldUseSkill(IScriptTargetAuras targetAuras, Dictionary<string, Func<int, bool>> conditions)
    {
        return conditions.All(condition =>
        {
            int auraValue = targetAuras.GetAuraValue(condition.Key);
            return condition.Value(auraValue);
        });
    }

    /// <summary>
    /// Gets a summary of all auras present on a list of monsters.
    /// </summary>
    /// <param name="monsters">Monsters to analyze</param>
    /// <returns>Dictionary with aura names as keys and count of monsters with that aura as values</returns>
    public static Dictionary<string, int> GetAuraSummary(IEnumerable<Monster> monsters)
    {
        var auraSummary = new Dictionary<string, int>();

        foreach (var monster in monsters)
        {
            if (monster.Auras != null)
            {
                foreach (var aura in monster.Auras)
                {
                    if (auraSummary.ContainsKey(aura.Name))
                        auraSummary[aura.Name]++;
                    else
                        auraSummary[aura.Name] = 1;
                }
            }
        }

        return auraSummary;
    }
}