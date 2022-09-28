using System.Linq;
using UAssetAPI.FieldTypes;
using UAssetAPI.UnrealTypes;

namespace UAssetAPI
{
    /// <summary>
    /// Export data for a <see cref="UField"/>.
    /// </summary>
    public class FieldExport : NormalExport
    {
        public UField Field;

        public FieldExport(Export super) : base(super)
        {

        }

        public FieldExport(UAsset asset, byte[] extras) : base(asset, extras)
        {

        }

        public FieldExport()
        {

        }

        public override void Read(AssetBinaryReader reader, int nextStarting)
        {
            base.Read(reader, nextStarting);
            reader.ReadInt32();

            Field = new UField();
            Field.Read(reader);
        }

        public override void Write(AssetBinaryWriter writer)
        {
            base.Write(writer);
            writer.Write((int)0);

            Field.Write(writer);
        }
    }
}
