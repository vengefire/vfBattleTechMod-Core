using vfBattleTechMod_Core.Mods.BaseImpl;

namespace vfBattleTechMod_Template.Mod.Features.Template
{
    public class TemplateFeatureSettings : ModFeatureSettingsBase
    {
        public double RarityWeightingDaysDivisor { get; set; } = 30.0;
        public int MaxRarityWeighting { get; set; } = 6;
    }
}