using IdelPog.Messaging.Exceptions;
using IdelPog.Messaging.Listeners.Buffer;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.Messaging.Assertions
{
    public class BatchControllerExecutionAssertion<T> : ContextualAssertion<IReadOnlyList<T>>, IBatchControllerExecutionAssertion<T>
    {
        public BatchControllerExecutionAssertion(IContextualHandler<IReadOnlyList<T>> contextualHandler) : base(contextualHandler)
        {
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
                    throw new ControllerThrownException(controller.GetType().Name, exception);
                }
            }, messages);
        }
    }
}