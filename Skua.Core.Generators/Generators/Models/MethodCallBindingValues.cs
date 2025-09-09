namespace Skua.Core.Generators;
/// <summary>
/// A model describing the specific values a Method Call Binding Attribute has.
/// </summary>
/// <param name="Path">The path in the game/client swf to get/set the value from.</param>
/// <param name="RunMethodPre">Whether to run the method body before the flash call.</param>
/// <param name="RunMethodPost">Whether to run the method body after the flash call.</param>
/// <param name="GameFunction">Whether it is a function from the game.</param>
/// <param name="ParseFromJson">Whether to deserialize the incoming value with Json.</param>
public record struct MethodCallBindingValues(
    string Path,
    string? Default,
    bool RunMethodPre,
    bool RunMethodPost,
    bool GameFunction,
    bool ParseFromJson);