using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WTT
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Language
    {
        Japanese,
        EnglishUSA,
        EnglishSG,
        TraditionalChineseTW,
        TraditionalChineseHK,
        SimplifiedChinese,
        Korean
    }
}
