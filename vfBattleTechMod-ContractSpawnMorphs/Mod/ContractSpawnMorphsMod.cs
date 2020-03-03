using System.Collections.Generic;
using Harmony;
using vfBattleTechMod_ContractSpawnMorphs.Mod.Features.UnitSpawnMorph;
using vfBattleTechMod_Core.Mods.BaseImpl;
using vfBattleTechMod_Core.Mods.Interfaces;

namespace vfBattleTechMod_ContractSpawnMorphs.Mod
{
    public class ContractSpawnMorphMod : ModBase<ContractSpawnMorphModSettings>
    {
        public ContractSpawnMorphMod(HarmonyInstance harmonyInstance, string directory, string settings, string name)
            : base(harmonyInstance, directory, settings, name, ContractSpawnMorphMod.AddModFeatures())
        {
        }

        private static List<IModFeature<IModFeatureSettings>> AddModFeatures()
        {
            return new List<IModFeature<IModFeatureSettings>>
            {
                new UnitSpawnMorphFeature(),
            };
        }
    }
}