using System.Collections.Generic;
using Harmony;
using HBS.Logging;
using vfBattleTechMod_Core.Helpers;
using vfBattleTechMod_Core.Mods.Interfaces;
using vfBattleTechMod_Core.Utils;
using vfBattleTechMod_Core.Utils.Interfaces;

namespace vfBattleTechMod_Core.Mods.BaseImpl
{
    public abstract class ModBase<TModSettings> : IBattleTechMod<TModSettings> where TModSettings : IModSettings, new()
    {
        protected ModBase(
            HarmonyInstance harmonyInstance,
            string directory,
            string settings,
            string name,
            List<IModFeature<IModFeatureSettings>> modFeatures)
        {
            this.Name = name;
            this.Directory = directory;
            this.ModFeatures = modFeatures;
            this.InitialiseLogging(directory);
            this.Logger.Debug($"Booting up [{this.Name}] at[{this.Directory}], with settings \r\n{settings}]...");
            this.Logger.Debug($"Default settings = [{this.GenerateDefaultModSettings()}]");
            this.Initialize(harmonyInstance, settings);
        }

        public string Directory { get; }

        public ILogger Logger { get; private set; }

        public List<IModFeature<IModFeatureSettings>> ModFeatures { get; }

        public string Name { get; }

        public string GenerateDefaultModSettings()
        {
            this.Logger.Debug($"Generating default settings...");
            var settingsList = new List<string>();
            settingsList.Add(new TModSettings().Serialize());
            this.ModFeatures.ForEach(feature => settingsList.Add(feature.DefaultSettings.Serialize()));
            return settingsList.Join();
        }

        public TModSettings ModSettings { get; private set; }

        private ILog GetHbsLogger()
        {
            var hbsLogger = HBS.Logging.Logger.GetLogger(this.Name, LogLevel.Debug);
            HBS.Logging.Logger.SetLoggerLevel(this.Name, LogLevel.Debug);
            return hbsLogger;
        }

        private void InitialiseLogging(string directory)
        {
            //this.Logger = new log4NetLogger(this.Name);
            this.Logger = new HbsLogger(GetHbsLogger(), this.Directory, this.Name);
            this.Logger.Debug($"Initialized logging for [{this.Name}]");
        }

        private void Initialize(HarmonyInstance harmonyInstance, string settings)
        {
            this.Logger.Debug("Initializing Features...");
            this.ModSettings = JsonHelpers.Deserialize<TModSettings>(settings) ?? new TModSettings();
            var jsonSettings = JsonHelpers.Deserialize(settings);
            this.ModFeatures.ForEach(
                feature => feature.Initialize(
                    harmonyInstance,
                    jsonSettings?[feature.Name]?.ToString(),
                    this.Logger,
                    this.Directory));
            this.Logger.Debug("Feature initialization complete.");
        }
    }
}