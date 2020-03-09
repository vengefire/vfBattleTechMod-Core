using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using vfBattleTechMod_Core.Utils.Enums;
using vfBattleTechMod_Core.Utils.Interfaces;

namespace vfBattleTechMod_Core.Utils
{
    public class log4NetLogger : ILogger
    {
        private readonly ILog logger;

        static log4NetLogger()
        {
            var filename = "log4net.config";
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filePath = Path.Combine(basePath, filename);

            if (!File.Exists(filePath))
            {
                throw new InvalidProgramException($"Failed to find logging config file [{filePath}]");
            }
            
            var result = XmlConfigurator.Configure(new FileInfo(filePath));
        }

        public log4NetLogger(string name)
        {
            logger = LogManager.GetLogger(name);
            LogLevel = LogLevel.Trace;
        }

        public void Debug(string message)
        {
            if (LogLevel >= LogLevel.Debug)
            {
                logger.Debug(message);
            }
        }

        public void Error(string message, Exception ex)
        {
            logger.Error(message, ex);
        }

        public void Trace(string message)
        {
            if (LogLevel >= LogLevel.Trace)
            {
                logger.Info(message);
            }
        }

        public LogLevel LogLevel { get; set; }
    }
}