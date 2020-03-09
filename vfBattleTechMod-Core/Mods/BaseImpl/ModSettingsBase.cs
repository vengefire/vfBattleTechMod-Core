using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using vfBattleTechMod_Core.Mods.Interfaces;
using vfBattleTechMod_Core.Utils.Enums;

namespace vfBattleTechMod_Core.Mods.BaseImpl
{
    public abstract class ModSettingsBase : IModSettingsBase
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public LogLevel LogLevel { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}