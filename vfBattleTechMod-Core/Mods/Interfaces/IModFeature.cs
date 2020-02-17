using System.Collections.Generic;

namespace vfBattleTechMod_Core.Mods.Interfaces
{
    using Harmony;
    using vfBattleTechMod_Core.Utils.Interfaces;

    public interface IModFeature<out TModFeatureSettings> where TModFeatureSettings : IModFeatureSettings
    {
        bool Enabled { get; }

        string Name { get; }

        void Initialize(HarmonyInstance harmonyInstance, string settings, ILogger logger, string directory);

        TModFeatureSettings Settings { get; }

        void OnInitializeComplete();

        List<IModPatchDirective> PatchDirectives { get; }
    }
}