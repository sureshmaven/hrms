using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Web.config", Watch = true)]
namespace HRMSApplication.Helpers
{
    public class LogInformation
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Info(string Infolog)
        {
            logger.Info(Infolog);
        }
        public static void Debug(string Debuglog)
        {
            logger.Debug(Debuglog);
        }
        public static void Warn(string Warnlog)
        {
            logger.Warn(Warnlog);
        }
        public static void Error(string Errorlog)
        {
            logger.Error(Errorlog);
        }
    }
}