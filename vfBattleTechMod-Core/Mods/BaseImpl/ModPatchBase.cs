using System.Reflection;
using Harmony;
using vfBattleTechMod_Core.Mods.Interfaces;
using vfBattleTechMod_Core.Utils.Interfaces;

namespace vfBattleTechMod_Core.Mods.BaseImpl
{
    public class ModPatchDirective : IModPatchDirective
    {
        public ModPatchDirective(
            MethodInfo targetMethodType,
            MethodInfo prefixMethodType,
            MethodInfo postfixMethodType,
            MethodInfo transpilerMethodType,
            int priority)
        {
            TargetMethodType = targetMethodType;
            PrefixMethodType = prefixMethodType;
            PostfixMethodType = postfixMethodType;
            TranspilerMethodType = transpilerMethodType;
            Priority = priority;
        }

        public MethodInfo PostfixMethodType { get; }

        public MethodInfo PrefixMethodType { get; }

        public int Priority { get; }

        public MethodInfo TargetMethodType { get; }

        public MethodInfo TranspilerMethodType { get; }

        public void Initialize(HarmonyInstance harmonyInstance, ILogger logger)
        {
            harmonyInstance.Patch(
                TargetMethodType,
                PrefixMethodType == null ? null : new HarmonyMethod(PrefixMethodType),
                PostfixMethodType == null ? null : new HarmonyMethod(PostfixMethodType),
                TranspilerMethodType == null ? null : new HarmonyMethod(TranspilerMethodType));
        }

        public static void JustDoNothing()
        {
        }

        public static MethodInfo JustDoNothingInfo()
        {
            return typeof(ModPatchDirective).GetMethod("JustDoNothing");
        }

        public static bool JustReturnFalse()
        {
            return false;
        }

        public static MethodInfo JustReturnFalseInfo()
        {
            return typeof(ModPatchDirective).GetMethod("JustReturnFalse");
        }

        public static bool JustReturnTrue()
        {
            return true;
        }

        public static MethodInfo JustReturnTrueInfo()
        {
            return typeof(ModPatchDirective).GetMethod("JustReturnTrue");
        }
    }
}