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

        public bool IsValidForAppearance(DateTime currentDate, string ownerValueName, Shop.ShopType shopThisShopType, ProcGenStoreContentFeatureSettings settings)
        {
            var wasPrototypedByOwner = this.PrototypeDate != null && ownerValueName == this.PrototypeFaction;
            var wasProducedByOwner = this.ProductionDate != null && ownerValueName == this.ProductionFaction;
            var wentExtinct = this.ExtinctionDate != null;
            var wasReintroduced = this.ReintroductionDate != null || this.CommonDate != null;
            var wasReintroducedByOwner = wasReintroduced && ownerValueName == this.ReintroductionFaction;
            var isNowCommon = this.CommonDate == null ? false : currentDate >= this.CommonDate;

            // Careful... if it has no reintro details, we're inferring it was made common without having reintro, hence order of testing is important...
            if (wentExtinct && currentDate >= this.ExtinctionDate)
            {
                if (isNowCommon)
                {
                    return true;
                }
                else if (wasReintroduced && currentDate >= this.ReintroductionDate && wasReintroducedByOwner)
                {
                    return true;
                }
                else
                {
                    // Went extinct, and was either never reintroduced by the owner, or never went common (Skipped reintro)
                    return false;
                }
            }
            else
            {
                // Check prototype and production parameters...
                if (this.PrototypeDate == null && this.ProductionDate == null)
                {
                    return this.CommonDate == null || isNowCommon;
                }
                else if (PrototypeDate != null)
                {
                    if (currentDate < PrototypeDate)
                    {
                        // Before it was ever prototyped...
                        return false;
                    }
                    else
                    {
                        if (ProductionDate != null && currentDate >= ProductionDate)
                        {
                            // It was prototyped and subsequently produced, or it went common 
                            return wasProducedByOwner || isNowCommon;
                        }
                        else
                        {
                            return wasPrototypedByOwner;
                        }
                    }
                }
            }

            return false;
        }
    }
}