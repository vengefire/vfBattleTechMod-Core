using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using vfBattleTechMod_Core.Utils.Interfaces;

namespace vfBattleTechMod_ProcGenStores.Mod.Features.ProcGenStoresContent.Logic
{
    public class ProcGenStoreService
    {
        private readonly ILogger _logger;
        private readonly List<ProcGenStoreContentFeatureSettings.RarityBracket> _rarityBrackets;
        // private readonly List<BattleTechResourceType> _storeResourceTypes;
        private static readonly Dictionary<BattleTechResourceType, List<ProcGenStoreItem>> StoreItemsByType = new Dictionary<BattleTechResourceType, List<ProcGenStoreItem>>();

        public ProcGenStoreService(ILogger logger, 
            List<ProcGenStoreContentFeatureSettings.RarityBracket> rarityBrackets, 
            List<BattleTechResourceType> storeResourceTypes)
        {
            _logger = logger;
            _rarityBrackets = rarityBrackets;
            storeResourceTypes.ForEach(type => ProcGenStoreService.StoreItemsByType[type] = new List<ProcGenStoreItem>());

            ProcGenStoreService.LoadItemsFromDataManager();
        }

        private static void LoadItemsFromDataManager()
        {
            Func<KeyValuePair<string, dynamic>, ProcGenStoreItem> ProcGenStoreItemSelector(BattleTechResourceType storeResourceType)
            {
                return pair => new ProcGenStoreItem(storeResourceType, pair.Value.Description.Id, pair.Value.ComponentTags);
            }

            Func<KeyValuePair<string, UpgradeDef>, bool> FilterTemplatesPredicate()
            {
                return pair => !pair.Value.Description.Id.ToLower().Contains("template");
            }

            var simGame = UnityGameInstance.BattleTechGame.Simulation;
            foreach (var storeResourceType in ProcGenStoreService.StoreItemsByType.Keys)
            {
                switch (storeResourceType)
                {
                    case BattleTechResourceType.UpgradeDef:
                        ProcGenStoreService
                            .StoreItemsByType[storeResourceType]
                            .AddRange(simGame.DataManager.UpgradeDefs
                                .Where(FilterTemplatesPredicate())
                                .Select<UpgradeDef, ProcGenStoreItem>(ProcGenStoreItemSelector(storeResourceType)));
                        break;
                }
            }
        }
    }
}