namespace Skua.Core.Models;

/// <summary>
/// An enum describing hunting priority behaviour.
/// </summary>
[Flags]
public enum HuntPriorities
{
    /// <summary>
    /// No priority.
    /// </summary>
    None = 0,

    /// <summary>
    /// Prioritises monsters with the lowest HP.
    /// </summary>
    LowHP = 1,

    /// <summary>
    /// Prioritises monsters with the highest HP.
    /// </summary>
    HighHP = 2,

    /// <summary>
    /// Prioritises monsters which are in the same cell.
    /// </summary>
    Close = 4
}