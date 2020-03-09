using vfBattleTechMod_Core.Utils.Enums;

namespace vfBattleTechMod_Core.Mods.Interfaces
{
    public interface IModSettingsBase
    {
        LogLevel LogLevel { get; set; }
        string Serialize();
    }
}