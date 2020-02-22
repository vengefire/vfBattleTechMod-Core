using vfBattleTechMod_Core.Mods.Interfaces;

namespace vfBattleTechMod_Core.Mods.BaseImpl
{
    public abstract class ModFeatureSettingsBase : ModSettingsBase, IModFeatureSettings
    {
        public bool Enabled { get; set; } = false;
    }
}