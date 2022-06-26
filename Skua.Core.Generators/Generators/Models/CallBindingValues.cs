namespace Skua.Core.Generators.Models;
/// <summary>
/// A model describing the specific values a CallBinding Attribute has.
/// </summary>
/// <param name="Path">The path to the game/SWF value.</param>
/// <param name="UseValue">Whether to use the incoming value to set the game value.</param>
/// <param name="Get">Whether it should make a Flash Call to get the value.</param>
/// <param name="Set">Whether it should make a Flash Call to set the value.</param>
public record struct CallBindingValues(
    string Path,
    string? Default,
    bool UseValue,
    bool Get,
    bool Set,
    bool HasSetter);
