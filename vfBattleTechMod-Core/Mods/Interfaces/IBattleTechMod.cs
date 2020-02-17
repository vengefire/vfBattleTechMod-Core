namespace vfBattleTechMod_Core.Mods.Interfaces
{
    using System.Collections.Generic;

    using vfBattleTechMod_Core.Utils.Interfaces;

    public interface IBattleTechMod
    {
        ILogger Logger { get; }

        List<IModFeature<IModFeatureSettings>> ModFeatures { get; }

        string Name { get; }
    }
}