using System.IO;
using System.Reflection;
using log4net;
using log4net.Repository;

namespace ClassicLogger
{
    public enum LoggerType { File, Console, Testfile }

    public static class LoggerFactory
    {
        static LoggerFactory()
        {
            string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string configFile = Path.Combine(assemblyDir, "ClassicLogger.config");
            ILoggerRepository localRepo = LogManager.CreateRepository("localRepo");
            log4net.Config.XmlConfigurator.Configure(localRepo, new FileInfo(configFile));

        }

        public static ILogger GetLogger(object forThis, LoggerType type = LoggerType.File)
        {
            return new FileLogger(forThis, LogManager.GetRepository("localRepo"));
        }
    }
}