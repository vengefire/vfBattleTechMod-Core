using System;
using System.IO;
using HBS.Logging;
using vfBattleTechMod_Core.Utils.Interfaces;

namespace vfBattleTechMod_Core.Utils
{
    public class HbsLogger : ILogger
    {
        private readonly ILogAppender logAppender;

        private readonly ILog logger;

        public HbsLogger(ILog logger, string directory, string moduleName)
        {
            var logFileName = $"{moduleName}-log.txt";
            var logFilePath = Path.Combine(directory, logFileName);

            if (File.Exists(logFilePath))
            {
                try
                {
                    File.Delete(logFilePath);
                }
                catch (Exception ex)
                {
                    logger.LogDebug($"Failed to delete existing log file [{logFilePath}]");
                }
            }

            this.logger = logger;
            logAppender = new FileLogAppender(
                logFilePath,
                FileLogAppender.WriteMode.INSTANT);
            Logger.AddAppender(moduleName, logAppender);
        }

        public void Debug(string message)
        {
            logger.LogDebug(message);
        }

        public void Error(string message, Exception ex)
        {
            logger.LogDebug(message);
        }
    }
}