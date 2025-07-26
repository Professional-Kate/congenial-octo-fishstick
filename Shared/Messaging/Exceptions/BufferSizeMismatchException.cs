using IdelPog.Validation.Constants;

namespace IdelPog.Messaging.Exceptions
{
    public class BufferSizeMismatchException : Exception
    {
        private const string BASE_MESSAGE = ExceptionConstants.BUFFER_SIZE_MISMATCH_MESSAGE;

        public BufferSizeMismatchException(int expectedSize, int actualSize)
            : base(string.Format(BASE_MESSAGE, expectedSize, actualSize))
        {
            // TODO : don't say it
        }
    }
}