using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using vfBattleTechMod_Core.Helpers;
using vfBattleTechMod_Core.Utils;
using vfBattleTechMod_Core.Utils.Interfaces;
using vfBattleTechMod_ProcGenStores.Mod.Features.ProcGenStoresContent;
using vfBattleTechMod_ProcGenStores.Mod.Features.ProcGenStoresContent.Logic;

namespace vfBattleTechMod_ProcGenStores_Test
{
    [TestFixture]
    public class StoreServiceTests
    {
        private string sourceFile;
        private JObject settings;
        private ProcGenStoreContentFeatureSettings procGenSettings;
        private readonly ILogger logger = new NullLogger();

        [OneTimeSetUp]
        public void Init()
        {
            this.sourceFile = TestContext.CurrentContext.TestDirectory + @"/res/xlrp-store-content.xlsx";
            this.settings = JsonHelpers.DeserializeFile(TestContext.CurrentContext.TestDirectory + @"/res/test-settings.json");
            this.procGenSettings = this.settings["Procedurally Generate Store Contents"].ToObject<ProcGenStoreContentFeatureSettings>();
        }

        [Test]
        public void TestStoreItemPotentialsCorrectlyExcludeFutureTechForEarlyDate()
        {
            var storeItemTypes = new List<BattleTechResourceType> { BattleTechResourceType.HeatSinkDef };
            var date = new DateTime(3025, 1, 1);
            var storeItemService = new StoreItemService(this.sourceFile, this.procGenSettings.RarityBrackets, storeItemTypes, this.logger);
            var potentialInventory = storeItemService.IdentifyPotentialInventoryItems(Shop.ShopType.System, "vengefire", date, this.procGenSettings);
            Assert.IsFalse(potentialInventory.Any(item => item.Id == "emod_engineslots_compact_center"));
        }

        [Test]
        public void TestStoreItemPotentialsCorrectlyIncludesFutureTechForLateDate()
        {
            var storeItemTypes = new List<BattleTechResourceType> { BattleTechResourceType.HeatSinkDef };
            var date = new DateTime(3100, 1, 1);
            var storeItemService = new StoreItemService(this.sourceFile, this.procGenSettings.RarityBrackets, storeItemTypes, this.logger);
            var potentialInventory = storeItemService.IdentifyPotentialInventoryItems(Shop.ShopType.System, "vengefire", date, this.procGenSettings);
            Assert.IsTrue(potentialInventory.Any(item => item.Id == "emod_engineslots_compact_center"));
        }

        [Test]
        public void TestStoreItemServiceBasicProcessing()
        {
            var storeItemTypes = new List<BattleTechResourceType> { BattleTechResourceType.HeatSinkDef };
            var date = new DateTime(3025, 1, 1);
            var storeItemService = new StoreItemService(this.sourceFile, this.procGenSettings.RarityBrackets, storeItemTypes, this.logger);
            var storeInventory = storeItemService.GenerateItemsForStore(Shop.ShopType.System, "Planet Vengeance", "vengefire", date, this.procGenSettings.PlanetTagModifiers, this.procGenSettings);
        }
    }
}