using IdelPog.Core.Logging;
using IdelPog.Core.Logging.Contracts;
using IdelPog.Core.Messaging.Assertion.Interface;

namespace IdelPog.Core.Messaging.Listener.Buffer
{
    public sealed class ManagedBufferListener<T> : IBufferListener<T>
    {
        private readonly IBatchController<T> _controller;
        private readonly IBatchControllerExecutionAssertion<T> _batchControllerExecutionAssertion;
        private readonly ILogger _logger;

        public ManagedBufferListener(IBatchController<T> controller, IBatchControllerExecutionAssertion<T> batchControllerExecutionAssertion, ILogger logger)
        {
            _controller = controller;
            _batchControllerExecutionAssertion = batchControllerExecutionAssertion;
            _logger = logger;
        }

        public Type ListenerType => typeof(T);
        
        public void Handle(IReadOnlyList<T> buffer)
        {
            _logger.LogInfo(LogDirection.IN, buffer.ToArray());
            _batchControllerExecutionAssertion.AssertBatchExecutesWithoutError(_controller, buffer);
        }

    }
}