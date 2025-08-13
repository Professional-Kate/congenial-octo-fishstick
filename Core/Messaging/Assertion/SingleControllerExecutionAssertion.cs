using IdelPog.Core.Messaging.Assertion.Interface;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Messaging.Listener.Single;
using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Handler.Interface;

namespace IdelPog.Core.Messaging.Assertion
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