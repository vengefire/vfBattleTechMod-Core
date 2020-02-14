using System;
using HBS.Logging;
using vfBattleTechMod_Core.Utils.Interfaces;

namespace vfBattleTechMod_Core.Utils
{
    public class Logger : ILogger
    {
        private readonly ILog _logger;

        public Logger(ILog logger)
        {
            _logger = logger;
        }

        public void Debug(string message)
        {
            _logger.LogDebug(message);
        }

        public void Error(string message, Exception ex)
        {
            _logger.LogError(message, ex);
        }
    }
}