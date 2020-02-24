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
        private readonly List<string> blankSystemTagList = new List<string>();

        [OneTimeSetUp]
        public void Init()
        {
            sourceFile = TestContext.CurrentContext.TestDirectory + @"/res/test-xlrp-store-content.xlsx";
            settings = JsonHelpers.DeserializeFile(
                TestContext.CurrentContext.TestDirectory + @"/res/test-settings.json");
            procGenSettings = settings["Procedurally Generate Store Contents"]
                .ToObject<ProcGenStoreContentFeatureSettings>();
        }

        [Test]
        public void TestStoreItemPotentialsCorrectlyExcludeFutureTechForEarlyDate()
        {
            var storeItemTypes = new List<BattleTechResourceType> {BattleTechResourceType.HeatSinkDef};
            var date = new DateTime(3025, 1, 1);
            var storeItemService =
                new StoreItemService(sourceFile, procGenSettings.RarityBrackets, storeItemTypes, logger);
            var potentialInventory =
                storeItemService.IdentifyPotentialInventoryItems(Shop.ShopType.System, "vengefire", blankSystemTagList,  date,
                    procGenSettings);
            Assert.IsFalse(potentialInventory.Any(item => item.StoreItem.Id == "emod_engineslots_compact_center"));
        }

        [Test]
        public void TestStoreItemPotentialsCorrectlyExcludesNaTech()
        {
            var storeItemTypes = new List<BattleTechResourceType> {BattleTechResourceType.HeatSinkDef};
            var date = new DateTime(3100, 1, 1);
            var storeItemService =
                new StoreItemService(sourceFile, procGenSettings.RarityBrackets, storeItemTypes, logger);
            var potentialInventory =
                storeItemService.IdentifyPotentialInventoryItems(Shop.ShopType.System, "TH", blankSystemTagList, date, procGenSettings);
            Assert.IsFalse(potentialInventory.Any(item => item.StoreItem.Id == "HeatSink_Template"));
        }

        [Test]
        public void TestStoreItemPotentialsCorrectlyExcludesPrototypeTechForLateDateAndFaction()
        {
            var storeItemTypes = new List<BattleTechResourceType> {BattleTechResourceType.HeatSinkDef};
            var date = new DateTime(3036, 1, 1);
            var storeItemService =
                new StoreItemService(sourceFile, procGenSettings.RarityBrackets, storeItemTypes, logger);
            var potentialInventory =
                storeItemService.IdentifyPotentialInventoryItems(Shop.ShopType.System, "TH", blankSystemTagList, date, procGenSettings);
            Assert.IsFalse(potentialInventory.Any(item => item.StoreItem.Id == "emod_engineslots_xl_center"));
        }
        
        [Test]
        public void TestStoreItemPotentialsCorrectlyExcludesPrototypeTechForLateDateAndFactionForBlackMarket()
        {
            var storeItemTypes = new List<BattleTechResourceType> {BattleTechResourceType.HeatSinkDef};
            var date = new DateTime(3036, 1, 1);
            var storeItemService =
                new StoreItemService(sourceFile, procGenSettings.RarityBrackets, storeItemTypes, logger);
            var potentialInventory =
                storeItemService.IdentifyPotentialInventoryItems(Shop.ShopType.BlackMarket, "TH", blankSystemTagList, date, procGenSettings);
            Assert.IsFalse(potentialInventory.Any(item => item.StoreItem.Id == "emod_engineslots_xl_center"));
        }
        
        [Test]
        public void TestStoreItemPotentialsCorrectlyIncludesPrototypeTechForLateDateAndFactionForBlackMarket()
        {
            var storeItemTypes = new List<BattleTechResourceType> {BattleTechResourceType.HeatSinkDef};
            var date = new DateTime(3036, 1, 1);
            var storeItemService =
                new StoreItemService(sourceFile, procGenSettings.RarityBrackets, storeItemTypes, logger);
            procGenSettings.BlackMarketSettings.CircumventFactionRestrictions = true;
            var potentialInventory =
                storeItemService.IdentifyPotentialInventoryItems(Shop.ShopType.BlackMarket, "TH", blankSystemTagList, date, procGenSettings);
            Assert.IsFalse(potentialInventory.Any(item => item.StoreItem.Id == "emod_engineslots_xl_center"));
        }
        
        [Test]
        public void TestStoreItemPotentialsCorrectlyExcludesProductionTechForLateDateAndFactionForBlackMarket()
        {
            var storeItemTypes = new List<BattleTechResourceType> {BattleTechResourceType.HeatSinkDef};
            var date = new DateTime(3070, 1, 1);
            var storeItemService =
                new StoreItemService(sourceFile, procGenSettings.RarityBrackets, storeItemTypes, logger);
            var potentialInventory =
                storeItemService.IdentifyPotentialInventoryItems(Shop.ShopType.BlackMarket, "TH", blankSystemTagList, date, procGenSettings);
            Assert.IsFalse(potentialInventory.Any(item => item.StoreItem.Id == "emod_engineslots_compact_center"));
        }
        
        [Test]
        public void TestStoreItemPotentialsCorrectlyIncludesProductionTechForLateDateAndFactionForBlackMarket()
        {
            var storeItemTypes = new List<BattleTechResourceType> {BattleTechResourceType.HeatSinkDef};
            var date = new DateTime(3070, 1, 1);
            var storeItemService =
                new StoreItemService(sourceFile, procGenSettings.RarityBrackets, storeItemTypes, logger);
            procGenSettings.BlackMarketSettings.CircumventFactionRestrictions = true;
            var potentialInventory =
                storeItemService.IdentifyPotentialInventoryItems(Shop.ShopType.BlackMarket, "TH", blankSystemTagList, date, procGenSettings);
            Assert.IsFalse(potentialInventory.Any(item => item.StoreItem.Id == "emod_engineslots_compact_center"));
        }
        
        [Test]
        public void TestStoreItemPotentialsCorrectlyIncludesFutureTechForLateDate()
        {
            var storeItemTypes = new List<BattleTechResourceType> {BattleTechResourceType.HeatSinkDef};
            var date = new DateTime(3100, 1, 1);
            var storeItemService =
                new StoreItemService(sourceFile, procGenSettings.RarityBrackets, storeItemTypes, logger);
            var potentialInventory =
                storeItemService.IdentifyPotentialInventoryItems(Shop.ShopType.System, "vengefire", blankSystemTagList, date,
                    procGenSettings);
            Assert.IsTrue(potentialInventory.Any(item => item.StoreItem.Id == "emod_engineslots_compact_center"));
        }

        [Test]
        public void TestStoreItemPotentialsCorrectlyIncludesPrototypeTechForLateDateAndFaction()
        {
            var storeItemTypes = new List<BattleTechResourceType> {BattleTechResourceType.HeatSinkDef};
            var date = new DateTime(3036, 1, 1);
            var storeItemService =
                new StoreItemService(sourceFile, procGenSettings.RarityBrackets, storeItemTypes, logger);
            var potentialInventory =
                storeItemService.IdentifyPotentialInventoryItems(Shop.ShopType.System, "LC", blankSystemTagList, date, procGenSettings);
            Assert.IsTrue(potentialInventory.Any(item => item.StoreItem.Id == "emod_engineslots_xl_center"));
        }
        
        [Test]
        public void TestStoreItemPotentialsCorrectlyExcludesMissingRequiredTags()
        {
            var storeItemTypes = new List<BattleTechResourceType> {BattleTechResourceType.HeatSinkDef};
            var date = new DateTime(3036, 1, 1);
            var storeItemService =
                new StoreItemService(sourceFile, procGenSettings.RarityBrackets, storeItemTypes, logger);
            var potentialInventory =
                storeItemService.IdentifyPotentialInventoryItems(Shop.ShopType.System, "LC", new List<string>()
                {
                    "planet_test_vengefire"
                }, date, procGenSettings);
            Assert.IsFalse(potentialInventory.Any(item => item.StoreItem.Id == "emod_engine_9000"));
        }
        
        [Test]
        public void TestStoreItemPotentialsCorrectlyExcludesSatisfiedRestrictedTags()
        {
            var storeItemTypes = new List<BattleTechResourceType> {BattleTechResourceType.HeatSinkDef};
            var date = new DateTime(3036, 1, 1);
            var storeItemService =
                new StoreItemService(sourceFile, procGenSettings.RarityBrackets, storeItemTypes, logger);
            var potentialInventory =
                storeItemService.IdentifyPotentialInventoryItems(Shop.ShopType.System, "LC", new List<string>()
                {
                    "planet_test_vengefire",
                    "planet_test_zappo",
                    "planet_test_MVP",
                }, date, procGenSettings);
            Assert.IsFalse(potentialInventory.Any(item => item.StoreItem.Id == "emod_engine_9000"));
        }
        
        [Test]
        public void TestStoreItemPotentialsCorrectlyIncludeSatisfiedRequiredTags()
        {
            var storeItemTypes = new List<BattleTechResourceType> {BattleTechResourceType.HeatSinkDef};
            var date = new DateTime(3036, 1, 1);
            var storeItemService =
                new StoreItemService(sourceFile, procGenSettings.RarityBrackets, storeItemTypes, logger);
            var potentialInventory =
                storeItemService.IdentifyPotentialInventoryItems(Shop.ShopType.System, "LC", new List<string>()
                {
                    "planet_test_vengefire",
                    "planet_test_zappo"
                }, date, procGenSettings);
            Assert.IsTrue(potentialInventory.Any(item => item.StoreItem.Id == "emod_engine_9000"));
        }
        
        [Test]
        public void TestStoreItemPotentialsCorrectlyExcludesSatisfiedRestrictedTagsForBlackMarket()
        {
            var storeItemTypes = new List<BattleTechResourceType> {BattleTechResourceType.HeatSinkDef};
            var date = new DateTime(3036, 1, 1);
            var storeItemService =
                new StoreItemService(sourceFile, procGenSettings.RarityBrackets, storeItemTypes, logger);
            var potentialInventory =
                storeItemService.IdentifyPotentialInventoryItems(Shop.ShopType.BlackMarket, "LC", new List<string>()
                {
                    "planet_test_vengefire",
                    "planet_test_zappo",
                    "planet_test_MVP",
                }, date, procGenSettings);
            Assert.IsFalse(potentialInventory.Any(item => item.StoreItem.Id == "emod_engine_9000"));
        }
        
        [Test]
        public void TestStoreItemPotentialsCorrectlyExcludesMissingRequiredTagsForBlackMarket()
        {
            var storeItemTypes = new List<BattleTechResourceType> {BattleTechResourceType.HeatSinkDef};
            var date = new DateTime(3036, 1, 1);
            var storeItemService =
                new StoreItemService(sourceFile, procGenSettings.RarityBrackets, storeItemTypes, logger);
            var potentialInventory =
                storeItemService.IdentifyPotentialInventoryItems(Shop.ShopType.BlackMarket, "LC", new List<string>()
                {
                    "planet_test_vengefire"
                }, date, procGenSettings);
            Assert.IsFalse(potentialInventory.Any(item => item.StoreItem.Id == "emod_engine_9000"));
        }
        
        public void TestStoreItemPotentialsCorrectlyIncludesSatisfiedRestrictedTagsForBlackMarket()
        {
            var storeItemTypes = new List<BattleTechResourceType> {BattleTechResourceType.HeatSinkDef};
            var date = new DateTime(3036, 1, 1);
            var storeItemService =
                new StoreItemService(sourceFile, procGenSettings.RarityBrackets, storeItemTypes, logger);
            procGenSettings.BlackMarketSettings.CircumventRestrictedPlanetTags = true;
            var potentialInventory =
                storeItemService.IdentifyPotentialInventoryItems(Shop.ShopType.BlackMarket, "LC", new List<string>()
                {
                    "planet_test_vengefire",
                    "planet_test_zappo",
                    "planet_test_MVP",
                }, date, procGenSettings);
            Assert.IsFalse(potentialInventory.Any(item => item.StoreItem.Id == "emod_engine_9000"));
        }
        
        [Test]
        public void TestStoreItemPotentialsCorrectlyIncludesMissingRequiredTagsForBlackMarket()
        {
            var storeItemTypes = new List<BattleTechResourceType> {BattleTechResourceType.HeatSinkDef};
            var date = new DateTime(3036, 1, 1);
            var storeItemService =
                new StoreItemService(sourceFile, procGenSettings.RarityBrackets, storeItemTypes, logger);
            procGenSettings.BlackMarketSettings.CircumventRequiredPlanetTags = true;
            var potentialInventory =
                storeItemService.IdentifyPotentialInventoryItems(Shop.ShopType.BlackMarket, "LC", new List<string>()
                {
                    "planet_test_vengefire"
                }, date, procGenSettings);
            Assert.IsFalse(potentialInventory.Any(item => item.StoreItem.Id == "emod_engine_9000"));
        }

        [Test]
        public void TestStoreItemServiceBasicProcessing()
        {
            var storeItemTypes = ProcGenStoreContentFeature.BattleTechStoreResourceTypes;
            var date = new DateTime(3025, 1, 1);
            var storeItemService =
                new StoreItemService(sourceFile, procGenSettings.RarityBrackets, storeItemTypes, logger);
            var planetTags = new List<string> {"planet_pop_large", "planet_pop_small"};
            var planetModifiers = procGenSettings.PlanetTagModifiers
                .Where(modifier => planetTags.Contains(modifier.Tag)).ToList();
            var storeInventory = storeItemService.GenerateItemsForStore(Shop.ShopType.System, "Planet Vengeance",
                "vengefire", date, blankSystemTagList, planetModifiers, procGenSettings);
        }
    }
}