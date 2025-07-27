using IdelPog.Messaging.Listeners;

namespace IdelPog.Messaging.Exceptions
{
    public class NoListenerFoundException : Exception
    {
        private const string MESSAGE = "The Listener {0} for type {1} was not found!";

        public readonly IListener Listener;
        public readonly Type ListenerType;

        public NoListenerFoundException(IListener listener) : base(string.Format(MESSAGE, listener, listener.ListenerType))
        {
            Listener = listener;
            ListenerType = listener.ListenerType;
        }
    }
}