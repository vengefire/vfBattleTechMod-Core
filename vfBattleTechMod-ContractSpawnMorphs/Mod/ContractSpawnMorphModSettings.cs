using vfBattleTechMod_Core.Mods.BaseImpl;
using vfBattleTechMod_Core.Mods.Interfaces;
using vfBattleTechMod_Core.Utils.Enums;

namespace vfBattleTechMod_ContractSpawnMorphs.Mod
{
    public class ContractSpawnMorphModSettings : ModSettingsBase, IModSettings
    {
        public string SomeCoolSetting = @"DefaultCodedCoolSetting";
        public LogLevel LogLevel { get; set; } = LogLevel.Debug;
    }
}