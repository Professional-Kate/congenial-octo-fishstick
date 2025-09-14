using IdelPog.Core.Logging.Contracts;
using IdelPog.Core.Logging.Writer;

namespace IdelPog.Core.Logging
{
    public sealed class LoggingService : ILogger
    {
        private readonly ILogWriter _logWriter;

        public LoggingService(ILogWriter logWriter)
        {
            _logWriter = logWriter;
        }

        public void Log<T>(LogLevel logLevel, LogDirection logDirection, T[] messages)
        {
            _logWriter.Write(logLevel, logDirection, messages);
        }

        public void LogInfo<T>(LogDirection logDirection, T[] messages)
        {
            Log(LogLevel.INFO, logDirection, messages);
        }

        public void LogError<T>(LogDirection logDirection, T[] messages)
        {
            Log(LogLevel.ERROR, logDirection, messages);
        }
        
        public void Log<T>(LogLevel logLevel, LogDirection logDirection, T message)
        { 
            Log(logLevel, logDirection, [message]);
        }
    }
}