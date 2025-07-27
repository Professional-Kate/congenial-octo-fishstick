using IdelPog.Messaging.Factory;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.Messaging.Dispatch
{
    public sealed class DispatchingHandler<TErrorDTO, TContext> : IHandler
    {
        private readonly IDispatchOne<TErrorDTO> _dispatcher;
        private readonly IErrorFactory<TErrorDTO, TContext> _errorFactory;
        private readonly TContext _context;

        public DispatchingHandler(IDispatchOne<TErrorDTO> dispatcher, IErrorFactory<TErrorDTO, TContext> errorFactory, TContext context)
        {
            _dispatcher = dispatcher;
            _errorFactory = errorFactory;
            _context = context;
        }

        public void Handle(Exception exception)
        {
            TErrorDTO errorDTO = _errorFactory.Create(_context, exception);
            _dispatcher.Dispatch(errorDTO);
        }
    }
}