using System;
using vfBattleTechMod_Core.Mods.BaseImpl;

namespace vfBattleTechMod_ContractSpawnMorphs.Mod.Features.UnitSpawnMorph
{
    public class UnitSpawnMorphFeatureSettings : ModFeatureSettingsBase
    {
        public double RarityWeightingDaysDivisor { get; set; } = 30.0;
        public int MaxRarityWeighting { get; set; } = 6;
    }
}