namespace vfBattleTechMod_Core.Mods.Interfaces
{
    using System.Collections.Generic;

    using Harmony;

    using vfBattleTechMod_Core.Utils.Interfaces;

    public interface IModFeature<out TModFeatureSettings>
        where TModFeatureSettings : IModFeatureSettings
    {
        bool Enabled { get; }

        string Name { get; }

        List<IModPatchDirective> PatchDirectives { get; }

        TModFeatureSettings Settings { get; }

        void Initialize(HarmonyInstance harmonyInstance, string settings, ILogger logger, string directory);

        void OnInitializeComplete();
    }
}