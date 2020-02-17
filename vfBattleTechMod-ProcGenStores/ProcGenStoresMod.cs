namespace vfBattleTechMod_ProcGenStores
{
    using System.Collections.Generic;
    using Harmony;
    using vfBattleTechMod_Core.Mods.Interfaces;
    using vfBattleTechMod_Core.Mods.BaseImpl;

    internal class ProcGenStoresMod : ModBase, IBattleTechMod
    {
        private static List<IModFeature<IModFeatureSettings>> AddModFeatures()
        {
            return new List<IModFeature<IModFeatureSettings>>()
            {
                new ProcGenStoreContentFeature()
            };
        }

        public ProcGenStoresMod(HarmonyInstance harmonyInstance, string directory, string settings, string name) : base(harmonyInstance, directory, settings, name, ProcGenStoresMod.AddModFeatures())
        {
        }
    }
}