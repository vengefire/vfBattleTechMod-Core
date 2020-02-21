using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
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
            List<StoreItem> storeInventory = new List<StoreItem>();
            var potentialInventoryItems = this.IdentifyPotentialInventoryItems(shopType, ownerName, currentDate, settings);

            return storeInventory;
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