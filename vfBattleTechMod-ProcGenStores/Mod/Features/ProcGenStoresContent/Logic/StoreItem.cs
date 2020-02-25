using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<string> RequiredPlanetTags { get; set; } = new List<string>();

        public List<string> RestrictedPlanetTags { get; set; } = new List<string>();

        public int Quantity { get; set; } = 0;

        public (bool result, int bracketBonus) IsValidForAppearance(DateTime currentDate, string ownerValueName,
            Shop.ShopType shopType,
            List<string> planetTags,
            ProcGenStoreContentFeatureSettings settings)
        {
            // Check tags...
            if (RequiredPlanetTags.Any())
            {
                // Check all required tags are present...
                // Unless we're populating a black market, and they're configured to circumvent required restrictions...
                if (RequiredPlanetTags.Except(planetTags).Any() && !(shopType == Shop.ShopType.BlackMarket && settings.BlackMarketSettings.CircumventRequiredPlanetTags))
                {
                    return (false, 0);
                }
            }

            if (RestrictedPlanetTags.Any() && planetTags.Any(s => RestrictedPlanetTags.Contains(s)) && !(shopType == Shop.ShopType.BlackMarket && settings.BlackMarketSettings.CircumventRestrictedPlanetTags))
            {
                return (false, 0);
            }

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

                if (wasReintroduced && currentDate >= ReintroductionDate)
                {
                    if (wasReintroducedByOwner)
                    {
                        return (true, 2);
                    }

                    if (shopType == Shop.ShopType.BlackMarket && settings.BlackMarketSettings.CircumventFactionRestrictions)
                    {
                        return (true, 0);
                    }
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

                // Never went extinct, has no prototype or production date. If it's now common, return true with a bracket roll bonus, else fall through to false.
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
                        // Not common yet, not produced by owner, but black market stores can circumvent that requirement...
                        if (!wasProducedByOwner && shopType == Shop.ShopType.BlackMarket && settings.BlackMarketSettings.CircumventFactionRestrictions)
                        {
                            return (true, 0);
                        }

                        // Not common yet, but produced by owner...
                        return (wasProducedByOwner, 2);
                    }

                    // It's now common
                    return (true, 2);
                }

                // We're in the prototype period, the item was not produced by the owner, but blackmarkets can circumvent that (maybe)...
                if (!wasPrototypedByOwner && shopType == Shop.ShopType.BlackMarket && settings.BlackMarketSettings.CircumventFactionRestrictions)
                {
                    return (true, 0);
                }

                // We're in the prototype period, return true if the owner of the store is the prototype agent...
                return (wasPrototypedByOwner, 2);
            }

            // No valid periods or matching owners...
            return (false, 0);
        }
    }
}