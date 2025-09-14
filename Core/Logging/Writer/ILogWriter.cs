using IdelPog.Core.Logging.Contracts;

namespace IdelPog.Core.Logging.Writer
{
    public interface ILogWriter
    {
        public void Write<T>(LogLevel logLevel, LogDirection logDirection, T[] messages);
    }
}