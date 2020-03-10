using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using BattleTech;
using HBS.Collections;
using HBS.Util;
using vfBattleTechMod_Core.Utils.Interfaces;
using vfBattleTechMod_ProcGenStores.Mod.Features.ProcGenStoresContent.Logic.MetaDataHelpers;

namespace vfBattleTechMod_ProcGenStores.Mod.Features.ProcGenStoresContent.Logic
{
    public class ProcGenStoreService
    {
        private static List<ProcGenStoreContentFeatureSettings.RarityBracket> RarityBrackets;

        private static readonly Dictionary<BattleTechResourceType, List<ProcGenStoreItem>> StoreItemsByType
            = new Dictionary<BattleTechResourceType, List<ProcGenStoreItem>>();

        private static ILogger Logger;

        public ProcGenStoreService(ILogger logger,
            ProcGenStoreContentFeatureSettings settings,
            List<BattleTechResourceType> storeResourceTypes
        )
        {
            ProcGenStoreService.Logger = logger;
            ProcGenStoreService.Settings = settings;
            ProcGenStoreService.RarityBrackets = settings.RarityBrackets;

            storeResourceTypes.ForEach(type => StoreItemsByType[type] = new List<ProcGenStoreItem>());

            logger.Debug($"Loading items from data manager...");
            LoadItemsFromDataManager();
            Logger.Debug($"Items loaded from data manager.");
        }

        public static ProcGenStoreContentFeatureSettings Settings { get; set; }

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

            static TagSet GetTagsByType(BattleTechResourceType type, object theObject)
            {
                switch (type)
                {
                    case BattleTechResourceType.AmmunitionBoxDef: return ((AmmunitionBoxDef) theObject).ComponentTags;
                    case BattleTechResourceType.UpgradeDef: return ((UpgradeDef) theObject).ComponentTags;
                    case BattleTechResourceType.HeatSinkDef: return ((HeatSinkDef) theObject).ComponentTags;
                    case BattleTechResourceType.JumpJetDef: return ((JumpJetDef) theObject).ComponentTags;
                    case BattleTechResourceType.WeaponDef: return ((WeaponDef) theObject).ComponentTags;
                    case BattleTechResourceType.MechDef: return ((MechDef) theObject).MechTags;
                    default:
                        throw new InvalidProgramException($"BattleTechResourceType [{type.ToString()}] unhandled.");
                }
            }

            static List<object> GetObjectListByType(BattleTechResourceType type, SimGameState simGame)
            {
                return type switch
                {
                    BattleTechResourceType.AmmunitionBoxDef => simGame.DataManager.AmmoBoxDefs
                        .Select(pair => pair.Value).Cast<object>().ToList(),
                    BattleTechResourceType.UpgradeDef => simGame.DataManager.UpgradeDefs.Select(pair => pair.Value)
                        .Cast<object>().ToList(),
                    BattleTechResourceType.HeatSinkDef => simGame.DataManager.HeatSinkDefs.Select(pair => pair.Value)
                        .Cast<object>().ToList(),
                    BattleTechResourceType.JumpJetDef => simGame.DataManager.JumpJetDefs.Select(pair => pair.Value)
                        .Cast<object>().ToList(),
                    BattleTechResourceType.WeaponDef => simGame.DataManager.WeaponDefs.Select(pair => pair.Value)
                        .Cast<object>().ToList(),
                    BattleTechResourceType.MechDef => simGame.DataManager.MechDefs.Select(pair => pair.Value)
                        .Cast<object>().ToList(),
                    _ => throw new InvalidProgramException($"BattleTechResourceType [{type.ToString()}] unhandled.")
                };
            }

            static DescriptionDef GetObjectDescriptionByType(BattleTechResourceType type, object theObject)
            {
                return type switch
                {
                    BattleTechResourceType.AmmunitionBoxDef => ((AmmunitionBoxDef) theObject).Description,
                    BattleTechResourceType.UpgradeDef => ((UpgradeDef) theObject).Description,
                    BattleTechResourceType.HeatSinkDef => ((HeatSinkDef) theObject).Description,
                    BattleTechResourceType.JumpJetDef => ((JumpJetDef) theObject).Description,
                    BattleTechResourceType.WeaponDef => ((WeaponDef) theObject).Description,
                    BattleTechResourceType.MechDef => ((MechDef) theObject).Description,
                    _ => throw new InvalidProgramException($"BattleTechResourceType [{type.ToString()}] unhandled.")
                };
            }

            Logger.Debug($"Parsing backup canon availability data...");
            var mechAppearanceData = MechModel.ProcessAvailabilityFile(Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Settings.MechAppearanceFile));

            foreach (var storeResourceType in ProcGenStoreService.StoreItemsByType.Keys)
            {
                ProcGenStoreService.Logger.Debug($"Building object lists for [{storeResourceType.ToString()}]...");
                var rawItemsList = GetObjectListByType(storeResourceType, simGame);
                var rawItemsListSansTemplates = rawItemsList.Where(theObject =>
                        !GetObjectDescriptionByType(storeResourceType, theObject).Id.ToLower().Contains("template"))
                    .ToList();
                ProcGenStoreService.StoreItemsByType[storeResourceType].AddRange(rawItemsListSansTemplates.Select(
                    o =>
                    {
                        var description = GetObjectDescriptionByType(storeResourceType, o);
                        string id = description.Id;
                        float definedRarity = description.Rarity;
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

                        DateTime? appearanceDate = null;
                        if (o is MechDef mechDef)
                        {
                            var mechModelEntry = mechAppearanceData.FirstOrDefault(model =>
                                model.Name.Trim('"') == mechDef.Description.UIName);
                            if (mechModelEntry != null)
                            {
                                appearanceDate = new DateTime(mechModelEntry.Year, 1, 1);
                            }

                            appearanceDate = mechDef.MinAppearanceDate ?? appearanceDate;
                        }

                        ProcGenStoreService.Logger.Trace(
                            $"Adding [{storeResourceType.ToString()}] - [{description.Id}]|" +
                            $"minAppearanceDate = [{appearanceDate.ToString()}]|" +
                            $"definedRarity = [{definedRarity.ToString(CultureInfo.InvariantCulture)}, mappedRarity = [{mappedRarity.bracket}]]|" +
                            $"tagSet = [{string.Join(",", tagSet)}]|" +
                            $"requiredTags = [{string.Join(", ", requiredTags)}]|" +
                            $"exclusionTags = [{string.Join(", ", exclusionTags)}].");
                        return new ProcGenStoreItem(storeResourceType, description.Id, appearanceDate, tagSet,
                            RarityBrackets.First(bracket => bracket.Name == mappedRarity.bracket), requiredTags,
                            exclusionTags);
                    }
                ).ToList());
                ProcGenStoreService.Logger.Debug(
                    $"Added [{ProcGenStoreService.StoreItemsByType[storeResourceType].Count.ToString()} items to list [{storeResourceType.ToString()}]].");
            }
            ProcGenStoreService.Logger.Debug(
                $"Mechs without appearance dates = [\r\n{string.Join("\r\n", StoreItemsByType[BattleTechResourceType.MechDef].Where(item => !item.MinAppearanceDate.HasValue).Select(item => item.Id))}]");
        }
    }
}