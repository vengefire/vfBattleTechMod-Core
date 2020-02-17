using System.Collections.Generic;
using Harmony;
using Newtonsoft.Json;
using vfBattleTechMod_Core.Mods.Interfaces;
using vfBattleTechMod_Core.Utils.Interfaces;

namespace vfBattleTechMod_Core.Mods.BaseImpl
{
    public abstract class ModFeatureBase<TModFeatureSettings> : IModFeature<TModFeatureSettings>
        where TModFeatureSettings : IModFeatureSettings
    {
        protected ModFeatureBase(List<IModPatchDirective> patchDirectives)
        {
            this.PatchDirectives = patchDirectives;
        }

        protected string Directory { get; private set; }

        protected static ILogger Logger { get; set; }

        public bool Enabled => this.Settings.Enabled;

        public abstract string Name { get; }

        public virtual void Initialize(HarmonyInstance harmonyInstance, string settings, ILogger logger, string directory)
        {
            ModFeatureBase<TModFeatureSettings>.Logger = logger;
            this.Directory = directory;
            this.Settings = JsonConvert.DeserializeObject<TModFeatureSettings>(settings);
            this.ExecutePatchDirectives(harmonyInstance);
            this.OnInitializeComplete();
            Logger.Debug($"Feature [{this.Name}] initialized with settings [{JsonConvert.SerializeObject(this.Settings)}]");
        }

        private void ExecutePatchDirectives(HarmonyInstance harmonyInstance)
        {
            this.PatchDirectives.ForEach(directive => directive.Initialize(harmonyInstance, ModFeatureBase<TModFeatureSettings>.Logger));
        }

        public TModFeatureSettings Settings { get; private set; }

        public abstract void OnInitializeComplete();

        public List<IModPatchDirective> PatchDirectives { get; }
    }
}