using System;
using System.Collections.Generic;
using BattleTech;
using HBS.Collections;

namespace vfBattleTechMod_ProcGenStores.Mod.Features.ProcGenStoresContent.Logic
{
    public class ProcGenStoreItem
    {
        public ProcGenStoreItem(BattleTechResourceType type, string id, DateTime? appearanceDate, TagSet tagSet,
            ProcGenStoreContentFeatureSettings.RarityBracket rarityBracket, List<string> requiredTags,
            List<string> restrictedTags)
        {
            Type = type;
            Id = id;
            MinAppearanceDate = appearanceDate;
            TagSet = tagSet;
            RarityBracket = rarityBracket;
            RequiredTags = requiredTags;
            RestrictedTags = restrictedTags;
        }

        public BattleTechResourceType Type { get; set; }
        public string Id { get; set; }
        public TagSet TagSet { get; set; }
        public ProcGenStoreContentFeatureSettings.RarityBracket RarityBracket { get; set; }

        public List<string> RequiredTags { get; set; } = new List<string>();

        public List<string> RestrictedTags { get; set; } = new List<string>();
        public DateTime? MinAppearanceDate { get; set; }
    }
}