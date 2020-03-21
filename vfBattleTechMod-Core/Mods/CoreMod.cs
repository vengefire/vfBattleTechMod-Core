using System.Collections.Generic;
using Harmony;
using vfBattleTechMod_Core.Helpers;
using vfBattleTechMod_Core.Mods.BaseImpl;
using vfBattleTechMod_Core.Mods.Interfaces;
using vfBattleTechMod_Core.Utils.MetaDataHelpers;

namespace vfBattleTechMod_Core.Mods
{
    public class CoreMod : ModBase<CoreModSettings>
    {
        private static CoreMod _coreMod;

        public static CoreMod CoreModSingleton
        {
            get { return _coreMod; }
            set { _coreMod = value; }
        }

        public CoreMod(HarmonyInstance harmonyInstance, string directory, string settings, string name) : base(
            harmonyInstance, directory, settings, name, new List<IModFeature<IModFeatureSettings>>())
        {
            MechAppearanceData =
                MechModel.ProcessAvailabilityFile(ModHelpers.ModResourceFilePath(this.ModSettings.MechAppearanceFile));
            CoreModSingleton = this;
        }

        public List<MechModel> MechAppearanceData { get; set; }
    }
}