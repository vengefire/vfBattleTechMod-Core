using System.Collections.Generic;
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
    }
}