using System.Reflection;
using Harmony;
using vfBattleTechMod_Core.Mods.Interfaces;
using vfBattleTechMod_Core.Utils.Interfaces;

namespace vfBattleTechMod_Core.Mods.BaseImpl
{
    public class ModPatchDirective : IModPatchDirective
    {
        public ModPatchDirective(
            MethodBase targetMethodType,
            MethodInfo prefixMethodType,
            MethodInfo postfixMethodType,
            MethodInfo transpilerMethodType,
            int priority = Harmony.Priority.Normal)
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

        public MethodBase TargetMethodType { get; }

        public MethodInfo TranspilerMethodType { get; }

        public void Initialize(HarmonyInstance harmonyInstance, ILogger logger)
        {
            var harmonyPrefix = PrefixMethodType == null ? null : new HarmonyMethod(PrefixMethodType);
            var harmonyPostfix = PostfixMethodType == null ? null : new HarmonyMethod(PostfixMethodType);
            var harmonyTranspiler = TranspilerMethodType == null ? null : new HarmonyMethod(TranspilerMethodType);
            
            var dynamicMethod = harmonyInstance.Patch(
                TargetMethodType,
                harmonyPrefix,
                harmonyPostfix,
                harmonyTranspiler);
            
            logger.Debug($"Patched [{TargetMethodType.Name}]. Add more details you lazy bastard...");
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