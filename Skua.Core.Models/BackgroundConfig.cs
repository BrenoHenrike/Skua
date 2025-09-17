namespace Skua.Core.Models;

public class BackgroundConfig
{
    /// <summary>
    /// The main background SWF file to use. Set to "hideme.swf" when using custom backgrounds.
    /// </summary>
    public string sBG { get; set; } = "Generic2.swf";

    /// <summary>
    /// Custom background file path or URL when using non-default backgrounds.
    /// </summary>
    public string? customBackground { get; set; }
}