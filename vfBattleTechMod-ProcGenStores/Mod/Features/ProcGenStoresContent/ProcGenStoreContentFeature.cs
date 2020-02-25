using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BattleTech;
using Newtonsoft.Json;
using vfBattleTechMod_Core.Mods.BaseImpl;
using vfBattleTechMod_Core.Mods.Interfaces;
using vfBattleTechMod_ProcGenStores.Mod.Features.ProcGenStoresContent.Logic;

namespace vfBattleTechMod_ProcGenStores.Mod.Features.ProcGenStoresContent
{
    public class ProcGenStoreContentFeature : ModFeatureBase<ProcGenStoreContentFeatureSettings>
    {
        private new static ProcGenStoreContentFeature Myself;
        private DateTime _lastGenDateTime;

        public Dictionary<BattleTechResourceType, ShopItemType> dictResourceTypeToShopitemType =
            new Dictionary<BattleTechResourceType, ShopItemType>
            {
                {BattleTechResourceType.AmmunitionBoxDef, ShopItemType.AmmunitionBox},
                {BattleTechResourceType.UpgradeDef, ShopItemType.Upgrade},
                {BattleTechResourceType.HeatSinkDef, ShopItemType.HeatSink},
                {BattleTechResourceType.JumpJetDef, ShopItemType.JumpJet},
                {BattleTechResourceType.WeaponDef, ShopItemType.Weapon},
                {BattleTechResourceType.MechDef, ShopItemType.Mech}
            };

        public ProcGenStoreContentFeature()
            : base(ProcGenStoreContentFeature.GetPatchDirectives)
        {
            ProcGenStoreContentFeature.Myself = this;
        }

        protected StoreItemService StoreItemService { get; set; }

        public static List<IModPatchDirective> GetPatchDirectives =>
            new List<IModPatchDirective>
            {
                new ModPatchDirective(
                    typeof(Shop).GetMethod("RefreshShop"),
                    typeof(ProcGenStoreContentFeature).GetMethod("PrefixRefreshShop"),
                    null,
                    null,
                    0)
                /*new ModPatchDirective(
                    typeof(Shop).GetMethod("Initialize"),
                    typeof(ProcGenStoreContentFeature).GetMethod("PrefixInitializeShop"),
                    null,
                    null,
                    0)*/
            };

        public override string Name => "Procedurally Generate Store Contents";

        public static List<BattleTechResourceType> BattleTechStoreResourceTypes => new List<BattleTechResourceType>
        {
            BattleTechResourceType.AmmunitionBoxDef, BattleTechResourceType.UpgradeDef,
            BattleTechResourceType.HeatSinkDef, BattleTechResourceType.JumpJetDef, BattleTechResourceType.WeaponDef
        };

        public static bool PrefixInitializeShop(Shop __instance, ref List<string> collections)
        {
            ModFeatureBase<ProcGenStoreContentFeatureSettings>.Logger.Debug("Noping out of Shop::Initialize.");
            collections = null;
            ModFeatureBase<ProcGenStoreContentFeatureSettings>.Logger.Debug("Noped out of Shop::Initialize.");
            return true;
        }

        public static bool PrefixRefreshShop(Shop __instance, SimGameState ___Sim, StarSystem ___system)
        {
            if (ProcGenStoreContentFeature.Myself._lastGenDateTime != null)
            {
                var difference = DateTime.Now - ProcGenStoreContentFeature.Myself._lastGenDateTime;
                if (difference.TotalMinutes < 1)
                {
                    ModFeatureBase<ProcGenStoreContentFeatureSettings>.Logger.Debug("Shop refresh request < 1 minute ago, skipping...");
                    return false;
                }
            }

            ProcGenStoreContentFeature.Myself._lastGenDateTime = DateTime.Now;

            ModFeatureBase<ProcGenStoreContentFeatureSettings>.Logger.Debug("Injecting custom shop inventory...");
            __instance.Clear();

            var owningSystemTags = ___system.Tags.ToList();
            var shopType = __instance.ThisShopType;
            var currentDate = ___Sim.CurrentDate;
            var owningFaction = ___system.OwnerValue;

            var planetTagModifiers = ModFeatureBase<ProcGenStoreContentFeatureSettings>.Myself.Settings
                .PlanetTagModifiers.Where(modifier => owningSystemTags.Contains(modifier.Tag)).ToList();

            var storeItems = ProcGenStoreContentFeature.Myself.StoreItemService.GenerateItemsForStore(shopType, ___system.Name, owningFaction.Name,
                currentDate, owningSystemTags, planetTagModifiers, ProcGenStoreContentFeature.Myself.Settings);
            var shopDefItems = storeItems.Select(item =>
            {
                return new ShopDefItem(item.Id, ProcGenStoreContentFeature.Myself.dictResourceTypeToShopitemType[item.Type], 0, item.Quantity,
                    item.Quantity == -1, false, 0);
            }).ToList();

            ModFeatureBase<ProcGenStoreContentFeatureSettings>.Logger.Debug($"ShopDefItems = [\r\n{JsonConvert.SerializeObject(shopDefItems, Formatting.Indented)}]");

            var result = new ItemCollectionResult
            {
                GUID = ___Sim.GenerateSimGameUID(),
                callback = null,
                itemCollectionID = "vf-Fake",
                items = shopDefItems,
                parentGUID = null,
                pendingCount = 0
            };

            ___Sim.ItemCollectionResultGen.InsertShopDefItem(result.items, __instance.ActiveInventory);
            ModFeatureBase<ProcGenStoreContentFeatureSettings>.Logger.Debug("Injecting custom shop inventory complete.");
            return false;
        }

        protected override bool ValidateSettings()
        {
            if (!File.Exists(StoreItemSourceFilePath()))
            {
                ModFeatureBase<ProcGenStoreContentFeatureSettings>.Logger.Debug(
                    $"{Name} failed settings validation, store items file [{StoreItemSourceFilePath()}] does not exist.");
                return false;
            }

            return true;
        }

        public string StoreItemSourceFilePath()
        {
            return Path.Combine(Directory, Settings.StoreItemSourceFile);
        }

        public override void OnInitializeComplete()
        {
            StoreItemService = new StoreItemService(Path.Combine(Directory, Settings.StoreItemSourceFile),
                Settings.RarityBrackets, ProcGenStoreContentFeature.BattleTechStoreResourceTypes, ModFeatureBase<ProcGenStoreContentFeatureSettings>.Logger);
        }
    }
}