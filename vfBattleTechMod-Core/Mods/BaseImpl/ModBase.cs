namespace vfBattleTechMod_Core.Mods.BaseImpl
{
    using System.Collections.Generic;

    using Harmony;

    using HBS.Logging;

    using vfBattleTechMod_Core.Helpers;
    using vfBattleTechMod_Core.Mods.Interfaces;
    using vfBattleTechMod_Core.Utils.Interfaces;

    using Logger = vfBattleTechMod_Core.Utils.Logger;

    public abstract class ModBase : IBattleTechMod
    {
        protected ModBase(
            HarmonyInstance harmonyInstance,
            string directory,
            string settings,
            string name,
            List<IModFeature<IModFeatureSettings>> modFeatures)
        {
            this.InitialiseLogging(directory);
            this.Name = name;
            this.Directory = directory;
            this.ModFeatures = modFeatures;
            this.Logger.Debug($"Booting up [{this.Name}] at[{this.Directory}], with settings \r\n{settings}]...");
            this.Initialize(harmonyInstance, settings);
        }

        public string Directory { get; }

        public ILogger Logger { get; private set; }

        public List<IModFeature<IModFeatureSettings>> ModFeatures { get; }

        public string Name { get; }

        private ILog GetHbsLogger()
        {
            var hbsLogger = HBS.Logging.Logger.GetLogger(this.Name, LogLevel.Debug);
            return hbsLogger;
        }

        private void InitialiseLogging(string directory)
        {
            var hbsLogger = this.GetHbsLogger();
            hbsLogger.LogDebug($"Initializing logging for [{this.Name}]");
            this.Logger = new Logger(hbsLogger, directory, this.Name);
        }

        private void Initialize(HarmonyInstance harmonyInstance, string settings)
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
    }
}