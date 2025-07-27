namespace IdelPog.Messaging.Exceptions
{
    public class BufferSizeMismatchException : Exception
    {
        private const string MESSAGE = "The passed collection is not the correct size! Expected {0}, got {1}!";

        public readonly int ActualSize;
        public readonly int ExpectedSize;

        public BufferSizeMismatchException(int actual, int expected) : base(string.Format(MESSAGE, expected, actual))
        {
            ActualSize = actual;
            ExpectedSize = expected;
        }
    }
}