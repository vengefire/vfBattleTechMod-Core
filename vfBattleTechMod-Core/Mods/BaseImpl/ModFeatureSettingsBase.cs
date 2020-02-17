namespace vfBattleTechMod_Core.Mods.BaseImpl
{
    using vfBattleTechMod_Core.Mods.Interfaces;

    public class ModFeatureSettingsBase : IModFeatureSettings
    {
        bool IModFeatureSettings.Enabled { get; } = false;
    }
}