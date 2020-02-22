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

        public (bool result, int bracketBonus) IsValidForAppearance(DateTime currentDate, string ownerValueName, Shop.ShopType shopThisShopType,
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
                    return (true, 0);
                }

                if (wasReintroduced && currentDate >= ReintroductionDate && wasReintroducedByOwner)
                {
                    return (true, 2);
                }

                // Went extinct, and was either never reintroduced by the owner, or never went common (Skipped reintro)
                return (false, 0);
            }

            // Check prototype and production parameters...
            if (PrototypeDate == null && ProductionDate == null)
            {
                if (CommonDate == null)
                {
                    return (true, 0);
                }

                // Never went extinct, has no prototype or production date. If it's now common, return true with a bracket roll bonus, else false.
                return (isNowCommon, 2);
            }

            if (PrototypeDate != null)
            {
                if (currentDate < PrototypeDate)
                {
                    // Before it was ever prototyped...
                    return (false, 0);
                }

                if (ProductionDate != null && currentDate >= ProductionDate)
                {
                    // It was prototyped and subsequently produced, or it went common 
                    if (!isNowCommon)
                    {
                        // Not common yet, but produced by owner...
                        return (wasProducedByOwner, 2);
                    }
                    
                    // It's now common
                    return (true, 2);
                }

                // We're in the prototype period, and the owner of the store is the prototype agent...
                return (wasPrototypedByOwner, 2);
            }

            // No valid periods or matching owners...
            return (false, 0);
        }
    }
}