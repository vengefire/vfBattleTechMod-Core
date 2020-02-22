using System.Collections.Generic;
using vfBattleTechMod_Core.Utils.Interfaces;

namespace vfBattleTechMod_Core.Mods.Interfaces
{
    public interface IBattleTechMod<out TModSettings> where TModSettings : IModSettings
    {
        ILogger Logger { get; }

        List<IModFeature<IModFeatureSettings>> ModFeatures { get; }

        string Name { get; }

        TModSettings ModSettings { get; }

        string GenerateDefaultModSettings();
    }
}