namespace WTT;

public class ExportSettings
{
    /// <summary>
    /// Export Japanese message.
    /// </summary>
    public bool ExportJapaneseMessage { get; set; } = true;

    /// <summary>
    /// Export English (USA) message.
    /// </summary>
    public bool ExportEnglishUSAMessage { get; set; } = true;

    /// <summary>
    /// Export English (SG) message.
    /// </summary>
    public bool ExportEnglishSGMessage { get; set; } = false;

    /// <summary>
    /// Export Traditional Chinese (TW) message.
    /// </summary>
    public bool ExportTChineseTWMessage { get; set; } = false;

    /// <summary>
    /// Export Traditional Chinese (HK) message.
    /// </summary>
    public bool ExportTChineseHKMessage { get; set; } = false;

    /// <summary>
    /// Export Simplified Chinese message along with the Japanese message.
    /// Useful for cross-referencing meaning of a phrase or word.
    /// </summary>
    public bool ExportSChineseMessage { get; set; } = false;

    /// <summary>
    /// Export Korean message along with the Japanese message.
    /// Useful for cross-referencing meaning of a phrase or word.
    /// </summary>
    public bool ExportKoreanMessage { get; set; } = true;
}