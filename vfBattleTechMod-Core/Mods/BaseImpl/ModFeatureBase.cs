using System;
using System.Collections.Generic;
using Harmony;
using Newtonsoft.Json;
using vfBattleTechMod_Core.Helpers;
using vfBattleTechMod_Core.Mods.Interfaces;
using vfBattleTechMod_Core.Utils.Interfaces;

namespace vfBattleTechMod_Core.Mods.BaseImpl
{
    public abstract class ModFeatureBase<TModFeatureSettings> : IModFeature<TModFeatureSettings>
        where TModFeatureSettings : IModFeatureSettings, new()
    {
        private static ModFeatureBase<TModFeatureSettings> _myself;

        protected ModFeatureBase(List<IModPatchDirective> patchDirectives)
        {
            PatchDirectives = patchDirectives;
            Myself = this;
        }

        protected static ModFeatureBase<TModFeatureSettings> Myself
        {
            get => _myself;
            set
            {
                if (_myself != null)
                {
                    throw new InvalidProgramException(
                        $"Mod Feature [{Myself.Name}] has already been created. Only one may be instanced.");
                }

                _myself = value;
            }
        }

        protected static ILogger Logger { get; set; }

        protected string Directory { get; private set; }

        public bool Enabled => Settings.Enabled;

        public abstract string Name { get; }

        public List<IModPatchDirective> PatchDirectives { get; }

        public TModFeatureSettings Settings { get; private set; }
        public TModFeatureSettings DefaultSettings => new TModFeatureSettings();

        public virtual void Initialize(
            HarmonyInstance harmonyInstance,
            string settings,
            ILogger logger,
            string directory)
        {
            Logger = logger;
            Directory = directory;

            Settings = settings == null
                ? new TModFeatureSettings()
                : JsonHelpers.DeserializeObject<TModFeatureSettings>(settings);

            if (!Settings.Enabled)
            {
                Logger.Debug(
                    $"Feature [{Name}] has been disabled with settings [{JsonConvert.SerializeObject(Settings)}]");
                return;
            }

            if (!ValidateSettings())
            {
                Logger.Debug($"Feature [{Name}] has been disabled due to settings validation failure.");
                return;
            }

            ExecutePatchDirectives(harmonyInstance);
            OnInitializeComplete();
            Logger.Debug($"Feature [{Name}] initialized with settings [{JsonConvert.SerializeObject(Settings)}]");
        }

        public abstract void OnInitializeComplete();

        protected abstract bool ValidateSettings();

        private void ExecutePatchDirectives(HarmonyInstance harmonyInstance)
        {
            PatchDirectives.ForEach(directive => directive.Initialize(harmonyInstance, Logger));
        }
    }
}