using IdelPog.Messaging.Exceptions;
using IdelPog.Messaging.Listeners.Buffer;
using IdelPog.Messaging.Listeners.Single;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.Messaging.Assertions
{
    public class SingleControllerExecutionAssertion<TContext> : ContextualAssertion<TContext>, ISingleControllerExecutionAssertion<TContext>
    {
        public SingleControllerExecutionAssertion(IContextualHandler<TContext> handler) : base(handler)
        {
        }

        public void AssertExecutesWithoutError(ISingleController<TContext> controller, TContext message)
        {
            AssertAndHandle<ControllerThrownException>(() =>
            {
                try
                {
                    controller.HandleMessage(message);
                }
                catch (Exception exception)
                {
                    throw new ControllerThrownException(controller.GetType().Name, exception);
                    
                }
            }, message);
        }
    }
}