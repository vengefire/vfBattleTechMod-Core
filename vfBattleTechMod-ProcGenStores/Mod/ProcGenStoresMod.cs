using System.Collections.Generic;
using Harmony;
using vfBattleTechMod_Core.Mods.BaseImpl;
using vfBattleTechMod_Core.Mods.Interfaces;
using vfBattleTechMod_ProcGenStores.Mod.Features;
using vfBattleTechMod_ProcGenStores.Mod.Features.ProcGenStoresContent;

namespace vfBattleTechMod_ProcGenStores.Mod
{
    internal class ProcGenStoresMod : ModBase, IBattleTechMod
    {
        public ProcGenStoresMod(HarmonyInstance harmonyInstance, string directory, string settings, string name)
            : base(harmonyInstance, directory, settings, name, ProcGenStoresMod.AddModFeatures())
        {
        }

        private static List<IModFeature<IModFeatureSettings>> AddModFeatures()
        {
            return new List<IModFeature<IModFeatureSettings>> { new ProcGenStoreContentFeature() };
        }
    }
}