namespace WhackTranslationTool;

public class ExportSettings
{
    /// <summary>
    /// Export Korean message along with the Japanese message.
    /// Useful for cross-referencing meaning of a phrase or word.
    /// </summary>
    public bool ExportKoreanMessage { get; set; } = true;
}