using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.Framework;
using Harmony;
using vfBattleTechMod_Core.Mods.BaseImpl;
using vfBattleTechMod_Core.Mods.Interfaces;

namespace vfBattleTechMod_ProcGenStores.Mod.Features.RepMods
{
    public class RepModFeature : ModFeatureBase<RepModFeatureSettings>
    {
        private new static RepModFeature Myself;

        public void PostFixContractConstructor(int initialContractValue)
        {
            Logger.Debug($"Patched Contract Constructor!");
            initialContractValue = 666;
        }

        public RepModFeature()
            : base(GetPatchDirectives)
        {
            Myself = this;
        }

        public static List<IModPatchDirective> GetPatchDirectives =>
            new List<IModPatchDirective>
            {
                new ModPatchDirective(
                    typeof(Contract).GetMethod(nameof(Contract.SetInitialReward)),
                    typeof(RepModFeature).GetMethod(nameof(PrefixContractSetInitialReward)),
                    null,
                    null,
                    0),
                new ModPatchDirective(
                    AccessTools.Constructor(typeof(Contract), new []{ typeof(string), typeof(string), typeof(string), typeof(ContractTypeValue), typeof(GameInstance), typeof(ContractOverride), typeof(GameContext), typeof(bool), typeof(int), typeof(int) , typeof(int) }),
                    null,
                    typeof(RepModFeature).GetMethod(nameof(PostFixContractConstructor)),
                    null,
                    0),
            };

        public override string Name => "Rep Mod Features";

        public static bool PrefixContractSetInitialReward(Contract __instance, int cbills)
        {
            Logger.Debug($"Modifying cbills for Set Initial Reward...");
            cbills *= 1000;
            return true;
        }

        protected override bool ValidateSettings()
        {
            return true;
        }

        public override void OnInitializeComplete()
        {
        }
    }
}