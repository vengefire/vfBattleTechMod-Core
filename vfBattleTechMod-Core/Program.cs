using Harmony;
using vfBattleTechMod_Core.Mods;

namespace vfBattleTechMod_Core
{
    public static class Program
    {
        private static CoreMod CoreMod { get; set; }
        public static void Init(string directory, string settings)
        {
            var harmonyInstance = HarmonyInstance.Create(@"vengefire.core");
            CoreMod = new CoreMod(harmonyInstance, directory, settings, nameof(CoreMod));
        }
    }
}