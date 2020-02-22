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

        public int Quantity { get; set; } = 0;

        public bool IsValidForAppearance(DateTime currentDate, string ownerValueName, Shop.ShopType shopThisShopType,
            ProcGenStoreContentFeatureSettings settings)
        {
            var wasPrototypedByOwner = PrototypeDate != null && ownerValueName == PrototypeFaction;
            var wasProducedByOwner = ProductionDate != null && ownerValueName == ProductionFaction;
            var wentExtinct = ExtinctionDate != null;
            var wasReintroduced = ReintroductionDate != null || CommonDate != null;
            var wasReintroducedByOwner = wasReintroduced && ownerValueName == ReintroductionFaction;
            var isNowCommon = CommonDate == null ? false : currentDate >= CommonDate;

            // Careful... if it has no reintro details, we're inferring it was made common without having reintro, hence order of testing is important...
            if (wentExtinct && currentDate >= ExtinctionDate)
            {
                if (isNowCommon)
                {
                    return true;
                }

                if (wasReintroduced && currentDate >= ReintroductionDate && wasReintroducedByOwner)
                {
                    return true;
                }

                // Went extinct, and was either never reintroduced by the owner, or never went common (Skipped reintro)
                return false;
            }

            // Check prototype and production parameters...
            if (PrototypeDate == null && ProductionDate == null)
            {
                return CommonDate == null || isNowCommon;
            }

            if (PrototypeDate != null)
            {
                if (currentDate < PrototypeDate)
                {
                    // Before it was ever prototyped...
                    return false;
                }

                if (ProductionDate != null && currentDate >= ProductionDate)
                {
                    // It was prototyped and subsequently produced, or it went common 
                    return wasProducedByOwner || isNowCommon;
                }

                return wasPrototypedByOwner;
            }

            return false;
        }
    }
}