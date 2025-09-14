using IdelPog.Core.Logging;
using IdelPog.Core.Messaging.Assertion.Interface;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Messaging.Listener.Single;
using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Handler.Interface;

namespace IdelPog.Core.Messaging.Assertion
{
    public class SingleControllerExecutionAssertion<TContext> : ContextualAssertion<TContext>, ISingleControllerExecutionAssertion<TContext> where TContext : struct
    {
        private readonly ILogger _logger;
        
        public SingleControllerExecutionAssertion(IContextualHandler<TContext> handler, ILogger logger) : base(handler)
        {
            _logger = logger;
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
                    _logger.LogError([message], exception);
                    throw new ControllerThrownException(controller.GetType().Name, exception);
                    
                }
            }, message);
        }
    }
}