using IdelPog.Messaging.Buffer;

namespace IdelPog.Messaging.Exceptions
{
    public class InvalidBufferStateException : Exception
    {
        private const string MESSAGE = "Expected BufferState was {0}, actual was {1}... Why would you do this :(";
        
        public readonly BufferState Expected;
        public readonly BufferState Actual;

        public InvalidBufferStateException(BufferState actual, BufferState expected) : base(string.Format(MESSAGE, expected, actual))
        {
            Expected = expected;
            Actual = actual;
        }
    }
}