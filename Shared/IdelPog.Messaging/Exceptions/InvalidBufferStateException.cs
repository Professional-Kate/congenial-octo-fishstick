using IdelPog.Messaging.Collection;
using IdelPog.Validation.Constants;

namespace IdelPog.Messaging.Exceptions
{
    public class InvalidBufferStateException : Exception
    {
        private const string BASE_MESSAGE = ExceptionConstants.BUFFER_STATE_INVALID_MESSAGE;

        public InvalidBufferStateException(BufferState expected, BufferState actual) 
            : base(string.Format(BASE_MESSAGE, expected, actual))
        {
            // TODO : I will get to it
        }
    }
}