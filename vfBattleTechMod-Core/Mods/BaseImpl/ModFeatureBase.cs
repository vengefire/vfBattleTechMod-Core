namespace vfBattleTechMod_Core.Mods.BaseImpl
{
    using System.Collections.Generic;

    using Harmony;

    using Newtonsoft.Json;

    using vfBattleTechMod_Core.Mods.Interfaces;
    using vfBattleTechMod_Core.Utils.Interfaces;

    public abstract class ModFeatureBase<TModFeatureSettings> : IModFeature<TModFeatureSettings>
        where TModFeatureSettings : IModFeatureSettings
    {
        protected ModFeatureBase(List<IModPatchDirective> patchDirectives)
        {
            this.PatchDirectives = patchDirectives;
        }

        public bool Enabled => this.Settings.Enabled;

        public abstract string Name { get; }

        public List<IModPatchDirective> PatchDirectives { get; }

        public TModFeatureSettings Settings { get; private set; }

        protected static ILogger Logger { get; set; }

        protected string Directory { get; private set; }

        public virtual void Initialize(
            HarmonyInstance harmonyInstance,
            string settings,
            ILogger logger,
            string directory)
        {
            Logger = logger;
            this.Directory = directory;
            this.Settings = JsonConvert.DeserializeObject<TModFeatureSettings>(settings);
            this.ExecutePatchDirectives(harmonyInstance);
            this.OnInitializeComplete();
            Logger.Debug(
                $"Feature [{this.Name}] initialized with settings [{JsonConvert.SerializeObject(this.Settings)}]");
        }

        public abstract void OnInitializeComplete();

        private void ExecutePatchDirectives(HarmonyInstance harmonyInstance)
        {
            this.PatchDirectives.ForEach(directive => directive.Initialize(harmonyInstance, Logger));
        }
    }
}