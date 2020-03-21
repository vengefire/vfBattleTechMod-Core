using vfBattleTechMod_Core.Mods.BaseImpl;
using vfBattleTechMod_Core.Mods.Interfaces;

namespace vfBattleTechMod_Core.Mods
{
    public class CoreModSettings : ModSettingsBase, IModSettings
    {
        public string MechAppearanceFile { get; set; } = "./res/BattleMech_Unit_Listing - canon.csv";
    }
}