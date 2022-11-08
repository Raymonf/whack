using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tommy;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;

namespace WTT
{
    public static class TomlTableExtensions
    {
        private readonly static char[] _multilineChars = { '\n', '"' };

        public static void AddMessage(this TomlTable tomlTable, StructPropertyData entry, int index, string key)
        {
            var value = (entry.Value[index].RawValue as FString)?.Value;
            if (value != null)
                tomlTable.Add(key, new TomlString
                {
                    IsMultiline = value.IndexOfAny(_multilineChars) != -1,
                    Value = value
                });
            else
                tomlTable.Add(key, false);
        }
    }
}
