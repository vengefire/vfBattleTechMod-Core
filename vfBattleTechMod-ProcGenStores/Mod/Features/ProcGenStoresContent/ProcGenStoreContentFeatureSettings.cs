using System.Collections.Generic;
using vfBattleTechMod_Core.Mods.BaseImpl;

namespace vfBattleTechMod_ProcGenStores.Mod.Features.ProcGenStoresContent
{
    public class ProcGenStoreContentFeatureSettings : ModFeatureSettingsBase
    {
        public string StoreItemSourceFile { get; set; } = string.Empty;

        public bool CascadeRollsOnFail { get; set; } = true;

        public bool UseAdditiveForModifiers { get; set; } = true;

        public string MaxItemRarityForCascadeQualification { get; set; } = "Uncommon";

        public BlackMarket BlackMarketSettings { get; set; } = new BlackMarket();

        public FactionMarket FactionMarketSettings { get; set; } = new FactionMarket();

        public List<RarityBracket> RarityBrackets { get; set; } = new List<RarityBracket> {new RarityBracket()};

        public List<PlanetTagModifier> PlanetTagModifiers { get; set; } =
            new List<PlanetTagModifier> {new PlanetTagModifier()};

        public class BlackMarket
        {
            public string BlackMarketMinBaseRarity { get; set; } = "VeryUncommon";

            public string BlackMarketMaxBaseRarity { get; set; } = "PracticallyExtinct";

            public double BlackMarketAppearanceModifier { get; set; } = 1.5;

            public double BlackMarketQuantityModifier { get; set; } = 0.5;

            public bool CircumventRequiredPlanetTags { get; set; } = false;

            public bool CircumventRestrictedPlanetTags { get; set; } = false;

            public bool CircumventFactionRestrictions { get; set; } = false;
        }

        public class FactionMarket
        {
            public string MinBaseRarity { get; set; } = "VeryUncommon";

            public string MaxBaseRarity { get; set; } = "PracticallyExtinct";

            public double AppearanceModifier { get; set; } = 1.5;

            public double QuantityModifier { get; set; } = 0.5;
        }

        public class RarityBracket
        {
            public string Name { get; set; } = string.Empty;

            public double ChanceToAppear { get; set; } = -1;

            public string Description { get; set; } = string.Empty;

            public QuantityBracket QuantityBracket { get; set; } = new QuantityBracket();

            public int Order { get; set; }
        }

        public class QuantityBracket
        {
            public string Name { get; set; } = string.Empty;

            public int LowCount { get; set; } = 0;

            public int HighCount { get; set; } = 0;
        }

        public class PlanetTagModifier
        {
            public string Tag { get; set; } = string.Empty;

            public double ChanceModifier { get; set; } = 1;

            public double QuantityModifier { get; set; } = 1;

            public string Description { get; set; } = string.Empty;

            public bool AppliesToStock { get; set; } = true;

            public bool AppliesToLosTech { get; set; } = false;

            public bool AppliesToFaction { get; set; } = false;
        }
    }
}