using BattleTech;
using HBS.Collections;

namespace vfBattleTechMod_ProcGenStores.Mod.Features.ProcGenStoresContent.Logic
{
    public class ProcGenStoreItem
    {
        public ProcGenStoreItem(BattleTechResourceType type, string id, TagSet tagSet)
        {
            Type = type;
            Id = id;
            TagSet = tagSet;
        }

        public BattleTechResourceType Type { get; set; }
        public string Id { get; set; }
        public TagSet TagSet { get; set; }
    }
}