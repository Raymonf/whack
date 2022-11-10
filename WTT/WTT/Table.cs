using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;
using WTT;

namespace WhackTranslationTool;

public class Table
{
    public const UE4Version UnrealVersion = UE4Version.VER_UE4_19;

    public static string GetMessageKeyName(Language language)
    {
        return language switch
        {
            Language.Japanese => "JapaneseMessage",
            Language.EnglishUSA => "EnglishMessageUSA",
            Language.EnglishSG => "EnglishMessageSG",
            Language.TraditionalChineseTW => "TraditionalChineseMessageTW",
            Language.TraditionalChineseHK => "TraditionalChineseMessageHK",
            Language.SimplifiedChinese => "SimplifiedChineseMessage",
            Language.Korean => "KoreanMessage",
            _ => throw new NotImplementedException($"unknown language {language}")
        };
    }

    /// <summary>
    /// Returns the prop
    /// </summary>
    /// <param name="entry"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static int GetMessageIndex(StructPropertyData entry, Language language)
    {
        switch (language)
        {
            case Language.Japanese:
                return entry.StructType.Value.Value switch
                {
                    "MessageData" => 0,
                    "CharaMessageData" => 3,
                    _ => throw new NotImplementedException($"Unhandled type '{entry.StructType}'")
                };
            case Language.EnglishUSA:
                return entry.StructType.Value.Value switch
                {
                    "MessageData" => 1,
                    "CharaMessageData" => 4,
                    _ => throw new NotImplementedException($"Unhandled type '{entry.StructType}'")
                };
            case Language.EnglishSG:
                return entry.StructType.Value.Value switch
                {
                    "MessageData" => 2,
                    "CharaMessageData" => 5,
                    _ => throw new NotImplementedException($"Unhandled type '{entry.StructType}'")
                };
            case Language.TraditionalChineseTW:
                return entry.StructType.Value.Value switch
                {
                    "MessageData" => 3,
                    "CharaMessageData" => 6,
                    _ => throw new NotImplementedException($"Unhandled type '{entry.StructType}'")
                };
            case Language.TraditionalChineseHK:
                return entry.StructType.Value.Value switch
                {
                    "MessageData" => 4,
                    "CharaMessageData" => 7,
                    _ => throw new NotImplementedException($"Unhandled type '{entry.StructType}'")
                };
            case Language.SimplifiedChinese:
                return entry.StructType.Value.Value switch
                {
                    "MessageData" => 5,
                    "CharaMessageData" => 8,
                    _ => throw new NotImplementedException($"Unhandled type '{entry.StructType}'")
                };
            case Language.Korean:
                return entry.StructType.Value.Value switch
                {
                    "MessageData" => 6,
                    "CharaMessageData" => 9,
                    _ => throw new NotImplementedException($"Unhandled type '{entry.StructType}'")
                };
            default:
                throw new NotImplementedException($"unknown language '{language}'");
        };
    }
}