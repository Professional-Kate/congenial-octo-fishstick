using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Listeners.Buffer;

namespace IdelPog.Messaging.Listeners.Single
{
    public sealed class ManagedSingleListener<T> : ISingleListener<T>
    {
        private readonly ISingleController _controller;
        private readonly IThrowingAssertion _throwingAssertion;

        public ManagedSingleListener(ISingleController controller, IThrowingAssertion throwingAssertion)
        {
            _controller = controller;
            _throwingAssertion = throwingAssertion;
        }

        public Type ListenerType => typeof(T);
        
        public void Handle(T item)
        {
            _throwingAssertion.AssertDoesNotThrow(item, _controller);
        }
    }
}