using vfBattleTechMod_Core.Mods.BaseImpl;
using vfBattleTechMod_Core.Mods.Interfaces;

namespace vfBattleTechMod_Template.Mod
{
    public class TemplateModSettings : ModSettingsBase, IModSettings
    {
        public string SomeCoolSetting = @"DefaultCodedCoolSetting";
    }
}