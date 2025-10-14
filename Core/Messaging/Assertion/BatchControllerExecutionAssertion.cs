using IdelPog.Core.Logging;
using IdelPog.Core.Messaging.Assertion.Interface;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Handler.Interface;

namespace IdelPog.Core.Messaging.Assertion
{
    public sealed class BatchControllerExecutionAssertion<T> : ContextualAssertion<IReadOnlyList<T>>, IBatchControllerExecutionAssertion<T>
    {
        private readonly IBufferLogger _bufferLogger;
        
        public BatchControllerExecutionAssertion(IContextualHandler<IReadOnlyList<T>> contextualHandler, IBufferLogger bufferLogger) : base(contextualHandler)
        {
            _bufferLogger = bufferLogger;
        }

        public void AssertBatchExecutesWithoutError(IBatchController<T> controller, IReadOnlyList<T> messages)
        {
            AssertAndHandle<ControllerThrownException>(() =>
            {
                try
                {
                    controller.HandleMessages(messages);
                }
                catch (Exception exception)
                {
                    _bufferLogger.LogError(messages.ToArray(), exception);
                    throw new ControllerThrownException(controller.GetType().Name, exception);
                }
            }, messages);
        }
    }
}