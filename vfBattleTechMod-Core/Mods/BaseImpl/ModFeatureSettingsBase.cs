namespace vfBattleTechMod_Core.Mods.BaseImpl
{
    using vfBattleTechMod_Core.Mods.Interfaces;

    public abstract class ModFeatureSettingsBase : ModSettingsBase, IModFeatureSettings
    {
        public bool Enabled { get; set; } = false;
    }
}