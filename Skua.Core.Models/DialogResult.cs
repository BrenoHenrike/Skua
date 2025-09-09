namespace Skua.Core.Models;
/// <summary>
/// An object with the button text and index from the shown dialog.
/// </summary>
/// <param name="Text">Text of the button that was clicked.</param>
/// <param name="Value">Value of the button that was clicked.</param>
public record DialogResult(string Text, int Value)
{
    /// <summary>
    /// Default <see cref="DialogResult"/> when the dialog is closed or cancelled.
    /// </summary>
    public static DialogResult Cancelled { get; } = new("Cancel", -1);
}