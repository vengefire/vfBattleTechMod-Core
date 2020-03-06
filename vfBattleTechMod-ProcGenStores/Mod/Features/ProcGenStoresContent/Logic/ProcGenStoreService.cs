using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using HBS.Collections;
using vfBattleTechMod_Core.Utils.Interfaces;

namespace vfBattleTechMod_ProcGenStores.Mod.Features.ProcGenStoresContent.Logic
{
    public class ProcGenStoreService
    {
        public static Dictionary<BattleTechResourceType, IEnumerable<dynamic>> StoreResourceTypesDictionary { get; set; }

        private static List<ProcGenStoreContentFeatureSettings.RarityBracket> RarityBrackets;

        private static Dictionary<BattleTechResourceType, List<ProcGenStoreItem>> StoreItemsByType
            = new Dictionary<BattleTechResourceType, List<ProcGenStoreItem>>();

        private static ILogger Logger;
        
        public ProcGenStoreService(ILogger logger,
            List<ProcGenStoreContentFeatureSettings.RarityBracket> rarityBrackets,
            Dictionary<BattleTechResourceType, IEnumerable<dynamic>> storeResourceTypesDictionary)
        {
            ProcGenStoreService.StoreResourceTypesDictionary = storeResourceTypesDictionary;
            ProcGenStoreService.Logger = logger;
            ProcGenStoreService.RarityBrackets = rarityBrackets;

            // Init store items list...
            StoreResourceTypesDictionary.Keys.ToList().ForEach(type => ProcGenStoreService.StoreItemsByType[type] = new List<ProcGenStoreItem>());

            ProcGenStoreService.LoadItemsFromDataManager(rarityBrackets);
        }

        private static void LoadItemsFromDataManager(List<ProcGenStoreContentFeatureSettings.RarityBracket> rarityBrackets)
        {
            ProcGenStoreService.Logger.Debug($"Building items lists from Data Manager...");
            var simGame = UnityGameInstance.BattleTechGame.Simulation;
            var rarityMap = new List<(int min, int max, string bracket)>
            {
                (0, 1, "Common"),
                (1, 2, "Uncommon"),
                (2, 3, "VeryUncommon"),
                (3, 4, "Rare"),
                (4, 5, "VeryRare"),
                (5, 6, "PracticallyExtinct"),
                (6, int.MaxValue, "Extinct")
            };
            rarityMap.Reverse();
            
            Func<dynamic, ProcGenStoreItem> Selector(BattleTechResourceType storeResourceType)
            {
                return o =>
                {
                    string id = o.Description.Id;
                    int definedRarity = o.Description.Rarity;
                    var mappedRarity = rarityMap.First(tuple => definedRarity < tuple.max && definedRarity >= tuple.min);
                    var tagSet = GetTagsByType(storeResourceType, o);

                    var containingShopDefinitions = simGame.DataManager.Shops
                        .Select(pair => pair.Value)
                        .Where(def => def.Inventory.Select(item => item.ID).Contains(id) || def.Specials.Select(item => item.ID).Contains(id))
                        .ToList();

                    var requiredTags = containingShopDefinitions.SelectMany(def => def.RequirementTags).Distinct().ToList();
                    var exclusionTags = containingShopDefinitions.SelectMany(def => def.ExclusionTags).Distinct().ToList();
                    
                    ProcGenStoreService.Logger.Trace($"Adding [{storeResourceType.ToString()}] - [{o.Description.Id}]|" +
                                                     $"definedRarity = [{definedRarity.ToString()}, mappedRarity = [{mappedRarity.bracket}]]|" +
                                                     $"tagSet = [{string.Join(",", tagSet)}]|" +
                                                     $"requiredTags = [{string.Join("\r\n", requiredTags)}]|" +
                                                     $"exclusionTags = [{string.Join("\r\n", exclusionTags)}].");
                    return new ProcGenStoreItem(storeResourceType, o.Description.Id, tagSet, rarityBrackets.First(bracket => bracket.Name == mappedRarity.bracket), requiredTags, exclusionTags);
                };
            }

            Func<dynamic, bool> FilterTemplatesPredicate()
            {
                return item => !item.Description.Id.ToLower().Contains("template");
            }

            static TagSet GetTagsByType(BattleTechResourceType type, dynamic theObject)
            {
                return type switch
                {
                    BattleTechResourceType.AmmunitionBoxDef => theObject.ComponentTags,
                    BattleTechResourceType.UpgradeDef => theObject.ComponentTags,
                    BattleTechResourceType.HeatSinkDef => theObject.ComponentTags,
                    BattleTechResourceType.JumpJetDef => theObject.ComponentTags,
                    BattleTechResourceType.WeaponDef => theObject.ComponentTags,
                    BattleTechResourceType.MechDef => theObject.MechTags,
                    _ => throw new InvalidProgramException($"BattleTechResourceType [{type.ToString()}] unhandled.")
                };
            }

            foreach (var storeResourceType in ProcGenStoreService.StoreItemsByType.Keys)
            {
                ProcGenStoreService.Logger.Debug($"Building object lists for [{storeResourceType.ToString()}]...");
                var rawItemsList = StoreResourceTypesDictionary[storeResourceType];
                var rawItemsListSansTemplates = rawItemsList.Where(FilterTemplatesPredicate());
                ProcGenStoreService.StoreItemsByType[storeResourceType].AddRange(rawItemsListSansTemplates.Select(Selector(storeResourceType)));
                ProcGenStoreService.Logger.Debug($"Added [{ProcGenStoreService.StoreItemsByType[storeResourceType].Count.ToString()} items to list [{storeResourceType.ToString()}]].");
            }
        }
    }
}