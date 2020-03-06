using System;
using vfBattleTechMod_Core.Utils.Enums;

namespace vfBattleTechMod_Core.Utils.Interfaces
{
    public interface ILogger
    {
        void Debug(string message);

        void Error(string message, Exception ex);

        void Trace(string message);
        LogLevel LogLevel { get; set; }
    }
}