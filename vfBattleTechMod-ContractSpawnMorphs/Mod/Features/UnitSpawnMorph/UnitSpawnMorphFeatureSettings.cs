using System;
using vfBattleTechMod_Core.Mods.BaseImpl;

namespace vfBattleTechMod_ContractSpawnMorphs.Mod.Features.UnitSpawnMorph
{
    public class UnitSpawnMorphFeatureSettings : ModFeatureSettingsBase
    {
        public double RarityWeightingDaysDivisor { get; set; } = 30.0;
        public int MaxRarityWeighting { get; set; } = 6;

        public DateTime CompressionFactorControlDate { get; set; } =
            new DateTime(3028, 4, 28); // Helm Memory Core Recovery

        public DateTime CompressionFactorTargetDate { get; set; } =
            new DateTime(3026, 1, 1); // One year from standard game start
    }
}