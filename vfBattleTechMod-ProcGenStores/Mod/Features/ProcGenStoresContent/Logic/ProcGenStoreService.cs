using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using HBS.Collections;
using Newtonsoft.Json;
using vfBattleTechMod_Core.Utils.Interfaces;

namespace vfBattleTechMod_ProcGenStores.Mod.Features.ProcGenStoresContent.Logic
{
    public class ProcGenStoreService
    {
        public static Dictionary<BattleTechResourceType, List<dynamic>> StoreResourceTypesDictionary { get; set; }

        private static List<ProcGenStoreContentFeatureSettings.RarityBracket> RarityBrackets;

        private static Dictionary<BattleTechResourceType, List<ProcGenStoreItem>> StoreItemsByType
            = new Dictionary<BattleTechResourceType, List<ProcGenStoreItem>>();

        private static ILogger Logger;

        public ProcGenStoreService(ILogger logger,
            List<ProcGenStoreContentFeatureSettings.RarityBracket> rarityBrackets,
            List<BattleTechResourceType> storeResourceTypes
        )
        {
            ProcGenStoreService.Logger = logger;
            ProcGenStoreService.RarityBrackets = rarityBrackets;

            dynamic test = new List<int>() {1};
            Logger.Debug($"Count = [{test.Count}]");

            /*var simGame = UnityGameInstance.BattleTechGame.Simulation;
            var dataManager = simGame.DataManager;
            var firstMech = dataManager.MechDefs.First().Value;
            dynamic dynamicMech = firstMech;
            Logger.Debug($"Test First Mech = [{firstMech.Description.Id}]");
            Logger.Debug($"Test First Dynamic Mech = [{dynamicMech.ToString()}]");

            List<dynamic> GetItemsByType(BattleTechResourceType type, SimGameState simGameState)
            {
                return type switch
                {
                    BattleTechResourceType.AmmunitionBoxDef => simGameState.DataManager.AmmoBoxDefs.Select(pair => (dynamic) pair.Value).ToList(),
                    BattleTechResourceType.UpgradeDef => simGameState.DataManager.UpgradeDefs.Select(pair => (dynamic) pair.Value).ToList(),
                    BattleTechResourceType.HeatSinkDef => simGameState.DataManager.HeatSinkDefs.Select(pair => (dynamic) pair.Value).ToList(),
                    BattleTechResourceType.JumpJetDef => simGameState.DataManager.JumpJetDefs.Select(pair => (dynamic) pair.Value).ToList(),
                    BattleTechResourceType.WeaponDef => simGameState.DataManager.WeaponDefs.Select(pair => (dynamic) pair.Value).ToList(),
                    BattleTechResourceType.MechDef => simGameState.DataManager.MechDefs.Select(pair => (dynamic) pair.Value).ToList(),
                    _ => throw new InvalidProgramException($"BattleTechResourceType [{type.ToString()}] unhandled.")
                };
            }

            Logger.Debug($"Constructing type->object dictionary...");
            ProcGenStoreService.StoreResourceTypesDictionary = storeResourceTypes
                .Select(type => new
                    {Key = type, Value = GetItemsByType(type, UnityGameInstance.BattleTechGame.Simulation)})
                .ToDictionary(arg => arg.Key, arg => arg.Value);

            Logger.Debug($"type->object dictionary constructed.");

            Logger.Debug($"StoreResourceTypesDictionary.Keys = [{string.Join("\r\n", StoreResourceTypesDictionary.Keys)}]");
            // Init store items list...
            StoreResourceTypesDictionary.Keys.ToList().ForEach(type =>
            {
                Logger.Debug($"Instancing up ProcGenStoreItemList for [{type}]...");
                ProcGenStoreService.StoreItemsByType[type] = new List<ProcGenStoreItem>();
            });

            Logger.Debug($"ProcGenStoreService.StoreItemsByType initialised.");
            LoadItemsFromDataManager();*/
        }

        private void LoadItemsFromDataManager()
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

