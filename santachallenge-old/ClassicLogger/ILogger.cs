namespace ClassicLogger
{

    public enum LogLevel { Debug, Info, Error }

    public interface ILogger
    {
        void Write(LogLevel level, string format, params object[] args);

        void Debug(string format, params object[] args);
        void Info(string format, params object[] args);
        void Error(string format, params object[] args);
    }
}
