using System;
using vfBattleTechMod_Core.Utils.Enums;
using vfBattleTechMod_Core.Utils.Interfaces;

namespace vfBattleTechMod_Core.Utils.Loggers
{
    public class NullLogger : ILogger
    {
        public void Debug(string message)
        {
            if (LogLevel >= LogLevel.Debug)
            {
                Console.WriteLine(message);
            }
        }

        public void Error(string message, Exception ex)
        {
            Console.WriteLine(string.Join("\r\n", message, ex.ToString()));
        }

        public void Trace(string message)
        {
            if (LogLevel >= LogLevel.Trace)
            {
                Console.WriteLine(message);
            }
        }

        public LogLevel LogLevel { get; set; }
    }
}