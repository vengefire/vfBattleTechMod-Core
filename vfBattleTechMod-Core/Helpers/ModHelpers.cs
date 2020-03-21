using System;
using System.IO;
using System.Reflection;

namespace vfBattleTechMod_Core.Helpers
{
    public static class ModHelpers
    {
        public static string ModResourceFilePath(string fileResource) =>
            Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                throw new InvalidProgramException($"Executing Assembly Location cannot be null."),
                fileResource);
    }
}