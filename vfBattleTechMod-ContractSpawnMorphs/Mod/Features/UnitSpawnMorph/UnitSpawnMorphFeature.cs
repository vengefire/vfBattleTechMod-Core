using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Data;
using HBS.Collections;
using vfBattleTechMod_Core.Mods.BaseImpl;
using vfBattleTechMod_Core.Mods.Interfaces;

namespace vfBattleTechMod_ContractSpawnMorphs.Mod.Features.UnitSpawnMorph
{
    public class UnitSpawnMorphFeature : ModFeatureBase<UnitSpawnMorphFeatureSettings>
    {
        private new static UnitSpawnMorphFeature Myself;

        public UnitSpawnMorphFeature()
            : base(GetPatchDirectives)
        {
            Myself = this;
        }

        public static List<IModPatchDirective> GetPatchDirectives =>
            new List<IModPatchDirective>
            {
                new ModPatchDirective(
                    typeof(TagSetQueryExtensions).GetMethod("GetMatchingUnitDefs"),
                    null,
                    typeof(UnitSpawnMorphFeature).GetMethod("TagSetQueryExtensions_GetMatchingUnitDefs_Postfix"),
                    null,
                    0)
            };

        public override string Name => "Procedurally Morph Contract Unit Spawns";

        public static void TagSetQueryExtensions_GetMatchingUnitDefs_Postfix(MetadataDatabase __instance, TagSet requiredTags, DateTime? currentDate, ref List<UnitDef_MDD> __result)
        {
            Logger.Debug($"Executing [{nameof(TagSetQueryExtensions_GetMatchingUnitDefs_Postfix)}],\r\n" +
                         $"RequiredTags = [{string.Join(", ", requiredTags)}]\r\n" +
                         $"MatchingDataByTagSet = [{string.Join("\r\n", __result.Select(defMdd => defMdd.UnitDefID))}]...");
            
            if (requiredTags.Contains("unit_vehicle") || requiredTags.Contains("unit_turret"))
            {
                Logger.Debug($"Bypassing lance spawn morph as required tags are not looking for mechs...");
                return;
            }
            else if (__result.Count == 0)
            {
                Logger.Debug($"Bypassing lance spawn morph as initial result is empty...");
                return;
            }
            
            // Alias the keywords for readability...
            var mdd = __instance;
            var matchingDataByTagSet = __result;

            var simGameState = UnityGameInstance.BattleTechGame.Simulation;
            var filteredList = new List<UnitDef_MDD>();

            // Group matching unitDef_MDD records by their associated prefabIdentifier
            var unitsGroupedByPrefab = matchingDataByTagSet
                .Select(defMdd => new {unitDefMdd = defMdd, mechDef = simGameState.DataManager.MechDefs.First(pair => pair.Key == defMdd.UnitDefID).Value})
                .GroupBy(arg => arg.mechDef.Chassis.PrefabIdentifier, arg => arg, (s, enumerable) => new {Base = s, Units = enumerable})
                .ToList();
            
            Logger.Debug($"Grouped result list into [\r\n" +
                         $"{string.Join("\r\n", unitsGroupedByPrefab.Select(arg => $"[{arg.Base}] -> {string.Join(", ", arg.Units.Select(arg1 => arg1.unitDefMdd.UnitDefID))}"))}]");

            // var prefabVariantsOccuringOnce = unitsGroupedByPrefab.Where(arg => arg.Units.Count() == 1).SelectMany(arg => arg.Units).ToList();
            foreach (var prefabGroup in unitsGroupedByPrefab)
            {
                Logger.Debug($"Processing units for prefab [{prefabGroup.Base}]...");
                var prefabSelectionList = new List<UnitDef_MDD>();
                foreach (var unit in prefabGroup.Units)
                {
                    Logger.Trace($"Processing unit [{unit.mechDef.Description.Id}], CurrentDate = [{currentDate}], MinAppearanceDate = [{unit.mechDef.MinAppearanceDate}]...");
                    // For mechs with an appearance date (and current date is set, which it should always be)
                    // 1. Each entry gets a rarityWeighting + 1 per 30 days since appearance date has passed
                    // 2. To a maximum of 6 (so mechs that appeared at least 180 days prior to the current date receive the maximum rarity weighting)
                    // These following two variables ought to be set via [Settings] in the mod.json...
                    var rarityWeighting = Myself.Settings.MaxRarityWeighting;
                    if (currentDate != null && unit.mechDef.MinAppearanceDate != null)
                    {
                        // Could do this in only one statement, but that ended up being a little kludgy and hard to read...
                        var rawDays = (currentDate - unit.mechDef.MinAppearanceDate).Value.TotalDays + 1;
                        var roundedDays = Math.Round(rawDays / Myself.Settings.RarityWeightingDaysDivisor, 0);
                        var rawRarity = Convert.ToInt32(roundedDays);
                        rarityWeighting = Math.Min(rawRarity, Myself.Settings.MaxRarityWeighting);
                        if (rarityWeighting <= 0)
                        {
                            Logger.Trace($"Rarity negative for [{unit.unitDefMdd.UnitDefID}], fixing to 1...");
                            rarityWeighting = 1;
                        }
                        Logger.Trace($"Raw Days = [{rawDays}], " +
                                     $"Rounded Days = [{roundedDays}], " +
                                     $"Raw Rarity = [{rawRarity}], " +
                                     $"Final Rarity Rating = [{rarityWeighting}]");
                    }

                    // Insert multiple copies of unitDefMdd to influence the RNG selection weighted by rarity, appropriately...
                    for (var i = 0; i < rarityWeighting; i++)
                    {
                        Logger.Trace($"Adding [{unit.unitDefMdd.UnitDefID}] to prefabSelectionList...");
                        prefabSelectionList.Add(unit.unitDefMdd);
                    }
                }

                Logger.Trace($"PrefabSelectionList count = [{prefabSelectionList.Count}]");
                var prefabSelectionListGroupByPrefab = prefabSelectionList
                    .Select(defMdd => new {unitDefMdd = defMdd, mechDef = simGameState.DataManager.MechDefs.First(pair => pair.Key == defMdd.UnitDefID).Value})
                    .GroupBy(arg => arg.unitDefMdd.UnitDefID, arg => arg, (s, enumerable) => new {Base = s, units = enumerable});
                
                Logger.Debug($"Final Prefab Selection List = [\r\n" +
                             $"{string.Join("\r\n", prefabSelectionListGroupByPrefab.Select(arg => $"[{arg.Base}] - Count [{arg.units.Count()}]"))}" +
                             $"]");

                // Select one variant of the prefab base to include as an option in the filtered list...
                Logger.Trace($"Shuffling prefab selection list...");
                prefabSelectionList.Shuffle();
                var selectedPrefabVariant = prefabSelectionList[0];
                Logger.Debug($"Selected [{selectedPrefabVariant.UnitDefID} for inclusion in final filtered list...]");
                filteredList.Add(selectedPrefabVariant);
            }

            Logger.Debug($"Final filtered list = [\r\n" +
                         $"{string.Join("\r\n", filteredList.Select(defMdd => defMdd.UnitDefID))}" +
                         $"]");
            
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