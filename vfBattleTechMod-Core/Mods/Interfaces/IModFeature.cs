namespace vfBattleTechMod_Core.Mods.Interfaces
{
    public interface IModFeature
    {
        string Name { get; set; }
        bool Enabled { get; set; }
        void Initialize();
    }
}