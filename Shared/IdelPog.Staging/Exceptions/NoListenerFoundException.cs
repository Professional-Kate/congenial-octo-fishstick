using IdelPog.Staging.Messaging;
using IdelPog.Validation.Constants;

namespace IdelPog.Staging.Exceptions
{
    public class NoListenerFoundException : Exception
    {
        private const string BASE_MESSAGE = ExceptionConstants.NO_LISTENER_FOUND_MESSAGE;

        public NoListenerFoundException(IListener listener) 
            : base(string.Format(BASE_MESSAGE, listener, listener.GetType()))
        {
            // TODO 
        }
    }
}