using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using BattleTech;
using Harmony;
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

        public Dictionary<BattleTechResourceType, ShopItemType> dictResourceTypeToShopitemType = new Dictionary<BattleTechResourceType, ShopItemType>
        {
            { BattleTechResourceType.AmmunitionBoxDef, ShopItemType.AmmunitionBox },
            { BattleTechResourceType.UpgradeDef, ShopItemType.Upgrade },
            { BattleTechResourceType.HeatSinkDef, ShopItemType.HeatSink },
            { BattleTechResourceType.JumpJetDef, ShopItemType.JumpJet },
            { BattleTechResourceType.WeaponDef, ShopItemType.Weapon },
            { BattleTechResourceType.MechDef, ShopItemType.Mech }
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
                    0),
                /*new ModPatchDirective(
                    typeof(Shop).GetMethod("Initialize"),
                    typeof(ProcGenStoreContentFeature).GetMethod("PrefixInitializeShop"),
                    null,
                    null,
                    0)*/
            };

        public override string Name => "Procedurally Generate Store Contents";

        public static List<BattleTechResourceType> BattleTechStoreResourceTypes => new List<BattleTechResourceType> { BattleTechResourceType.AmmunitionBoxDef, BattleTechResourceType.UpgradeDef, BattleTechResourceType.HeatSinkDef, BattleTechResourceType.JumpJetDef, BattleTechResourceType.WeaponDef };

        public static bool PrefixInitializeShop(Shop __instance, ref List<string> collections)
        {
            Logger.Debug($"Noping out of Shop::Initialize.");
            collections = null;
            Logger.Debug($"Noped out of Shop::Initialize.");
            return true;
        }

        public static bool PrefixRefreshShop(Shop __instance, SimGameState ___Sim, StarSystem ___system)
        {
            if (Myself._lastGenDateTime != null)
            {
                var difference = DateTime.Now - Myself._lastGenDateTime;
                if (difference.TotalMinutes < 1)
                {
                    Logger.Debug($"Shop refresh request < 1 minute ago, skipping...");
                    return false;
                }
            }

            Myself._lastGenDateTime = DateTime.Now;

            ModFeatureBase<ProcGenStoreContentFeatureSettings>.Logger.Debug("Injecting custom shop inventory...");
            __instance.Clear();

            var owningSystemTags = ___system.Tags.ToList();
            var shopType = __instance.ThisShopType;
            var currentDate = ___Sim.CurrentDate;
            var owningFaction = ___system.OwnerValue;

            var planetTagModifiers = ModFeatureBase<ProcGenStoreContentFeatureSettings>.Myself.Settings.PlanetTagModifiers.Where(modifier => owningSystemTags.Contains(modifier.Tag)).ToList();

            var storeItems = ProcGenStoreContentFeature.Myself.StoreItemService.GenerateItemsForStore(shopType, ___system.Name, owningFaction.Name, currentDate, planetTagModifiers, ProcGenStoreContentFeature.Myself.Settings);
            var shopDefItems = storeItems.Select(item => { return new ShopDefItem(item.Id, ProcGenStoreContentFeature.Myself.dictResourceTypeToShopitemType[item.Type], 0, item.Quantity, item.Quantity == -1, false, 0); }).ToList();

            Logger.Debug($"ShopDefItems = [\r\n{JsonConvert.SerializeObject(shopDefItems, Formatting.Indented)}]");

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
            if (!File.Exists(this.StoreItemSourceFilePath()))
            {
                ModFeatureBase<ProcGenStoreContentFeatureSettings>.Logger.Debug($"{this.Name} failed settings validation, store items file [{this.StoreItemSourceFilePath()}] does not exist.");
                return false;
            }

            return true;
        }

        public string StoreItemSourceFilePath()
        {
            return Path.Combine(this.Directory, this.Settings.StoreItemSourceFile);
        }

        public override void OnInitializeComplete()
        {
            this.StoreItemService = new StoreItemService(Path.Combine(this.Directory, this.Settings.StoreItemSourceFile), this.Settings.RarityBrackets, ProcGenStoreContentFeature.BattleTechStoreResourceTypes, ModFeatureBase<ProcGenStoreContentFeatureSettings>.Logger);
        }
    }
}