using System.Collections.Generic;
using vfBattleTechMod_Core.Mods.BaseImpl;

namespace vfBattleTechMod_ProcGenStores.Mod.Features.ProcGenStoresContent
{
    public class ProcGenStoreContentFeatureSettings : ModFeatureSettingsBase
    {
        public string StoreItemSourceFile { get; set; }
        public bool CascadeRollsOnFail { get; set; } = true;

        public List<RarityBracket> RarityBrackets { get; set; } = new List<RarityBracket>() { new RarityBracket() };
        public List<PlanetTagModifier> PlanetTagModifiers { get; set; } = new List<PlanetTagModifier>() { new PlanetTagModifier() };

        public class RarityBracket
        {
            public string Name { get; set; } = string.Empty;

            public float ChanceToAppear { get; set; } = -1;

            public string Description { get; set; } = string.Empty;

            public QuantityBracket QuantityBracket { get; set; } = new QuantityBracket();
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

            public float ChanceModifier { get; set; } = 1;

            public float QuantityModifier { get; set; } = 1;

            public string Description { get; set; } = string.Empty;

            public bool AppliesToStock { get; set; } = true;
            public bool AppliesToLosTech { get; set; } = false;

            public bool AppliesToFaction { get; set; } = false;
        }
    }
}