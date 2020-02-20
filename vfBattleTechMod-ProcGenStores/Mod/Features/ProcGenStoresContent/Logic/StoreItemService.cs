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

        public List<StoreItem> GenerateItemsForStore(Shop shop, StarSystem starSystem, DateTime currentDate, List<ProcGenStoreContentFeatureSettings.PlanetTagModifier> planetTagModifiers, ProcGenStoreContentFeatureSettings settings)
        {
            this.logger.Debug($"Generating shop inventory for [{starSystem.Name} - {shop.ThisShopType.ToString()} - {starSystem.OwnerValue.Name}]...");
            List<StoreItem> storeInventory = new List<StoreItem>();
            switch (shop.ThisShopType)
            {
                case Shop.ShopType.System:
                    var potentialInventoryItems = this.StoreItems.Where(item =>
                    {
                        var result = item.IsValidForAppearance(currentDate, starSystem.OwnerValue.Name, shop.ThisShopType, settings);
                        this.logger.Debug($"[{item.Id}] - [{result.ToString()}]");
                        return result;
                    });
                    break;
            }

            return storeInventory;
        }
    }
}