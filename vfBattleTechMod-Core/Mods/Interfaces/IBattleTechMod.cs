using System.Collections.Generic;
using vfBattleTechMod_Core.Utils.Interfaces;

namespace vfBattleTechMod_Core.Mods.Interfaces
{
    public interface IBattleTechMod
    {
        string Name { get; set; }
        ILogger Logger { get; set; }
        List<IModFeature> ModFeatures { get; set; }
        void Initialize();
    }
}