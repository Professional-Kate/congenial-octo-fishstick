namespace Console.Commands.Resolver.Exceptions
{
    public class InvalidArgumentCountException : Exception
    {
        private const string MESSAGE = "Expected argument list of length {0}, but got length {1}.";
        public readonly int ExpectedSize;
        public readonly int ActualSize;

        public InvalidArgumentCountException(int expected, int actual) : base(string.Format(MESSAGE, expected, actual))
        {
            ExpectedSize = expected;
            ActualSize = actual;    
        }
    }
}