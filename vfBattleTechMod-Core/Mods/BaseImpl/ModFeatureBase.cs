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
        protected ModFeatureBase(List<IModPatchDirective> patchDirectives)
        {
            this.PatchDirectives = patchDirectives;
            ModFeatureBase<TModFeatureSettings>.Myself = this;
        }

        protected static ModFeatureBase<TModFeatureSettings> Myself
        {
            get => ModFeatureBase<TModFeatureSettings>.Myself;
            set
            {
                if (ModFeatureBase<TModFeatureSettings>.Myself != null)
                {
                    throw new InvalidProgramException($"Mod Feature [{ModFeatureBase<TModFeatureSettings>.Myself.Name}] has already been created. Only one may be instanced.");
                }

                ModFeatureBase<TModFeatureSettings>.Myself = value;
            }
        }

        protected static ILogger Logger { get; set; }

        protected string Directory { get; private set; }

        public bool Enabled => this.Settings.Enabled;

        public abstract string Name { get; }

        public List<IModPatchDirective> PatchDirectives { get; }

        public TModFeatureSettings Settings { get; private set; }

        public virtual void Initialize(
            HarmonyInstance harmonyInstance,
            string settings,
            ILogger logger,
            string directory)
        {
            ModFeatureBase<TModFeatureSettings>.Logger = logger;
            this.Directory = directory;

            ModFeatureBase<TModFeatureSettings>.Logger.Debug($"Mod Feature [{this.Name}] - Default Settings [\r\n{JsonConvert.SerializeObject(this.Settings)}]");

            this.Settings = settings == null ? new TModFeatureSettings() : JsonHelpers.DeserializeObject<TModFeatureSettings>(settings);

            if (!this.Settings.Enabled)
            {
                ModFeatureBase<TModFeatureSettings>.Logger.Debug($"Feature [{this.Name}] has been disabled with settings [{JsonConvert.SerializeObject(this.Settings)}]");
                return;
            }

            if (!this.ValidateSettings())
            {
                ModFeatureBase<TModFeatureSettings>.Logger.Debug($"Feature [{this.Name}] has been disabled due to settings validation failure.");
                return;
            }

            this.ExecutePatchDirectives(harmonyInstance);
            this.OnInitializeComplete();
            ModFeatureBase<TModFeatureSettings>.Logger.Debug($"Feature [{this.Name}] initialized with settings [{JsonConvert.SerializeObject(this.Settings)}]");
        }

        public abstract void OnInitializeComplete();

        protected abstract bool ValidateSettings();

        private void ExecutePatchDirectives(HarmonyInstance harmonyInstance)
        {
            this.PatchDirectives.ForEach(directive => directive.Initialize(harmonyInstance, ModFeatureBase<TModFeatureSettings>.Logger));
        }
    }
}