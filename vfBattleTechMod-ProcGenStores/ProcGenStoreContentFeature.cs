namespace vfBattleTechMod_ProcGenStores
{
    using System.Collections.Generic;

    using BattleTech;

    using Harmony;

    using Newtonsoft.Json;

    using vfBattleTechMod_Core.Mods.Interfaces;
    using vfBattleTechMod_Core.Utils.Interfaces;

    internal class ProcGenStoreContentFeature : IModFeature
    {
        public bool Enabled { get; set; }

        public string Name { get; set; } = "Procedurally Generate Store Contents";

        private static ILogger Logger { get; set; }

        private string Directory { get; set; }

        private ProcGenStoreContentFeatureSettings Settings { get; set; }

        public static bool PrefixRefreshShop(Shop __instance, SimGameState ___Sim)
        {
            Logger.Debug("Injecting custom shop inventory...");
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
            Logger.Debug("Injecting custom shop inventory complete.");
            return false;
        }

        public void Initialize(HarmonyInstance harmonyInstance, string settings, ILogger logger, string directory)
        {
            Logger = logger;
            this.Directory = directory;
            Logger.Debug($"Initializing [{this.Name}] with settings [{settings}]...");
            this.Settings = JsonConvert.DeserializeObject<ProcGenStoreContentFeatureSettings>(settings);

            var target = typeof(Shop).GetMethod("RefreshShop");
            var prefix = typeof(ProcGenStoreContentFeature).GetMethod("PrefixRefreshShop");
            harmonyInstance.Patch(target, new HarmonyMethod(prefix), null, null);

            Logger.Debug($"Feature [{this.Name}] initialized with setting [{this.Settings.test}]");
        }
    }
}