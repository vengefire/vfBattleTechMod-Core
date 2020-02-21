using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using vfBattleTechMod_Core.Extensions;
using vfBattleTechMod_Core.Utils.Interfaces;

namespace vfBattleTechMod_ProcGenStores.Mod.Features.ProcGenStoresContent.Logic
{
    public class StoreItemService
    {
        public List<StoreItem> StoreItems { get; set; } = new List<StoreItem>();
        public List<ProcGenStoreContentFeatureSettings.RarityBracket> RarityBrackets = new List<ProcGenStoreContentFeatureSettings.RarityBracket>();
        private readonly ILogger logger;

        public StoreItemService(string storeItemSourceFilePath, List<ProcGenStoreContentFeatureSettings.RarityBracket> rarityBrackets, List<BattleTechResourceType> storeResourceTypes, ILogger logger)
        {
            this.logger = logger;
            this.StoreItems = StoreItemLoader.LoadStoreItemsFromExcel(storeItemSourceFilePath, rarityBrackets, storeResourceTypes, logger);
        }

        public List<StoreItem> GenerateItemsForStore(Shop.ShopType shopType, string starSystemName, string ownerName, DateTime currentDate, List<ProcGenStoreContentFeatureSettings.PlanetTagModifier> planetTagModifiers, ProcGenStoreContentFeatureSettings settings)
        {
            this.logger.Debug($"Generating shop inventory for [{starSystemName} - {shopType.ToString()} - {ownerName}]...");
            var potentialInventoryItems = this.IdentifyPotentialInventoryItems(shopType, ownerName, currentDate, settings);
            this.logger.Debug($"Potential Inventory Items = {string.Join("\r\n", potentialInventoryItems.Select(item => item.Id))}");
            List<StoreItem> storeInventory = this.ProduceStoreInventoryFromPotentialItemList(shopType, ownerName, currentDate, settings, planetTagModifiers, potentialInventoryItems);
            this.logger.Debug($"Final Inventory Items = \r\n{string.Join("\r\n", storeInventory.Select(item => $"{item.Id} @ {item.Quantity} units"))}");

            return storeInventory;
        }

        private List<StoreItem> ProduceStoreInventoryFromPotentialItemList(Shop.ShopType shopType, string ownerName, DateTime currentDate, ProcGenStoreContentFeatureSettings settings, List<ProcGenStoreContentFeatureSettings.PlanetTagModifier> planetTagModifiers, List<StoreItem> potentialInventoryItems)
        {
            this.logger.Debug($"Rolling for inventory stock...");
            List<StoreItem> inventoryItems = new List<StoreItem>();
            Random random = new Random();
            potentialInventoryItems.ForEach(
                potentialItem =>
                {
                    var addedToStore = false;
                    var validRarityBrackets = settings.RarityBrackets.Where(bracket => bracket.Order >= potentialItem.RarityBracket.Order).ToList().OrderBy(bracket => bracket.Order).ToList();
                    this.logger.Debug($"Rolling for item [{potentialItem.Id}]...");

                    foreach (var bracket in validRarityBrackets)
                    {
                        double chance = bracket.ChanceToAppear;
                        planetTagModifiers.ForEach(modifier => chance *= modifier.ChanceModifier);
                        var appearanceRoll = random.NextDouble();

                        this.logger.Debug($"Default chance = [{bracket.ChanceToAppear}] for [{bracket.Name}]\r\n" +
                                          $"Planet Modifiers = [{string.Join(",", planetTagModifiers.Select(modifier => $"{modifier.Tag} - {modifier.ChanceModifier}"))}]\r\n" +
                                          $"Final Chance = [{chance}]\r\n" +
                                          $"Roll = [{appearanceRoll}]");

                        if (appearanceRoll <= chance)
                        {
                            var storeItem = potentialItem.Copy();
                            var quantityRoll = random.Next(bracket.QuantityBracket.LowCount, bracket.QuantityBracket.HighCount + 1);
                            storeItem.Quantity = bracket.QuantityBracket.LowCount == -1 ? -1 : quantityRoll;
                            planetTagModifiers.ForEach(modifier => storeItem.Quantity = Convert.ToInt32(Math.Round(Convert.ToDouble(storeItem.Quantity) * modifier.QuantityModifier, 0)));

                            this.logger.Debug($"Rolling for quantity [{potentialItem.Id}].\r\n" +
                                              $"Default range = [{bracket.QuantityBracket.LowCount} - {bracket.QuantityBracket.HighCount}] for [{bracket.QuantityBracket.Name}]\r\n" +
                                              $"Planet Modifiers = [{string.Join(",", planetTagModifiers.Select(modifier => $"{modifier.Tag} - {modifier.QuantityModifier}"))}]\r\n" +
                                              $"Unmodified Roll = [{quantityRoll}]\r\n" +
                                              $"Modified Roll = [{storeItem.Quantity}]");

                            this.logger.Debug($"Adding [{storeItem.Id}] to store with quantity [{storeItem.Quantity}].{(potentialItem.RarityBracket.Order != bracket.Order ? "CASCADE SUCCESS":"")}");

                            inventoryItems.Add(storeItem);
                            addedToStore = true;
                            break;
                        }
                        else
                        {
                            if (!settings.CascadeRollsOnFail)
                            {
                                this.logger.Debug($"CASCADE DISABLED : [{potentialItem.Id}] FAILED roll");
                                break;
                            }
                            else
                            {
                                this.logger.Debug($"CASCADE ENABLED : [{potentialItem.Id}] FAILED roll, checking next rarity bracket...");
                            }
                        }
                    }
                    this.logger.Debug($"[{potentialItem.Id}] - [{(addedToStore ? "added to store" : "not added to store")}.]");
                });
            return inventoryItems;
        }

        public List<StoreItem> IdentifyPotentialInventoryItems(Shop.ShopType shopType, string ownerName, DateTime currentDate, ProcGenStoreContentFeatureSettings settings)
        {
            List<StoreItem> potentialInventoryItems = new List<StoreItem>();
            switch (shopType)
            {
                case Shop.ShopType.System:
                    potentialInventoryItems = this.StoreItems.Where(
                        item =>
                        {
                            var result = item.IsValidForAppearance(currentDate, ownerName, shopType, settings);
                            this.logger.Debug($"[{item.Id}] - [{result.ToString()}]");
                            return result;
                        }).ToList();
                    break;
            }

            return potentialInventoryItems;
        }
    }
}