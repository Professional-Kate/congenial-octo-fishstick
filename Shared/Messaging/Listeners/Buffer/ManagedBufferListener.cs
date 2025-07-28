using IdelPog.Messaging.Assertions;

namespace IdelPog.Messaging.Listeners.Buffer
{
    public sealed class ManagedBufferListener<T> : IBufferListener<T>
    {
        private readonly IBatchController<T> _controller;
        private readonly IBatchControllerExecutionAssertion<T> _singleControllerExecutionAssertion;

        public ManagedBufferListener(IBatchController<T> controller, IBatchControllerExecutionAssertion<T> singleControllerExecutionAssertion)
        {
            _controller = controller;
            _singleControllerExecutionAssertion = singleControllerExecutionAssertion;
        }

        public Type ListenerType => typeof(T);
        
        public void Handle(IReadOnlyList<T> buffer)
        {
            _singleControllerExecutionAssertion.AssertBatchExecutesWithoutError(_controller, buffer);
        }

    }
}