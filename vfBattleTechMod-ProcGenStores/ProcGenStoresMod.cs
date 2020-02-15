namespace vfBattleTechMod_ProcGenStores
{
    using System.Collections.Generic;

    using Harmony;

    using HBS.Logging;

    using vfBattleTechMod_Core.Helpers;
    using vfBattleTechMod_Core.Mods.Interfaces;
    using vfBattleTechMod_Core.Utils.Interfaces;

    using Logger = vfBattleTechMod_Core.Utils.Logger;

    internal class ProcGenStoresMod : IBattleTechMod
    {
        public ProcGenStoresMod(HarmonyInstance harmonyInstance, string directory, string settings)
        {
            this.InitialiseLogging(directory);
            this.Logger.Debug($"Booting up [{this.Name}] at[{directory}], with settings \r\n{settings}]...");
            this.Directory = directory;
            this.Initialize(harmonyInstance, settings);
        }

        public string Directory { get; set; }

        public ILogger Logger { get; set; }

        public List<IModFeature> ModFeatures { get; set; } = new List<IModFeature> { new ProcGenStoreContentFeature() };

        public string Name { get; set; } = @"vfProcGenStores";

        public void Initialize(HarmonyInstance harmonyInstance, string settings)
        {
            this.Logger.Debug("Initializing Features...");
            var jsonSettings = JsonHelpers.Deserialize(settings);
            this.ModFeatures.ForEach(
                feature => feature.Initialize(
                    harmonyInstance,
                    jsonSettings[feature.Name].ToString(),
                    this.Logger,
                    this.Directory));
            this.Logger.Debug("Feature initialization complete.");
        }

        private ILog GetHbsLogger()
        {
            var hbsLogger = HBS.Logging.Logger.GetLogger(this.Name, LogLevel.Debug);
            return hbsLogger;
        }

        private void InitialiseLogging(string directory)
        {
            var hbsLogger = this.GetHbsLogger();
            this.Logger = new Logger(hbsLogger, directory, this.Name);
        }
    }
}