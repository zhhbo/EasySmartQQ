using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Logging;
namespace Prism.Logging
{
    public static class LogEx
    {
        public static void Debug(this ILoggerFacade log, string debugInfo)
        {
            log.Log(debugInfo, Category.Debug, Priority.Medium);
        }
        public static void Debug(this ILoggerFacade log, string debugInfo,params object[] arg)
        {
            log.Log(string.Format( debugInfo,arg), Category.Debug, Priority.Medium);
        }
        public static void Error(this ILoggerFacade log, string error)
        {
            log.Log(error, Category.Exception, Priority.High);
        }
        public static void Info(this ILoggerFacade log, string message)
        {
            log.Log(message, Category.Info, Priority.Low);
        }
        public static void Error(this ILoggerFacade log,Exception e, string message)
        {
            log.Log(message+"："+e.Message, Category.Info, Priority.Low);
            log.Log(message + "详细信息：" + e.StackTrace, Category.Info, Priority.Low);
        }
        public static void Error(this ILoggerFacade log, Exception e)
        {
            log.Log( "出现错误：" + e.Message, Category.Info, Priority.Low);
            log.Log("详细信息：" + e.StackTrace, Category.Info, Priority.Low);
        }
        public static void Warn(this ILoggerFacade log,  string message)
        {
            log.Log(message , Category.Info, Priority.Low);
        }
    }
}
