using System;
using BattleTech;

namespace vfBattleTechMod_ProcGenStores.Mod.Features.ProcGenStoresContent.Logic
{
    public class StoreItem
    {
        public string Id { get; set; }

        public DateTime? PrototypeDate { get; set; } = null;

        public string PrototypeFaction { get; set; } = string.Empty;

        public DateTime? ProductionDate { get; set; } = null;

        public string ProductionFaction { get; set; } = string.Empty;

        public DateTime? ExtinctionDate { get; set; } = null;

        public DateTime? ReintroductionDate { get; set; } = null;

        public string ReintroductionFaction { get; set; } = string.Empty;

        public DateTime? CommonDate { get; set; } = null;

        public ProcGenStoreContentFeatureSettings.RarityBracket RarityBracket { get; set; }

        public BattleTechResourceType Type { get; set; }
    }
}