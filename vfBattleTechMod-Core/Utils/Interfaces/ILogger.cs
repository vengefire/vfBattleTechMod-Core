namespace vfBattleTechMod_Core.Utils.Interfaces
{
    using System;

    public interface ILogger
    {
        void Debug(string message);

        void Error(string message, Exception ex);
    }
}