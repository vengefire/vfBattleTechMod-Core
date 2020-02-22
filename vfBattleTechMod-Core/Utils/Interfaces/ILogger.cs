using System;

namespace vfBattleTechMod_Core.Utils.Interfaces
{
    public interface ILogger
    {
        void Debug(string message);

        void Error(string message, Exception ex);
    }
}