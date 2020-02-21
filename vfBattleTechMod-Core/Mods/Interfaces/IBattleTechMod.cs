namespace vfBattleTechMod_Core.Mods.Interfaces
{
    using System.Collections.Generic;

    using vfBattleTechMod_Core.Utils.Interfaces;

    public interface IBattleTechMod<out TModSettings> where TModSettings : IModSettings
    {
        ILogger Logger { get; }

        List<IModFeature<IModFeatureSettings>> ModFeatures { get; }

        string Name { get; }

        string GenerateDefaultModSettings();

        TModSettings ModSettings { get; }
    }
}