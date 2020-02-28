using System.Collections.Generic;
using System.Linq;
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

        public RepModFeature()
            : base(RepModFeature.GetPatchDirectives)
        {
            RepModFeature.Myself = this;
        }

        /*
        [HarmonyPatch(typeof(Mech), "Init")]
        public static class Mech_Init_Patch
        {
            public static void Postfix(Mech __instance)
            {
                if (__instance.Combat.ActiveContract.ContractTypeValue.UsesFury || __instance.team.PlayerControlsTeam)
                    return;

                // Build up a list of Mech Components that we can dynamically upgrade... weapons and misc upgrades
                var mechComponentsToUpgrade = __instance.Weapons.Cast<MechComponent>().ToList();
                mechComponentsToUpgrade.AddRange(__instance.miscComponents.ToList());

                foreach (var component in mechComponentsToUpgrade)
                {
                    // Get the MechComponentDef from the object
                    var definition = component.mechComponentRef.Def;
                    
                    // Exclude if it's not stock
                    if (!definition.ComponentTags.Contains("component_type_stock"))
                    {
                        continue;
                    }
                    
                    Logger.Log("Old Component: " + definition.Description.Id);
                    // Upgrade the component based on it's derived type
                    MechComponentDef replacementDefinition = component is Weapon ? UpgradeWeapons(__instance.Combat.ActiveContract, definition) : UpgradeUpgrades(__instance.Combat.ActiveContract, definition);
                    // Set the ComponentDefId on the component. This forces a Reload of the component from the Data Manager...
                    component.baseComponentRef.ComponentDefID = replacementWeaponDefinition.Description.Id;
                    component.mechComponentRef.ComponentDefID = replacementWeaponDefinition.Description.Id;
                    // Update the protected Def property to the refreshed component def value to keep everything nice and consistent...
                    Traverse.Create(component).Property("componentDef").SetValue(component.baseComponentRef.Def);
                    Logger.Log("New Component: " + component.mechComponentRef.Def.Description.Id);
                }
            }
        }*/

        public static List<IModPatchDirective> GetPatchDirectives =>
            new List<IModPatchDirective>
            {
                new ModPatchDirective(
                    typeof(Contract).GetMethod(nameof(Contract.SetInitialReward)),
                    typeof(RepModFeature).GetMethod(nameof(RepModFeature.PrefixContractSetInitialReward)),
                    null,
                    null,
                    0),
                new ModPatchDirective(
                    AccessTools.Constructor(typeof(Contract),
                        new[]
                        {
                            typeof(string), typeof(string), typeof(string), typeof(ContractTypeValue),
                            typeof(GameInstance), typeof(ContractOverride), typeof(GameContext), typeof(bool),
                            typeof(int), typeof(int), typeof(int)
                        }),
                    null,
                    typeof(RepModFeature).GetMethod(nameof(RepModFeature.PostFixContractConstructor)),
                    null,
                    0)
            };

        public override string Name => "Rep Mod Features";

        public void PostFixContractConstructor(int initialContractValue)
        {
            ModFeatureBase<RepModFeatureSettings>.Logger.Debug("Patched Contract Constructor!");
            initialContractValue = 666;
        }

        public static bool PrefixContractSetInitialReward(Contract __instance, int cbills)
        {
            ModFeatureBase<RepModFeatureSettings>.Logger.Debug("Modifying cbills for Set Initial Reward...");
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