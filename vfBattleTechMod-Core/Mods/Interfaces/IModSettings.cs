using vfBattleTechMod_Core.Utils.Enums;

namespace vfBattleTechMod_Core.Mods.Interfaces
{
    public interface IModSettings : IModSettingsBase
    {
        LogLevel LogLevel { get; set; }
    }
}