using IdelPog.Core.Logging.Contracts;

namespace IdelPog.Core.Logging
{
    public interface ILogger
    {
        public void Log<T>(LogLevel logLevel, LogDirection logDirection, T[] messages);

        public void Log<T>(LogLevel logLevel, LogDirection logDirection, T message);
        
        public void LogInfo<T>(LogDirection logDirection, T[] messages);
        
        public void LogError<T>(T[] messages, Exception exception);
    }
}