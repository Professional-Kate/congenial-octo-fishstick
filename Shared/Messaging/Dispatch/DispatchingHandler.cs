using IdelPog.Common.Factories;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.Messaging.Dispatch
{
    public sealed class DispatchingHandler<TErrorDTO, TContext> : IContextualHandler<TContext>
    {
        private readonly IDispatchOne<TErrorDTO> _dispatcher;
        private readonly IErrorFactory<TErrorDTO, TContext> _errorFactory;

        public DispatchingHandler(IDispatchOne<TErrorDTO> dispatcher, IErrorFactory<TErrorDTO, TContext> errorFactory)
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