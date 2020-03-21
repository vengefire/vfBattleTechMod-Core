using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Data;
using vfBattleTechMod_Core.Mods.BaseImpl;
using vfBattleTechMod_Core.Mods.Interfaces;

namespace vfBattleTechMod_Template.Mod.Features.Template
{
    public class TemplateFeature : ModFeatureBase<TemplateFeatureSettings>
    {
        private new static TemplateFeature Myself;

        public TemplateFeature()
            : base(GetPatchDirectives)
        {
            Myself = this;
        }

        public static List<IModPatchDirective> GetPatchDirectives =>
            new List<IModPatchDirective>
            {
                new ModPatchDirective(
                    typeof(MetadataDatabase).GetMethod("GetMatchingUnitDefs"),
                    typeof(TemplateFeature).GetMethod("MetadataDatabase_GetMatchingUnitDefs_Prefix"),
                    typeof(TemplateFeature).GetMethod("MetadataDatabase_GetMatchingUnitDefs_Postfix"),
                    null,
                    Harmony.Priority.Normal)
            };

        public override string Name => "Template Mod Feature";

        public static bool MetadataDatabase_GetMatchingUnitDefs_Prefix(MetadataDatabase __instance)
        {
            return true;
        }
        
        public static void MetadataDatabase_GetMatchingUnitDefs_Postfix(MetadataDatabase __instance, DateTime? currentDate, ref List<UnitDef_MDD> __result)
        {
        }

        protected override bool ValidateSettings()
        {
            return true;
        }

        public override void OnInitializeComplete()
        {
        }
    }
}