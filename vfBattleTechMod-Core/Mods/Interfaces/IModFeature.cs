namespace vfBattleTechMod_Core.Mods.Interfaces
{
    using Harmony;

    using vfBattleTechMod_Core.Utils.Interfaces;

    public interface IModFeature
    {
        bool Enabled { get; set; }

        string Name { get; set; }

        void Initialize(HarmonyInstance harmonyInstance, string settings, ILogger logger, string directory);
    }
}