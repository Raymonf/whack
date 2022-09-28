using Newtonsoft.Json;
using System.Linq;
using UAssetAPI.UnrealTypes;

namespace UAssetAPI
{
    /// <summary>
    /// A string table. Holds Key->SourceString pairs of text.
    /// </summary>
    public class FStringTable : TMap<FString, FString>
    {
        [JsonProperty]
        public FString TableNamespace;

        public FStringTable(FString tableNamespace) : base()
        {
            TableNamespace = tableNamespace;
        }

        public FStringTable() : base()
        {

        }
    }

    /// <summary>
    /// Export data for a string table. See <see cref="FStringTable"/>.
    /// </summary>
    public class StringTableExport : NormalExport
    {
        [JsonProperty]
        public FStringTable Table;

        public StringTableExport(Export super) : base(super)
        {

        }

        public StringTableExport(FStringTable data, UAsset asset, byte[] extras) : base(asset, extras)
        {
            Table = data;
        }

        public StringTableExport()
        {

        }

        public override void Read(AssetBinaryReader reader, int nextStarting)
        {
            base.Read(reader, nextStarting);
            reader.ReadInt32();

            Table = new FStringTable(reader.ReadFString());

            int numEntries = reader.ReadInt32();
            for (int i = 0; i < numEntries; i++)
            {
                Table.Add(reader.ReadFString(), reader.ReadFString());
            }
        }

        public override void Write(AssetBinaryWriter writer)
        {
            base.Write(writer);
            writer.Write((int)0);

            writer.Write(Table.TableNamespace);
            writer.Write(Table.Count);
            for (int i = 0; i < Table.Count; i++)
            {
                writer.Write(Table.Keys.ElementAt(i));
                writer.Write(Table[i]);
            }
        }
    }
}