            static TagSet GetTagsByType(BattleTechResourceType type, dynamic theObject)
            {
                switch (type)
                {
                    case BattleTechResourceType.AmmunitionBoxDef:
                    case BattleTechResourceType.UpgradeDef:
                    case BattleTechResourceType.HeatSinkDef:
                    case BattleTechResourceType.JumpJetDef:
                    case BattleTechResourceType.WeaponDef:
                        return theObject.ComponentTags;
                    case BattleTechResourceType.MechDef:
                        return theObject.MechTags;
                    default:
                        throw new InvalidProgramException($"BattleTechResourceType [{type.ToString()}] unhandled.");
                }
            }
            
            static dynamic GetDynamicObjectByType(BattleTechResourceType type, object theObject)
            {
                return type switch
                {
                    BattleTechResourceType.AmmunitionBoxDef => (dynamic) (AmmunitionBoxDef) theObject,
                    BattleTechResourceType.UpgradeDef => (UpgradeDef) theObject,
                    BattleTechResourceType.HeatSinkDef => (HeatSinkDef) theObject,
                    BattleTechResourceType.JumpJetDef => (JumpJetDef) theObject,
                    BattleTechResourceType.WeaponDef => (WeaponDef) theObject,
                    BattleTechResourceType.MechDef => (MechDef) theObject,
                    _ => throw new InvalidProgramException($"BattleTechResourceType [{type.ToString()}] unhandled.")
                };
            }

            foreach (var storeResourceType in ProcGenStoreService.StoreItemsByType.Keys)
            {
                ProcGenStoreService.Logger.Debug($"Building object lists for [{storeResourceType.ToString()}]...");
                var rawItemsList = StoreResourceTypesDictionary[storeResourceType];
                Logger.Debug($"First Test Item = {GetDynamicObjectByType(storeResourceType, rawItemsList.First()).ToJSON()}...");
                var rawItemsListSansTemplates = rawItemsList.Where(theObject => !GetDynamicObjectByType(storeResourceType, theObject).Description.Id.ToLower().Contains("template")).ToList();
                ProcGenStoreService.StoreItemsByType[storeResourceType].AddRange(rawItemsListSansTemplates.Select(
                    o =>
                    {
                        string id = o.Description.Id;
                        int definedRarity = o.Description.Rarity;
                        var mappedRarity =
                            rarityMap.First(tuple => definedRarity < tuple.max && definedRarity >= tuple.min);
                        var tagSet = GetTagsByType(storeResourceType, o);

                        var containingShopDefinitions = simGame.DataManager.Shops
                            .Select(pair => pair.Value)
                            .Where(def =>
                                def.Inventory.Select(item => item.ID).Contains(id) ||
                                def.Specials.Select(item => item.ID).Contains(id))
                            .ToList();

                        var requiredTags = containingShopDefinitions.SelectMany(def => def.RequirementTags).Distinct()
                            .ToList();
                        var exclusionTags = containingShopDefinitions.SelectMany(def => def.ExclusionTags).Distinct()
                            .ToList();

                        ProcGenStoreService.Logger.Trace(
                            $"Adding [{storeResourceType.ToString()}] - [{o.Description.Id}]|" +
                            $"definedRarity = [{definedRarity.ToString()}, mappedRarity = [{mappedRarity.bracket}]]|" +
                            $"tagSet = [{string.Join(",", tagSet)}]|" +
                            $"requiredTags = [{string.Join("\r\n", requiredTags)}]|" +
                            $"exclusionTags = [{string.Join("\r\n", exclusionTags)}].");
                        return new ProcGenStoreItem(storeResourceType, o.Description.Id, tagSet,
                            RarityBrackets.First(bracket => bracket.Name == mappedRarity.bracket), requiredTags,
                            exclusionTags);
                    }
                ).ToList());
                ProcGenStoreService.Logger.Debug(
                    $"Added [{ProcGenStoreService.StoreItemsByType[storeResourceType].Count.ToString()} items to list [{storeResourceType.ToString()}]].");
            }
        }
    }
}