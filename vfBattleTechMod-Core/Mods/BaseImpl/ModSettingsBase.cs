using Newtonsoft.Json;
using vfBattleTechMod_Core.Mods.Interfaces;

namespace vfBattleTechMod_Core.Mods.BaseImpl
{
    public abstract class ModSettingsBase : IModSettingsBase
    {
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}