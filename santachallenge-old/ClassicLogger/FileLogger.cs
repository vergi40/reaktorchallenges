using log4net.Core;
using log4net.Repository;

namespace ClassicLogger
{
    public class FileLogger : ILogger
    {
        private readonly log4net.ILog _mLog;

        public FileLogger(object forThis, ILoggerRepository localRepo)
        {
            _mLog = log4net.LogManager.GetLogger(localRepo.Name, forThis.ToString());
        }

        public void Write(LogLevel level, string format, params object[] args)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    Debug(format, args);
                    break;
                case LogLevel.Error:
                    Error(format, args);
                    break;
                default:
                    Info(format, args);
                    break;
            }
        }

        public void Debug(string format, params object[] args)
        {
            _mLog.DebugFormat(format, args);
        }

        public void Info(string format, params object[] args)
        {
            _mLog.InfoFormat(format, args);
        }

        public void Error(string format, params object[] args)
        {
            _mLog.ErrorFormat(format, args);
        }
    }
}