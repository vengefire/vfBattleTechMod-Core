namespace vfBattleTechMod_Core.Mods.BaseImpl
{
    using System.Reflection;

    using Harmony;

    using vfBattleTechMod_Core.Mods.Interfaces;
    using vfBattleTechMod_Core.Utils.Interfaces;

    public class ModPatchDirective : IModPatchDirective
    {
        public ModPatchDirective(
            MethodInfo targetMethodType,
            MethodInfo prefixMethodType,
            MethodInfo postfixMethodType,
            MethodInfo transpilerMethodType,
            int priority)
        {
            this.TargetMethodType = targetMethodType;
            this.PrefixMethodType = prefixMethodType;
            this.PostfixMethodType = postfixMethodType;
            this.TranspilerMethodType = transpilerMethodType;
            this.Priority = priority;
        }

        public MethodInfo PostfixMethodType { get; }

        public MethodInfo PrefixMethodType { get; }

        public int Priority { get; }

        public MethodInfo TargetMethodType { get; }

        public MethodInfo TranspilerMethodType { get; }

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

        public void Initialize(HarmonyInstance harmonyInstance, ILogger logger)
        {
            harmonyInstance.Patch(
                this.TargetMethodType,
                this.PrefixMethodType == null ? null : new HarmonyMethod(this.PrefixMethodType),
                this.PostfixMethodType == null ? null : new HarmonyMethod(this.PostfixMethodType),
                this.TranspilerMethodType == null ? null : new HarmonyMethod(this.TranspilerMethodType));
        }
    }
}