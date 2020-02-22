using System.Collections.Generic;
using Harmony;
using vfBattleTechMod_Core.Mods.BaseImpl;
using vfBattleTechMod_Core.Mods.Interfaces;
using vfBattleTechMod_ProcGenStores.Mod.Features.ProcGenStoresContent;

namespace vfBattleTechMod_ProcGenStores.Mod
{
    public class ProcGenStoresMod : ModBase<ProcGenStoresModSettings>
    {
        public ProcGenStoresMod(HarmonyInstance harmonyInstance, string directory, string settings, string name)
            : base(harmonyInstance, directory, settings, name, AddModFeatures())
        {
        }

        private static List<IModFeature<IModFeatureSettings>> AddModFeatures()
        {
            return new List<IModFeature<IModFeatureSettings>> {new ProcGenStoreContentFeature()};
        }
    }
}