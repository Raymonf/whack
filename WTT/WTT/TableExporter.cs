using Tommy;
using UAssetAPI;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;
using WhackTranslationTool.Exceptions;
using WTT;

namespace WhackTranslationTool;


public class TableExporter
{
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
        
        var asset = new UAsset(filename, Table.UnrealVersion);
        if (!asset.VerifyBinaryEquality())
            throw new UnsupportedAssetException($"'{asset.FilePath}' is not supported (no binary equality)");
        Read(asset);
    }

    private TomlTable MessageDataToTomlTable(StructPropertyData entry)
    {
        var tomlTable = new TomlTable();

        tomlTable.AddMessage(entry, 0, "JapaneseMessage");
        if (_settings.ExportEnglishUSAMessage)
            tomlTable.AddMessage(entry, 1, "EnglishMessageUSA");
        if (_settings.ExportSChineseMessage)
            tomlTable.AddMessage(entry, 5, "SimplifiedChineseMessage");
        if (_settings.ExportKoreanMessage)
            tomlTable.AddMessage(entry, 6, "KoreanMessage");

        return tomlTable;
    }

    private TomlTable CharacterMessageDataToTomlTable(StructPropertyData entry)
    {
        var tomlTable = new TomlTable();

        tomlTable.AddMessage(entry, 3, "JapaneseMessage");
        if (_settings.ExportEnglishUSAMessage)
            tomlTable.AddMessage(entry, 4, "EnglishMessageUSA");
        if (_settings.ExportSChineseMessage)
            tomlTable.AddMessage(entry, 8, "SimplifiedChineseMessage");
        if (_settings.ExportKoreanMessage)
            tomlTable.AddMessage(entry, 9, "KoreanMessage");
        
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