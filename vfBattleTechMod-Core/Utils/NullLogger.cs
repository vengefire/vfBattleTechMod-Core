using System;
using vfBattleTechMod_Core.Utils.Interfaces;

namespace vfBattleTechMod_Core.Utils
{
    public class NullLogger : ILogger
    {
        public void Debug(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(string message, Exception ex)
        {
            Console.WriteLine(string.Join("\r\n", message, ex.ToString()));
        }

        public void Trace(string message)
        {
            Console.WriteLine(message);
        }
    }
}