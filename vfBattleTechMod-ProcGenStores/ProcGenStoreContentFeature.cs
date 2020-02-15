namespace vfBattleTechMod_ProcGenStores
{
    using Harmony;

    using Newtonsoft.Json;

    using vfBattleTechMod_Core.Mods.Interfaces;
    using vfBattleTechMod_Core.Utils.Interfaces;

    internal class ProcGenStoreContentFeature : IModFeature
    {
        public bool Enabled { get; set; }

        public string Name { get; set; } = "Procedurally Generate Store Contents";

        private string Directory { get; set; }

        private ILogger Logger { get; set; }

        private ProcGenStoreContentFeatureSettings Settings { get; set; }

        public void Initialize(HarmonyInstance harmonyInstance, string settings, ILogger logger, string directory)
        {
            this.Logger = logger;
            this.Directory = directory;
            this.Logger.Debug($"Initializing [{this.Name}] with settings [{settings}]...");
            this.Settings = JsonConvert.DeserializeObject<ProcGenStoreContentFeatureSettings>(settings);
            this.Logger.Debug($"Feature [{this.Name}] initialized with setting [{this.Settings.test}]");
        }
    }
}