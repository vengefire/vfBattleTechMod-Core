using System.Collections.Generic;
using Harmony;
using vfBattleTechMod_Core.Mods.BaseImpl;
using vfBattleTechMod_Core.Mods.Interfaces;

namespace vfBattleTechMod_Core.Mods
{
    public class CoreMod : ModBase<CoreModSettings>
    {
        public CoreMod(HarmonyInstance harmonyInstance, string directory, string settings, string name) : base(harmonyInstance, directory, settings, name, new List<IModFeature<IModFeatureSettings>>())
        {
        }
    }
}