using Harmony;
using vfBattleTechMod_ProcGenStores.Mod;

namespace vfBattleTechMod_ProcGenStores
{
    public static class Program
    {
        private static ProcGenStoresMod ProcGenStoresMod;

        public static void Init(string directory, string settings)
        {
            var harmonyInstance = HarmonyInstance.Create(@"vengefire.procgenstores");
            ProcGenStoresMod = new ProcGenStoresMod(harmonyInstance, directory, settings, nameof(ProcGenStoresMod));
        }
    }
}