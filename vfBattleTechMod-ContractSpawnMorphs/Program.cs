using Harmony;
using vfBattleTechMod_ContractSpawnMorphs.Mod;

namespace vfBattleTechMod_ContractSpawnMorphs
{
    public static class Program
    {
        private static ContractSpawnMorphMod ContractSpawnMorphMod;

        public static void Init(string directory, string settings)
        {
            var harmonyInstance = HarmonyInstance.Create(@"vengefire.contract-spawn-morphs");
            Program.ContractSpawnMorphMod = new ContractSpawnMorphMod(harmonyInstance, directory, settings, nameof(Program.ContractSpawnMorphMod));
        }
    }
}