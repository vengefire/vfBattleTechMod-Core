namespace vfBattleTechMod_Core.Mods.Interfaces
{
    using System.Collections.Generic;

    using Harmony;

    using vfBattleTechMod_Core.Utils.Interfaces;

    public interface IBattleTechMod
    {
        ILogger Logger { get; set; }

        List<IModFeature> ModFeatures { get; set; }

        string Name { get; set; }

        void Initialize(HarmonyInstance harmonyInstance, string settings);
    }
}