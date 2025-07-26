using IdelPog.Validation.Constants;

namespace IdelPog.Messaging.Exceptions
{
    public class BufferSizeMismatchException : Exception
    {
        private const string BASE_MESSAGE = ExceptionConstants.BUFFER_SIZE_MISMATCH_MESSAGE;

        public BufferSizeMismatchException(int actual, int expected)
            : base(string.Format(BASE_MESSAGE, actual, expected))
        {
            // TODO : don't say it
        }
    }
}