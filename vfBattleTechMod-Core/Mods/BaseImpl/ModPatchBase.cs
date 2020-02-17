using System.Reflection;
using Harmony;
using vfBattleTechMod_Core.Mods.Interfaces;
using vfBattleTechMod_Core.Utils.Interfaces;

namespace vfBattleTechMod_Core.Mods.BaseImpl
{
    public class ModPatchDirective : IModPatchDirective
    {
        public ModPatchDirective(MethodInfo targetMethodType, MethodInfo prefixMethodType, MethodInfo postfixMethodType, MethodInfo transpilerMethodType, int priority)
        {
            this.TargetMethodType = targetMethodType;
            this.PrefixMethodType = prefixMethodType;
            this.PostfixMethodType = postfixMethodType;
            this.TranspilerMethodType = transpilerMethodType;
            this.Priority = priority;
        }

        public MethodInfo TargetMethodType { get; }

        public MethodInfo PrefixMethodType { get; }

        public MethodInfo PostfixMethodType { get; }

        public MethodInfo TranspilerMethodType { get; }

        public int Priority { get; }

        public void Initialize(HarmonyInstance harmonyInstance, ILogger logger)
        {
            harmonyInstance.Patch(
                this.TargetMethodType,
                this.PrefixMethodType == null ? null : new HarmonyMethod(this.PrefixMethodType),
                this.PostfixMethodType == null ? null : new HarmonyMethod(this.PostfixMethodType),
                this.TranspilerMethodType == null ? null : new HarmonyMethod(this.TranspilerMethodType));
        }

        public static MethodInfo JustDoNothingInfo()
        {
            return typeof(ModPatchDirective).GetMethod("JustDoNothing");
        }

        public static void JustDoNothing()
        {
        }

        public static MethodInfo JustReturnTrueInfo()
        {
            return typeof(ModPatchDirective).GetMethod("JustReturnTrue");
        }

        public static bool JustReturnTrue()
        {
            return true;
        }

        public static MethodInfo JustReturnFalseInfo()
        {
            return typeof(ModPatchDirective).GetMethod("JustReturnFalse");
        }

        public static bool JustReturnFalse()
        {
            return false;
        }
    }
}