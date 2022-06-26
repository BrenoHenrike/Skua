namespace Skua.Core.Utils;
[Flags]
public enum DecamelizeTextOptions
{
    /// <summary>
    /// No option is defined.
    /// </summary>
    None = 0,

    /// <summary>
    /// First character will be upper case.
    /// </summary>
    ForceFirstUpper = 1,

    /// <summary>
    /// Characters beyond the first will be lower case.
    /// </summary>
    ForceRestLower = 2,

    /// <summary>
    /// Unescape unicode encoding (format is \u0000).
    /// </summary>
    UnescapeUnicode = 4,

    /// <summary>
    /// Unescape hexadecimal encoding (format is \x0000).
    /// </summary>
    UnescapeHexadecimal = 8,

    /// <summary>
    /// Replaces spaces by underscore.
    /// </summary>
    ReplaceSpacesByUnderscore = 0x10,

    /// <summary>
    /// Replaces spaces by minus.
    /// </summary>
    ReplaceSpacesByMinus = 0x20,

    /// <summary>
    /// Replaces spaces by dot.
    /// </summary>
    ReplaceSpacesByDot = 0x40,

    /// <summary>
    /// Keep first underscores sticked as is.
    /// </summary>
    KeepFirstUnderscores = 0x80,

    /// <summary>
    /// Numbers are not considered as separators.
    /// </summary>
    DontDecamelizeNumbers = 0x100,

    /// <summary>
    /// Keep indices used by the String.Format method.
    /// </summary>
    KeepFormattingIndices = 0x200,

    /// <summary>
    /// Defines the default options.
    /// </summary>
    Default = ForceFirstUpper | UnescapeUnicode | UnescapeHexadecimal | KeepFirstUnderscores,
}
