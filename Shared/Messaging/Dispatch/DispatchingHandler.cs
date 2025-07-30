using IdelPog.Common.Factories;
using IdelPog.Messaging.Dispatch.Single;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.Messaging.Dispatch
{
    public sealed class DispatchingHandler<TError, TContext> : IContextualHandler<TContext>
    {
        private readonly IDispatchOne<TError> _dispatcher;
        private readonly IErrorFactory<TError, TContext> _errorFactory;

        public DispatchingHandler(IDispatchOne<TError> dispatcher, IErrorFactory<TError, TContext> errorFactory)
        {
            _dispatcher = dispatcher;
            _errorFactory = errorFactory;
        }

        public void Handle<TException>(TException exception, TContext context)where TException : Exception
        {
            _dispatcher.Dispatch(_errorFactory.Create(context, exception));
        }
    }
}