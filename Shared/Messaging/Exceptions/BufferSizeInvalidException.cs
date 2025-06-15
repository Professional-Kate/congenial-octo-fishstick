using IdelPog.Validation.Constants;

namespace IdelPog.Messaging.Exceptions
{
    public class BufferSizeInvalidException : Exception
    {
        private const string BASE_MESSAGE = ExceptionConstants.BUFFER_SIZE_INVALID_MESSAGE;

        public BufferSizeInvalidException(int size) 
            : base(string.Format(BASE_MESSAGE, size))
        {
            // TODO : hoo hee ha ha
        }
    }
}