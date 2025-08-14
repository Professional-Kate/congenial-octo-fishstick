using IdelPog.Core.Messaging.Assertion.Interface;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Handler.Interface;

namespace IdelPog.Core.Messaging.Assertion
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