using System.Collections.Generic;
using Harmony;
using vfBattleTechMod_Core.Mods.BaseImpl;
using vfBattleTechMod_Core.Mods.Interfaces;
using vfBattleTechMod_Template.Mod.Features.Template;

namespace vfBattleTechMod_Template.Mod
{
    public class TemplateMod : ModBase<TemplateModSettings>
    {
        public TemplateMod(HarmonyInstance harmonyInstance, string directory, string settings, string name)
            : base(harmonyInstance, directory, settings, name, AddModFeatures())
        {
        }

        private static List<IModFeature<IModFeatureSettings>> AddModFeatures()
        {
            return new List<IModFeature<IModFeatureSettings>>
            {
                new TemplateFeature(),
            };
        }
    }
}