using IdelPog.Core.Logging;
using IdelPog.Core.Logging.Contracts;
using IdelPog.Core.Messaging.Assertion.Interface;

namespace IdelPog.Core.Messaging.Listener.Buffer
{
    public sealed class ManagedBufferListener<T> : IBufferListener<T>
    {
        private readonly IBatchController<T> _controller;
        private readonly IBatchControllerExecutionAssertion<T> _batchControllerExecutionAssertion;
        private readonly IBufferLogger _bufferLogger;

        public ManagedBufferListener(IBatchController<T> controller, IBatchControllerExecutionAssertion<T> batchControllerExecutionAssertion, IBufferLogger bufferLogger)
        {
            _controller = controller;
            _batchControllerExecutionAssertion = batchControllerExecutionAssertion;
            _bufferLogger = bufferLogger;
        }

        public Type ListenerType => typeof(T);
        
        public void Handle(IReadOnlyList<T> buffer)
        {
            _bufferLogger.LogInfo(LogDirection.IN, buffer.ToArray());
            _batchControllerExecutionAssertion.AssertBatchExecutesWithoutError(_controller, buffer);
        }

    }
}