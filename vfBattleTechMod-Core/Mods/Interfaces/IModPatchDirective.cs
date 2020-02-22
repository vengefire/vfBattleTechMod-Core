using System.Reflection;
using Harmony;
using vfBattleTechMod_Core.Utils.Interfaces;

namespace vfBattleTechMod_Core.Mods.Interfaces
{
    public interface IModPatchDirective
    {
        MethodInfo PostfixMethodType { get; }

        MethodInfo PrefixMethodType { get; }

        int Priority { get; }

        MethodInfo TargetMethodType { get; }

        MethodInfo TranspilerMethodType { get; }

        void Initialize(HarmonyInstance harmonyInstance, ILogger logger);
    }
}