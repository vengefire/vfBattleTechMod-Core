using System.Collections.Generic;
using Harmony;
using HBS.Logging;
using vfBattleTechMod_Core.Helpers;
using vfBattleTechMod_Core.Mods.Interfaces;
using vfBattleTechMod_Core.Utils.Interfaces;
using Logger = vfBattleTechMod_Core.Utils.Logger;

namespace vfBattleTechMod_Core.Mods.BaseImpl
{
    public abstract class ModBase<TSettingsClass> : IBattleTechMod
    {
        protected ModBase(HarmonyInstance harmonyInstance, string directory, string settings, string name, List<IModFeature> modFeatures)
        {
            this.InitialiseLogging(directory);
            this.Name = name;
            this.Directory = directory;
            this.ModFeatures = modFeatures;
            this.Logger.Debug($"Booting up [{this.Name}] at[{this.Directory}], with settings \r\n{settings}]...");
            this.Initialize(harmonyInstance, settings);
        }

        private void InitialiseLogging(string directory)
        {
            var hbsLogger = this.GetHbsLogger();
            this.Logger = new Logger(hbsLogger, directory, this.Name);
        }

        private ILog GetHbsLogger()
        {
            var hbsLogger = HBS.Logging.Logger.GetLogger(this.Name, LogLevel.Debug);
            return hbsLogger;
        }

        public ILogger Logger { get; set; }

        public List<IModFeature> ModFeatures { get; }

        public string Name { get; set; }

        public string Directory { get; set; }

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
