using IdelPog.Core.Logging.Contracts;

namespace IdelPog.Core.Logging.Writer
{
    public sealed class ConsoleWriter : ILogWriter
    {
        public void Write<T>(LogLevel logLevel, LogDirection logDirection, T[] messages)
        {
            WriteLog<T>(logLevel, logDirection);
            WriteMessages(messages);
        }

        public void WriteError<T>(T[] messages, Exception exception)
        {
            Console.WriteLine($"[{LogLevel.ERROR}] [{GetNowTime()}] {LogDirection.IN} {typeof(T).Name} \"{exception}\"");
            WriteMessages(messages);
        }

        private static void WriteLog<T>(LogLevel logLevel, LogDirection logDirection)
        {
            Console.WriteLine($"[{logLevel}] [{GetNowTime()}] {logDirection} {typeof(T).Name}");
        }
        
        private static void WriteMessages<T>(T[] messages)
        {
            foreach (T message in messages)
            {
                Console.WriteLine($"-> {message}");
            }
        }
        
        private static string GetNowTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }
    }
}