using vfBattleTechMod_Core.Mods.BaseImpl;
using vfBattleTechMod_Core.Mods.Interfaces;
using vfBattleTechMod_Core.Utils.Enums;

namespace vfBattleTechMod_Template.Mod
{
    public class TemplateModSettings : ModSettingsBase, IModSettings
    {
        public string SomeCoolSetting = @"DefaultCodedCoolSetting";
    }
}