using System;
using System.IO;
using log4net;
using log4net.Config;
using vfBattleTechMod_Core.Utils.Interfaces;

namespace vfBattleTechMod_Core.Utils
{
    public class log4NetLogger : ILogger
    {
        private readonly ILog logger;

        static log4NetLogger()
        {
            XmlConfigurator.Configure(new FileInfo("log4net.config"));
        }

        public log4NetLogger(string name)
        {
            this.logger = LogManager.GetLogger(name);
        }

        public void Debug(string message)
        {
            this.logger.Debug(message);
        }

        public void Error(string message, Exception ex)
        {
            this.logger.Error(message, ex);
        }
    }
}