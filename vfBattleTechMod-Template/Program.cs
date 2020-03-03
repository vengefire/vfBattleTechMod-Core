using Harmony;
using vfBattleTechMod_Template.Mod;

namespace vfBattleTechMod_Template
{
    public static class Program
    {
        private static TemplateMod TemplateMod;

        public static void Init(string directory, string settings)
        {
            var harmonyInstance = HarmonyInstance.Create(@"vengefire.template");
            Program.TemplateMod = new TemplateMod(harmonyInstance, directory, settings, nameof(Program.TemplateMod));
        }
    }
}