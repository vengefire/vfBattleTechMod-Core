namespace vfBattleTechMod_Core.Mods.Interfaces
{
    public interface IModFeatureSettings : IModSettingsBase
    {
        bool Enabled { get; set; }
    }
}