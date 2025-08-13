using IdelPog.Core.Messaging.Assertion.Interface;

namespace IdelPog.Core.Messaging.Listener.Buffer
{
    public sealed class ManagedBufferListener<T> : IBufferListener<T>
    {
        private readonly IBatchController<T> _controller;
        private readonly IBatchControllerExecutionAssertion<T> _batchControllerExecutionAssertion;

        public ManagedBufferListener(IBatchController<T> controller, IBatchControllerExecutionAssertion<T> batchControllerExecutionAssertion)
        {
            _controller = controller;
            _batchControllerExecutionAssertion = batchControllerExecutionAssertion;
        }

        public Type ListenerType => typeof(T);
        
        public void Handle(IReadOnlyList<T> buffer)
        {
            _batchControllerExecutionAssertion.AssertBatchExecutesWithoutError(_controller, buffer);
        }

    }
}