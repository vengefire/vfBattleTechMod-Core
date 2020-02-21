namespace vfBattleTechMod_Core.Mods.BaseImpl
{
    using vfBattleTechMod_Core.Mods.Interfaces;

    public abstract class ModFeatureSettingsBase : ModSettingsBase, IModFeatureSettings
    {
        bool IModFeatureSettings.Enabled { get; } = false;
    }
}