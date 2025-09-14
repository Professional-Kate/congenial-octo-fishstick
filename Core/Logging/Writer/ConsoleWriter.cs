using IdelPog.Core.Logging.Contracts;

namespace IdelPog.Core.Logging.Writer
{
    public sealed class ConsoleWriter : ILogWriter
    {
        public void Write<T>(LogLevel logLevel, LogDirection logDirection, T[] messages)
        {
            string timeStamp = DateTime.Now.ToString("HH:mm:ss");
            
            Console.WriteLine($"[{logLevel}] [{timeStamp}] {logDirection} {typeof(T).Name}");

            foreach (T message in messages)
            {
                Console.WriteLine($"-> {message}");
            }
        }
    }
}