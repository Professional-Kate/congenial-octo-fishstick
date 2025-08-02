using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.Validation.Assertions
{
    public abstract class ContextualAssertion<TContext>
    {
        private readonly IContextualHandler<TContext> _contextualHandler;

        protected ContextualAssertion(IContextualHandler<TContext> contextualHandler)
        {
            _contextualHandler = contextualHandler;
        }

        protected void AssertAndHandle<TException>(Action action, TContext context) where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException exception)
            {
                _contextualHandler.Handle(exception, context);
            }
        }
    }
}