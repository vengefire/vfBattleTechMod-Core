using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Data;
using vfBattleTechMod_Core.Mods.BaseImpl;
using vfBattleTechMod_Core.Mods.Interfaces;

namespace vfBattleTechMod_ContractSpawnMorphs.Mod.Features.UnitSpawnMorph
{
    public class UnitSpawnMorphFeature : ModFeatureBase<UnitSpawnMorphFeatureSettings>
    {
        private new static UnitSpawnMorphFeature Myself;

        public UnitSpawnMorphFeature()
            : base(UnitSpawnMorphFeature.GetPatchDirectives)
        {
            UnitSpawnMorphFeature.Myself = this;
        }

        public static List<IModPatchDirective> GetPatchDirectives =>
            new List<IModPatchDirective>
            {
                new ModPatchDirective(
                    typeof(MetadataDatabase).GetMethod("GetMatchingUnitDefs"),
                    null,
                    typeof(UnitSpawnMorphFeature).GetMethod("MetadataDatabase_GetMatchingUnitDefs_Postfix"),
                    null,
                    0)
            };

        public override string Name => "Procedurally Morph Contract Unit Spawns";

        public static void MetadataDatabase_GetMatchingUnitDefs_Postfix(MetadataDatabase __instance, DateTime? currentDate, ref List<UnitDef_MDD> __result)
        {
            // Alias the keywords for readability...
            var mdd = __instance;
            var matchingDataByTagSet = __result;

            var simGameState = UnityGameInstance.BattleTechGame.Simulation;
            var filteredList = new List<UnitDef_MDD>();

            // Group matching unitDef_MDD records by their associated prefabIdentifier
            var unitsGroupedByPrefab = matchingDataByTagSet
                .Select(defMdd => new {unitDefMdd = defMdd, mechDef = simGameState.DataManager.MechDefs.First(pair => pair.Key == defMdd.UnitDefID).Value})
                .GroupBy(arg => arg.mechDef.Chassis.PrefabIdentifier, arg => arg, (s, enumerable) => new {Base = s, Units = enumerable}).ToList();

            // var prefabVariantsOccuringOnce = unitsGroupedByPrefab.Where(arg => arg.Units.Count() == 1).SelectMany(arg => arg.Units).ToList();
            foreach (var prefabGroup in unitsGroupedByPrefab)
            {
                var prefabSelectionList = new List<UnitDef_MDD>();
                foreach (var unit in prefabGroup.Units)
                {
                    // For mechs with an appearance date (and current date is set, which it should always be)
                    // 1. Each entry gets a rarityWeighting + 1 per 30 days since appearance date has passed
                    // 2. To a maximum of 6 (so mechs that appeared at least 180 days prior to the current date receive the maximum rarity weighting)
                    // These following two variables ought to be set via [Settings] in the mod.json...
                    var rarityWeighting = UnitSpawnMorphFeature.Myself.Settings.MaxRarityWeighting;
                    if (currentDate != null && unit.mechDef.MinAppearanceDate != null)
                    {
                        // Could do this in only one statement, but that ended up being a little kludgy and hard to read...
                        var roundedDays = Math.Min(1, Math.Round(((currentDate - unit.mechDef.MinAppearanceDate).Value.TotalDays + 1) / UnitSpawnMorphFeature.Myself.Settings.RarityWeightingDaysDivisor, 0));
                        var rawRarity = Convert.ToInt32(roundedDays);
                        rarityWeighting = Math.Min(rawRarity, UnitSpawnMorphFeature.Myself.Settings.MaxRarityWeighting);
                    }

                    // Insert multiple copies of unitDefMdd to influence the RNG selection weighted by rarity, appropriately...
                    for (var i = 0; i < rarityWeighting; i++) prefabSelectionList.Append(unit.unitDefMdd);
                }

                // Select one variant of the prefab base to include as an option in the filtered list...
                prefabSelectionList.Shuffle();
                var selectedPrefabVariant = prefabSelectionList[0];
                filteredList.Append(selectedPrefabVariant);
            }

            __result = filteredList;
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