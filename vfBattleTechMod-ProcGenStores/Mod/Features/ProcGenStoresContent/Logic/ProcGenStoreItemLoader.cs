﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using BattleTech;
using HBS.Collections;
using vfBattleTechMod_Core.Utils.Interfaces;
using vfBattleTechMod_ProcGenStores.Mod.Features.ProcGenStoresContent.Logic.MetaDataHelpers;

namespace vfBattleTechMod_ProcGenStores.Mod.Features.ProcGenStoresContent.Logic
{
    public static class ProcGenStoreItemLoader
    {
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

        static List<object> GetObjectListByType(BattleTechResourceType type, SimGameState simGame)
        {
            return type switch
            {
                BattleTechResourceType.AmmunitionBoxDef => simGame.DataManager.AmmoBoxDefs
                    .Select(pair => (object) pair.Value).ToList(),
                BattleTechResourceType.UpgradeDef => simGame.DataManager.UpgradeDefs.Select(pair => (object) pair.Value)
                    .ToList(),
                BattleTechResourceType.HeatSinkDef => simGame.DataManager.HeatSinkDefs
                    .Select(pair => (object) pair.Value).ToList(),
                BattleTechResourceType.JumpJetDef => simGame.DataManager.JumpJetDefs.Select(pair => (object) pair.Value)
                    .ToList(),
                BattleTechResourceType.WeaponDef => simGame.DataManager.WeaponDefs.Select(pair => (object) pair.Value)
                    .ToList(),
                BattleTechResourceType.MechDef => simGame.DataManager.MechDefs.Select(pair => (object) pair.Value)
                    .ToList(),
                _ => throw new InvalidProgramException($"BattleTechResourceType [{type.ToString()}] unhandled.")
            };
        }

        public static Dictionary<BattleTechResourceType, List<ProcGenStoreItem>> LoadItemsFromDataManager(
            ILogger logger, ProcGenStoreContentFeatureSettings settings,
            List<BattleTechResourceType> storeResourceTypes)
        {
            logger.Debug($"Building items lists from Data Manager...");
            var simGame = UnityGameInstance.BattleTechGame.Simulation;
            var rarityBrackets = settings.RarityBrackets;
            var storeItemsByType = new Dictionary<BattleTechResourceType, List<ProcGenStoreItem>>();
            storeResourceTypes.ForEach(type => storeItemsByType[type] = new List<ProcGenStoreItem>());
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

            logger.Debug($"Parsing backup canon availability data...");
            var mechAppearanceData =
                MechModel.ProcessAvailabilityFile(AvailabilityFilePath(settings.MechAppearanceFile));

            foreach (var storeResourceType in storeItemsByType.Keys)
            {
                logger.Debug($"Building object lists for [{storeResourceType.ToString()}]...");
                var rawItemsList = GetObjectListByType(storeResourceType, simGame);
                var rawItemsListSansTemplates = rawItemsList.Where(theObject =>
                    {
                        var description = GetObjectDescriptionByType(storeResourceType, theObject);
                        if (description.Id.ToLower().Contains("template") || !description.Purchasable)
                        {
                            logger.Trace($"Filtering out [{description.Id}], Purchasable = [{description.Purchasable}].");
                            return false;
                        }
                        // Filter out templates and items flagged as non-purchasable...
                        return true;
                    })
                    .ToList();
                
                var itemDetails = rawItemsListSansTemplates.Select(
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

                        var invalid_tags = new List<string>() {"debug"};
                        requiredTags.RemoveAll(s => invalid_tags.Contains(s));
                        exclusionTags.RemoveAll(s => invalid_tags.Contains(s));

                        DateTime? appearanceDate = null;
                        appearanceDate = GetAppearanceDate(o, mechAppearanceData);

                        logger.Trace(
                            $"Adding [{storeResourceType.ToString()}] - [{description.Id}]|" +
                            $"minAppearanceDate = [{appearanceDate.ToString()}]|" +
                            $"definedRarity = [{definedRarity.ToString(CultureInfo.InvariantCulture)}, mappedRarity = [{mappedRarity.bracket}]]|" +
                            $"tagSet = [{string.Join(",", tagSet)}]|" +
                            $"requiredTags = [{string.Join(", ", requiredTags)}]|" +
                            $"exclusionTags = [{string.Join(", ", exclusionTags)}].");

                        return new ProcGenStoreItem(storeResourceType, description.Id, appearanceDate, tagSet,
                            rarityBrackets.First(bracket => bracket.Name == mappedRarity.bracket), requiredTags,
                            exclusionTags);
                    }
                ).ToList();
                storeItemsByType[storeResourceType].AddRange(itemDetails);

                logger.Debug(
                    $"Added [{storeItemsByType[storeResourceType].Count.ToString()} items to list [{storeResourceType.ToString()}]].");
            }

            logger.Debug(
                $"Mechs without appearance dates (and therefore removed) = [\r\n{string.Join("\r\n", storeItemsByType[BattleTechResourceType.MechDef].Where(item => !item.MinAppearanceDate.HasValue).Select(item => item.Id))}]");
            storeItemsByType[BattleTechResourceType.MechDef].RemoveAll(item => !item.MinAppearanceDate.HasValue);

            return storeItemsByType;
        }

        private static string AvailabilityFilePath(string mechAppearanceFile) =>
            Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                throw new InvalidProgramException($"Executing Assembly Location cannot be null."),
                mechAppearanceFile);

        private static DateTime? GetAppearanceDate(object o, List<MechModel> mechAppearanceData)
        {
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

            return appearanceDate;
        }
    }
}