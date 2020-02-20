using System;
using System.IO;
using HBS.Logging;
using vfBattleTechMod_Core.Utils.Interfaces;

namespace vfBattleTechMod_Core.Utils
{
    public class Logger : ILogger
    {
        private readonly ILogAppender logAppender;

        private readonly ILog logger;

        public Logger(ILog logger, string directory, string moduleName)
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
            this.logAppender = new FileLogAppender(
                logFilePath,
                FileLogAppender.WriteMode.INSTANT);
            HBS.Logging.Logger.AddAppender(moduleName, this.logAppender);
        }

        public void Debug(string message)
        {
            this.logger.LogDebug(message);
        }

        public void Error(string message, Exception ex)
        {
            this.logger.LogDebug(message);
        }
    }
}