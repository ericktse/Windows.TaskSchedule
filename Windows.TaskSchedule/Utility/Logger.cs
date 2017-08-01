using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Internal;

namespace Windows.TaskSchedule.Utility
{
    public class Logger
    {
        private static readonly object LockObj = new object();

        private static NLog.Logger _nlog;
        private static NLog.Logger NLog
        {
            get
            {
                if (_nlog == null)
                {
                    lock (LockObj)
                    {
                        if (_nlog == null)
                        {
                            string xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "NLog.config");
                            XmlLoggingConfiguration config = new XmlLoggingConfiguration(xmlPath, false);
                            LogManager.Configuration = config;
                            _nlog = LogManager.GetCurrentClassLogger();
                        }
                    }
                }
                return _nlog;
            }
        }

        public static void Debug(string message)
        {
            NLog.Debug(message);
        }

        public static void Info(string message)
        {
            NLog.Info(message);
        }

        public static void Error(string message, Exception ex = null)
        {
            string alarmText = ex == null ? message : string.Join("------", message, ex.ToString());
            NLog.Error(ex, alarmText);
        }
    }
}
