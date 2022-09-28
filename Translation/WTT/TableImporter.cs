using System.Text;
using Tommy;
using UAssetAPI;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;
using WhackTranslationTool.Exceptions;

namespace WhackTranslationTool;

public class TableImporter
{
    private Dictionary<string, string?> strings = new();
    private UAsset asset;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tomlPath">Path to the toml source import file</param>
    /// <param name="assetPath">Path to the target uasset file</param>
    /// <exception cref="UnsupportedAssetException">When UAssetAPI fails to reconstruct the asset file</exception>
    public TableImporter(string tomlPath, string assetPath)
    {
        asset = new UAsset(assetPath, TableConstants.UnrealVersion);
        if (!asset.VerifyBinaryEquality())
        {
            throw new UnsupportedAssetException($"'{asset.FilePath}' is not supported (no binary equality)");
        }
        
        ReadStrings(tomlPath);
        UpdateAssetStrings();
    }

    private void ReadStrings(string tomlPath)
    {
        using var reader = File.OpenText(tomlPath);
        var table = TOML.Parse(reader);
        foreach (var (key, value) in table.RawTable)
        {
            if (string.IsNullOrEmpty(key))
                throw new InvalidDataException($"key '{key}' is invalid");
            if (strings.ContainsKey(key))
                throw new DuplicateKeyException($"duplicate key '{key}' found for file {tomlPath} ({asset.FilePath})");
            strings.Add(key, value.HasKey("JapaneseMessage") ? value["JapaneseMessage"].AsString.Value : null);
        }
    }

    private void UpdateAssetStrings()
    {
        var export = asset.Exports[0] as DataTableExport;
        var table = export?.Table.Data!;

        foreach (var entry in table)
        {
            var name = entry.Name.ToString();
            if (!strings.ContainsKey(name))
            {
                // throw new MissingKeyException($"missing key '{name}' in toml file for {asset.FilePath}");
                Console.WriteLine($"Warning: skipping missing key '{name}' in toml file for {asset.FilePath}");
                continue;
            }

            // index of JapaneseMessage
            var index = entry.StructType.Value.Value switch
            {
                "MessageData" => 0,
                "CharaMessageData" => 3,
                _ => throw new Exception($"Unhandled type '{entry.StructType}'")
            };
            entry.Value[index].SetObject(FString.FromString(strings[name]));
        }
    }

    public void Write(string newAssetPath)
    {
        asset.Write(newAssetPath);
    }
}