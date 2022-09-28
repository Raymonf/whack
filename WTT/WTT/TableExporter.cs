using Tommy;
using UAssetAPI;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;
using WhackTranslationTool.Exceptions;

namespace WhackTranslationTool;

public class TableExporter
{
    private readonly char[] _multilineChars = { '\n', '"' };
    private readonly ExportSettings _settings;
    private Dictionary<string, TomlTable> strings = new();
    
    public TableExporter(string filename) : this(filename, new ExportSettings()) { }

    /// <summary>
    /// Initializes a table exporter object
    /// </summary>
    /// <param name="filename">Name of the uasset source file to export to toml</param>
    /// <param name="settings">Export settings</param>
    /// <exception cref="UnsupportedAssetException">When UAssetAPI fails to reconstruct the asset file</exception>
    public TableExporter(string filename, ExportSettings settings)
    {
        _settings = settings;
        
        var asset = new UAsset(filename, TableConstants.UnrealVersion);
        if (!asset.VerifyBinaryEquality())
            throw new UnsupportedAssetException($"'{asset.FilePath}' is not supported (no binary equality)");
        Read(asset);
    }

    private TomlTable MessageDataToTomlTable(StructPropertyData entry)
    {
        var tomlTable = new TomlTable();
        
        var japaneseValue = (entry.Value[0].RawValue as FString)?.Value;
        if (japaneseValue != null)
            tomlTable.Add("JapaneseMessage", new TomlString
            {
                IsMultiline = japaneseValue.IndexOfAny(_multilineChars) != -1,
                Value = japaneseValue
            });
        
        var koreanValue = (entry.Value[6].RawValue as FString)?.Value;
        if (koreanValue != null)
            tomlTable.Add("KoreanMessage", new TomlString
            {
                IsMultiline = koreanValue.IndexOfAny(_multilineChars) != -1,
                Value = koreanValue
            });

        return tomlTable;
    }

    private TomlTable CharacterMessageDataToTomlTable(StructPropertyData entry)
    {
        var tomlTable = new TomlTable();
        
        var japaneseValue = (entry.Value[3].RawValue as FString)?.Value;
        if (japaneseValue != null)
            tomlTable.Add("JapaneseMessage", new TomlString
            {
                IsMultiline = japaneseValue.IndexOfAny(_multilineChars) != -1,
                Value = japaneseValue
            });
        
        var koreanValue = (entry.Value[9].RawValue as FString)?.Value;
        if (koreanValue != null)
            tomlTable.Add("KoreanMessage", new TomlString
            {
                IsMultiline = koreanValue.IndexOfAny(_multilineChars) != -1,
                Value = koreanValue
            });
        
        return tomlTable;
    }

    private void Read(UAsset asset)
    {
        var export = asset.Exports[0] as DataTableExport;
        var table = export?.Table.Data!;

        foreach (var entry in table)
        {
            var name = entry.Name.ToString();
            if (strings.ContainsKey(name))
                throw new DuplicateKeyException($"duplicate key '{name}' found for file {asset.FilePath}");
            strings.Add(name, entry.StructType.Value.Value switch
            {
                "MessageData" => MessageDataToTomlTable(entry),
                "CharaMessageData" => CharacterMessageDataToTomlTable(entry),
                _ => throw new Exception($"Unhandled type '{entry.StructType}'")
            });
        }
    }

    public void WriteTo(TextWriter writer)
    {
        var toml = new TomlTable();
        foreach (var str in strings)
            toml.Add(str.Key, str.Value);
        toml.WriteTo(writer);
    }
}