using System.Reflection;
using Harmony;
using vfBattleTechMod_Core.Utils.Interfaces;

namespace vfBattleTechMod_Core.Mods.Interfaces
{
    public interface IModPatchDirective
    {
        MethodInfo TargetMethodType { get; }

        MethodInfo PrefixMethodType { get; }

        MethodInfo PostfixMethodType { get; }

        MethodInfo TranspilerMethodType { get; }

        int Priority { get; }

        void Initialize(HarmonyInstance harmonyInstance, ILogger logger);
    }
}