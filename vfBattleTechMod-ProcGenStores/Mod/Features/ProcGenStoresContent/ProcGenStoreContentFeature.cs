using System.Collections.Generic;
using System.IO;
using System.Linq;
using BattleTech;
using Harmony;
using Newtonsoft.Json;
using vfBattleTechMod_Core.Mods.BaseImpl;
using vfBattleTechMod_Core.Mods.Interfaces;

namespace vfBattleTechMod_ProcGenStores.Mod.Features.ProcGenStoresContent
{
    internal class ProcGenStoreContentFeature : ModFeatureBase<ProcGenStoreContentFeatureSettings>
    {
        public ProcGenStoreContentFeature()
            : base(ProcGenStoreContentFeature.GetPatchDirectives)
        {
        }

        public static List<IModPatchDirective> GetPatchDirectives =>
            new List<IModPatchDirective>
            {
                new ModPatchDirective(
                    typeof(Shop).GetMethod("RefreshShop"),
                    typeof(ProcGenStoreContentFeature).GetMethod("PrefixRefreshShop"),
                    null,
                    null,
                    0)
            };

        public override string Name => "Procedurally Generate Store Contents";

        public static bool PrefixRefreshShop(Shop __instance, SimGameState ___Sim, StarSystem ___system)
        {
            ModFeatureBase<ProcGenStoreContentFeatureSettings>.Logger.Debug("Injecting custom shop inventory...");
            __instance.Clear();

            var owningSystemTags = ___system.Tags.ToList();
            var shopType = __instance.ThisShopType;
            var currentDate = ___Sim.CurrentDate;
            var owningFaction = ___system.OwnerValue;

            var result = new ItemCollectionResult
            {
                GUID = ___Sim.GenerateSimGameUID(),
                callback = null,
                itemCollectionID = "vf-Fake",
                items = new List<ShopDefItem>
                {
                    new ShopDefItem(
                        "Gear_HeatSink_Generic_Standard",
                        ShopItemType.HeatSink,
                        0.5f,
                        666,
                        false,
                        false,
                        666)
                },
                parentGUID = null,
                pendingCount = 0
            };
            ___Sim.ItemCollectionResultGen.InsertShopDefItem(result.items, __instance.ActiveInventory);
            ModFeatureBase<ProcGenStoreContentFeatureSettings>.Logger.Debug("Injecting custom shop inventory complete.");
            return false;
        }

        protected override bool ValidateSettings()
        {
            if (!File.Exists(this.StoreItemSourceFilePath()))
            {
                Logger.Debug($"{this.Name} failed settings validation, store items file [{this.StoreItemSourceFilePath()}] does not exist.");
                return false;
            }

            return true;
        }

        public string StoreItemSourceFilePath() => Path.Combine(this.Directory, this.Settings.StoreItemSourceFile);

        public override void OnInitializeComplete()
        {
        }
    }
}