using IdelPog.Messaging.Assertions;

namespace IdelPog.Messaging.Listeners.Buffer
{
    public sealed class ManagedBufferListener<T> : IBufferListener<T>
    {
        private readonly IBatchedController<T> _controller;
        private readonly IThrowingAssertion _throwingAssertion;

        public ManagedBufferListener(IBatchedController<T> controller, IThrowingAssertion throwingAssertion)
        {
            _controller = controller;
            _throwingAssertion = throwingAssertion;
        }

        public Type ListenerType => typeof(T);
        
        public void Handle(IReadOnlyList<T> buffer)
        {
            _throwingAssertion.AssertDoesNotThrow(buffer, _controller);
        }

    }
}