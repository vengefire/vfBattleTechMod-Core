using System.IO;
using Harmony;
using NUnit.Framework;
using vfBattleTechMod_ProcGenStores.Mod;

namespace vfBattleTechMod_ProcGenStores_Test
{
    [TestFixture]
    public class ProcGenStoreModTests
    {
        [Test]
        public void TestModDefaultSettingsGeneration()
        {
            var settings = File.ReadAllText(TestContext.CurrentContext.TestDirectory + @"/res/test-settings.json");
            var mod = new ProcGenStoresMod(HarmonyInstance.Create("vf-test"), Directory.GetCurrentDirectory(), settings,
                nameof(ProcGenStoresMod));
            var default_settings = mod.GenerateDefaultModSettings();
        }
    }
}