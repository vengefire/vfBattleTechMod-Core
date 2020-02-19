using System.Collections.Generic;
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
                    0),
                new ModPatchDirective(
                    AccessTools.Property(typeof(AbstractActor), nameof(AbstractActor.IsTargetMarked)).GetGetMethod(false),
                    typeof(ProcGenStoreContentFeature).GetMethod(nameof(ProcGenStoreContentFeature.GetIsTargetMarked)),
                    null,
                    null,
                    0)
            };

        public override string Name => "Procedurally Generate Store Contents";

        public static bool PrefixRefreshShop(Shop __instance, SimGameState ___Sim)
        {
            ModFeatureBase<ProcGenStoreContentFeatureSettings>.Logger.Debug("Injecting custom shop inventory...");
            __instance.Clear();
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

        public override void OnInitializeComplete()
        {
        }

        public static bool GetIsTargetMarked(AbstractActor __instance, ref bool __result)
        {
            // Logger.Debug($"IsTargetMarked?");
            __result = false;
            var isMarked = __instance.Combat.EffectManager.GetAllEffectsTargeting(__instance).FindAll(
                x =>
                {
                    // Logger.Debug($"{JsonConvert.SerializeObject(x.EffectData)}");
                    if (x.EffectData?.Description?.Id == null)
                    {
                        ModFeatureBase<ProcGenStoreContentFeatureSettings>.Logger.Debug($"Dodgy EffectData = {JsonConvert.SerializeObject(x.EffectData)}");
                        return false;
                    }

                    if (x.EffectData.Description.Id.Contains("StatusEffect-TAG") || x.EffectData.Description.Id.Contains("StatusEffect-NARC"))
                    {
                        return true;
                    }

                    return false;
                }).Any();

            // Logger.Debug($"IsTargetMarked = [{__result}]");
            return false;
        }
    }
}