using vfBattleTechMod_Core.Mods.Interfaces;

namespace vfBattleTechMod_Core.Mods.BaseImpl
{
    public class ModFeatureSettingsBase : IModFeatureSettings
    {
        private bool enabled = false;

        bool IModFeatureSettings.Enabled => this.enabled;
    }
}