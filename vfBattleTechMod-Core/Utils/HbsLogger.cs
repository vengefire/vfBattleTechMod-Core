using System;
using System.IO;
using HBS.Logging;
using vfBattleTechMod_Core.Utils.Interfaces;
using LogLevel = vfBattleTechMod_Core.Utils.Enums.LogLevel;

namespace vfBattleTechMod_Core.Utils
{
    public class HbsLogger : ILogger
    {
        private readonly ILogAppender logAppender;

        private readonly ILog logger;
        private LogLevel _logLevel;

        public LogLevel LogLevel
        {
            get => _logLevel;
            set
            {
                _logLevel = value;
            }
        }

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
                catch (Exception)
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
            if (LogLevel >= LogLevel.Debug)
            {
                logger.Log(message);
            }
        }

        public void Error(string message, Exception ex)
        {
            logger.LogError(message);
        }

        public void Trace(string message)
        {
            if (LogLevel >= LogLevel.Trace)
            {
                logger.LogDebug(message);
            }
        }
    }
}