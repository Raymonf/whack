using System.Linq;
using UAssetAPI.FieldTypes;
using UAssetAPI.UnrealTypes;

namespace UAssetAPI
{
    /// <summary>
    /// Export data for a <see cref="UProperty"/>.
    /// </summary>
    public class PropertyExport : NormalExport
    {
        public UProperty Property;

        public PropertyExport(Export super) : base(super)
        {

        }

        public PropertyExport(UAsset asset, byte[] extras) : base(asset, extras)
        {

        }

        public PropertyExport()
        {

        }

        public override void Read(AssetBinaryReader reader, int nextStarting)
        {
            base.Read(reader, nextStarting);
            reader.ReadInt32();

            FName exportClassType = this.GetExportClassType();
            Property = MainSerializer.ReadUProperty(reader, exportClassType);
        }

        public override void Write(AssetBinaryWriter writer)
        {
            base.Write(writer);
            writer.Write((int)0);

            MainSerializer.WriteUProperty(Property, writer);
        }
    }
}
