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
            TemplateMod = new TemplateMod(harmonyInstance, directory, settings, nameof(TemplateMod));
        }
    }
}