namespace vfBattleTechMod_Core.Utils
{
    using System;
    using System.IO;

    using HBS.Logging;

    using vfBattleTechMod_Core.Utils.Interfaces;

    public class Logger : ILogger
    {
        private readonly ILogAppender logAppender;

        private readonly ILog logger;

        public Logger(ILog logger, string directory, string moduleName)
        {
            this.logger = logger;
            this.logAppender = new FileLogAppender(
                Path.Combine(directory, "log.txt"),
                FileLogAppender.WriteMode.INSTANT);
            HBS.Logging.Logger.AddAppender(moduleName, this.logAppender);
        }

        public void Debug(string message)
        {
            this.logger.LogDebug(message);
        }

        public void Error(string message, Exception ex)
        {
            this.logger.LogError(message, ex);
        }
    }
}